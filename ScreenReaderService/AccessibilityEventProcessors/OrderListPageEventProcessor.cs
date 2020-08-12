using System;
using System.Linq;
using System.Text.RegularExpressions;

using Android.Views.Accessibility;

using Autofac;

using ScreenReaderService.Gestures;

using ScreenReaderService.Data;
using ScreenReaderService.Data.Exceptions;

namespace ScreenReaderService.AccessibilityEventProcessors
{
    public class OrderListPageEventProcessor : AccessabilityEventProcessorBase
    {
        private const string ORDER_LIST_ID = "ua.ipost.work:id/list";
        private const string ORDER_STATUS_ID = "ua.ipost.work:id/tvStatus";
        private const string ORDER_FROM_ID = "ua.ipost.work:id/tvFromName";
        private const string ORDER_TO_ID = "ua.ipost.work:id/tvToName";
        private const string ORDER_DELIVERY_TYPE_ID = "ua.ipost.work:id/tvDeliveryType";
        private const string ORDER_TYPE_ID = "ua.ipost.work:id/tvType";
        private const string ORDER_GAIN_ID = "ua.ipost.work:id/tvPrice";
        private const string ORDER_TAKE_TIME_ID = "ua.ipost.work:id/tvFromTime";
        private const string ORDER_DELIVER_TIME_ID = "ua.ipost.work:id/tvToTime";

        private const string STATUS_AVAILABLE = "Доступно";

        private const string TODAY = "сегодня";
        private const string TODAY_ENG_C = "cегодня";
        private const string TOMORROW = "завтра";

        private static readonly Regex TAKE_TIME_REGEX = new Regex($"Забрать\\s*({TODAY}|{TODAY_ENG_C}|{TOMORROW})\\s*:\\s*с\\s*(\\d+)\\s*:\\s*(\\d+)\\s*до\\s*(\\d+)\\s*:\\s*(\\d+)");
        private static readonly Regex DELIVER_TIME_REGEX = new Regex($"Доставить\\s*({TODAY}|{TODAY_ENG_C}|{TOMORROW})\\s*:\\s*с\\s*(\\d+)\\s*:\\s*(\\d+)\\s*до\\s*(\\d+)\\s*:\\s*(\\d+)");

        private UpdateGestureService UpdateGestureService = DependencyHolder.Dependencies.Resolve<UpdateGestureService>();

        public override bool IsValuableEvent(AccessibilityEvent e)
        {
            return e.Source != null;
        }

