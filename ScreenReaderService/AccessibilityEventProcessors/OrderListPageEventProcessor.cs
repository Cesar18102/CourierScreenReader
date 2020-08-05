using System.Linq;

using Android.Views.Accessibility;

using Autofac;

using Newtonsoft.Json;

using ScreenReaderService.Data;
using ScreenReaderService.Data.Services;

namespace ScreenReaderService.AccessibilityEventProcessors
{
    public class OrderListPageEventProcessor : AccessabilityEventProcessorBase
    {
        private FilterService FilterService = DependencyHolder.Dependencies.Resolve<FilterService>();
        private ConstraintsConfigService ConstraintsConfigService = DependencyHolder.Dependencies.Resolve<ConstraintsConfigService>();

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

        public override void ProcessEvent(AccessibilityEvent e)
        {
            if (BotInfo.ActiveOrders.Count == ConstraintsConfigService.Constraints.MaxActiveOrdersAtTime)
                return;

            if (BotInfo.ActiveOrders.Count + BotInfo.DoneOrders.Count == ConstraintsConfigService.Constraints.MaxOrdersPerDay)
                return;

            if (BotInfo.Paused)
                return;

            AccessibilityNodeInfo root = GetRoot(e.Source);
            AccessibilityNodeInfo list = root.FindAccessibilityNodeInfosByViewId(ORDER_LIST_ID).FirstOrDefault();

            if (list == null || list.ChildCount == 0)
                return;

            for(int i = 0; i < list.ChildCount; ++i)
            {
                AccessibilityNodeInfo child = list.GetChild(i);
                AccessibilityNodeInfo statusView = child.FindAccessibilityNodeInfosByViewId(ORDER_STATUS_ID).FirstOrDefault();

                if (statusView != null && statusView.Text != STATUS_AVAILABLE)
                    continue;

                AccessibilityNodeInfo fromView = child.FindAccessibilityNodeInfosByViewId(ORDER_FROM_ID).FirstOrDefault();
                AccessibilityNodeInfo toView = child.FindAccessibilityNodeInfosByViewId(ORDER_TO_ID).FirstOrDefault();
                AccessibilityNodeInfo deliveryTypeView = child.FindAccessibilityNodeInfosByViewId(ORDER_DELIVERY_TYPE_ID).FirstOrDefault();
                AccessibilityNodeInfo typeView = child.FindAccessibilityNodeInfosByViewId(ORDER_TYPE_ID).FirstOrDefault();

                Order order = new Order(++BotInfo.IdCounter, fromView.Text, toView.Text);

                if (deliveryTypeView != null && !string.IsNullOrEmpty(deliveryTypeView.Text))
                    order.Modifiers.Add(deliveryTypeView.Text);

                if (typeView != null && !string.IsNullOrEmpty(typeView.Text))
                    order.Modifiers.Add(typeView.Text);

                if (!FilterService.Assert(order))
                    continue;

                Click(child);
                BotInfo.DiscoveredOrder = order;
                ScreenReader.EventProcessor = DependencyHolder.Dependencies.Resolve<OrderPageEventProcessor>();

                break;
            }
        }
    }
}