using System;
using System.Linq;

using Android.Views.Accessibility;

using Autofac;

using ScreenReaderService.Data;

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

        private const string STATUS_AVAILABLE = "Доступно";

        public override bool IsValuableEvent(AccessibilityEvent e)
        {
            return e.Source != null;
        }

        public override async void ProcessEvent(AccessibilityEvent e)
        {
            try
            {
                DateTime start = DateTime.Now;

                if (BotService.WorkService.WorkInfo.ActiveOrders.Count == BotService.ConstraintsService.Constraints.MaxActiveOrdersAtTime)
                {
                    await Notifier.NotifyMessage("Scanning skipped due to: 'active orders count exceeded'");
                    return;
                }

                int todayTakenOrdersCount = BotService.WorkService.WorkInfo.ActiveOrders.Count + 
                                            BotService.WorkService.WorkInfo.DoneOrders.Count;

                if (todayTakenOrdersCount == BotService.ConstraintsService.Constraints.MaxOrdersPerDay)
                {
                    await Notifier.NotifyMessage("Scanning skipped due to: 'daily orders count exceeded'");
                    return;
                }

                if (BotService.StateService.StateInfo.Paused)
                {
                    await Notifier.NotifyMessage("Scanning skipped due to: 'bot paused'");
                    return;
                }

                AccessibilityNodeInfo root = GetRoot(e.Source);
                AccessibilityNodeInfo list = root.FindAccessibilityNodeInfosByViewId(ORDER_LIST_ID).FirstOrDefault();

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
                    AccessibilityNodeInfo deliveryTypeView = child.FindAccessibilityNodeInfosByViewId(ORDER_DELIVERY_TYPE_ID).FirstOrDefault();
                    AccessibilityNodeInfo typeView = child.FindAccessibilityNodeInfosByViewId(ORDER_TYPE_ID).FirstOrDefault();

                    if (fromView == null || toView == null)
                        continue;

                    Order order = new Order(
                        BotService.StateService.StateInfo.IdCounter + i, 
                        fromView.Text, toView.Text
                    );

                    if (deliveryTypeView != null && !string.IsNullOrEmpty(deliveryTypeView.Text))
                        order.Modifiers.Add(deliveryTypeView.Text);

                    if (typeView != null && !string.IsNullOrEmpty(typeView.Text))
                        order.Modifiers.Add(typeView.Text);

                    if (!BotService.FilterService.Assert(order))
                        continue;

                    ScreenReader.EventProcessor = DependencyHolder.Dependencies.Resolve<OrderPageEventProcessor>();

                    ++BotService.StateService.StateInfo.IdCounter;
                    BotService.StateService.StateInfo.DiscoveredOrder = order;
                    BotService.StateService.Save();

                    Click(child);

                    break;
                }

                TimeSpan duration = DateTime.Now - start;

                await Notifier.NotifyMessage($"Scanning {list.ChildCount} orders took {duration.TotalMilliseconds} ms");

                //ScreenReader.UpdateNeeded = true;
            }
            catch (Exception ex) { await NotifyException(ex); }
        }
    }
}