        public override async void ProcessEvent(AccessibilityEvent e)
        {
            try
            {
                DateTime start = DateTime.Now;

                if (BotService.StateService.StateInfo.Paused)
                {
                    await Notifier.NotifyMessage("Scanning skipped due to: 'bot paused'");
                    return;
                }

                try { BotService.ConstraintsService.CheckInvariantConstraintsPassed(); }
                catch(ConstraintNotPassedException ex)
                {
                    await Notifier.NotifyMessage($"Scanning skipped due to: '{ex.Reason}'");
                    return;
                }

                AccessibilityNodeInfo root = GetRoot(e.Source);
                AccessibilityNodeInfo list = root.FindAccessibilityNodeInfosByViewId(ORDER_LIST_ID).FirstOrDefault();

                string json = GetAllText(root);

                if (list == null || list.ChildCount == 0)
                {
                    await Notifier.NotifyMessage("Scanning skipped due to: 'no orders listed'");
                    return;
                }

                for (int i = 0; i < list.ChildCount; ++i)
                {
                    AccessibilityNodeInfo child = list.GetChild(i);
                    AccessibilityNodeInfo statusView = child.FindAccessibilityNodeInfosByViewId(ORDER_STATUS_ID).FirstOrDefault();

                    if (statusView != null && statusView.Text != STATUS_AVAILABLE)
                        continue;

                    AccessibilityNodeInfo fromView = child.FindAccessibilityNodeInfosByViewId(ORDER_FROM_ID).FirstOrDefault();
                    AccessibilityNodeInfo toView = child.FindAccessibilityNodeInfosByViewId(ORDER_TO_ID).FirstOrDefault();
                    AccessibilityNodeInfo gainView = child.FindAccessibilityNodeInfosByViewId(ORDER_GAIN_ID).FirstOrDefault();

                    if (fromView == null || toView == null || gainView == null)
                        continue;

                    Order order = new Order(
                        BotService.StateService.StateInfo.IdCounter + i, 
                        fromView.Text, toView.Text, (int)Convert.ToDouble(gainView.Text)
                    );

                    AccessibilityNodeInfo deliveryTypeView = child.FindAccessibilityNodeInfosByViewId(ORDER_DELIVERY_TYPE_ID).FirstOrDefault();
                    if (deliveryTypeView != null && !string.IsNullOrEmpty(deliveryTypeView.Text))
                        order.Modifiers.Add(deliveryTypeView.Text);

                    AccessibilityNodeInfo typeView = child.FindAccessibilityNodeInfosByViewId(ORDER_TYPE_ID).FirstOrDefault();
                    if (typeView != null && !string.IsNullOrEmpty(typeView.Text))
                        order.Modifiers.Add(typeView.Text);

                    AccessibilityNodeInfo fromTimeNode = child.FindAccessibilityNodeInfosByViewId(ORDER_TAKE_TIME_ID).FirstOrDefault();
                    if (fromTimeNode != null && !string.IsNullOrEmpty(fromTimeNode.Text))
                    {
                        (DateTime min, DateTime max)? takeTime = GetPeriodFromText(fromTimeNode.Text, TAKE_TIME_REGEX);

                        if (takeTime.HasValue)
                        {
                            order.MinTakeTime = takeTime.Value.min;
                            order.MaxTakeTime = takeTime.Value.max;
                        }
                    }

                    AccessibilityNodeInfo toTimeNode = child.FindAccessibilityNodeInfosByViewId(ORDER_DELIVER_TIME_ID).FirstOrDefault();
                    if (toTimeNode != null && !string.IsNullOrEmpty(toTimeNode.Text))
                    {
                        (DateTime min, DateTime max)? deliverTime = GetPeriodFromText(toTimeNode.Text, DELIVER_TIME_REGEX);

                        if (deliverTime.HasValue)
                        {
                            order.MinDeliverTime = deliverTime.Value.min;
                            order.MaxDeliverTime = deliverTime.Value.max;
                        }
                    }

                    if (BotService.BadOrdersService.OrdersBlackList.Contains(order))
                        continue;

                    if (!BotService.FilterService.Assert(order))
                        continue;

                    try { BotService.ConstraintsService.PreOpenCheckOrderPassesConstraints(order); }
                    catch (ConstraintNotPassedException ex) { continue; }

                    ScreenReader.EventProcessor = DependencyHolder.Dependencies.Resolve<OrderPageEventProcessor>();

                    ++BotService.StateService.StateInfo.IdCounter;
                    BotService.StateService.StateInfo.DiscoveredOrder = order;
                    BotService.StateService.Save();

                    Click(child);

                    break;
                }

                TimeSpan duration = DateTime.Now - start;

                await Notifier.NotifyMessage($"Scanning took {duration.TotalMilliseconds} ms ({list.ChildCount} orders)");

                //ScreenReader.GesturesToPerform.Enqueue(UpdateGestureService.Gesture);
            }
            catch (Exception ex) { await NotifyException(ex); }
        }

        private (DateTime min, DateTime max)? GetPeriodFromText(string text, Regex timeParseRegex)
        {
            MatchCollection matches = timeParseRegex.Matches(text);

            if (matches.Count == 0)
                return null;

            DateTime now = DateTime.Now;

            DateTime min = GetDateTimeFromMatch(now, matches[0], 1, 2, 3);
            DateTime max = GetDateTimeFromMatch(now, matches[0], 1, 4, 5);

            return (min, max);
        }

        private DateTime GetDateTimeFromMatch(DateTime now, Match match, int dayGroupNumber, int hourGroupNumber, int minuteGroupNumber)
        {
            int hour = Convert.ToInt32(match.Groups[hourGroupNumber].Value);
            int minute = Convert.ToInt32(match.Groups[minuteGroupNumber].Value);

            DateTime dateTime = new DateTime(now.Year, now.Month, now.Day, hour, minute, 0);

            return match.Groups[dayGroupNumber].Value == TOMORROW ? dateTime.AddDays(1) : dateTime;
        }
    }
}