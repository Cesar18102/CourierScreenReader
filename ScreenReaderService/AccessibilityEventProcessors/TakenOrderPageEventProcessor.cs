using Newtonsoft.Json;

using Autofac;

using Android.Views.Accessibility;

namespace ScreenReaderService.AccessibilityEventProcessors
{
    public class TakenOrderPageEventProcessor : AccessabilityEventProcessorBase
    {
        private bool NeedGoBack { get; set; }
        private const string ORDER_TAKEN_MESSAGE_TEXT = "Доставка принята!";

        public override bool IsValuableEvent(AccessibilityEvent e)
        {
            return e.Source != null;
        }

        public override async void ProcessEvent(AccessibilityEvent e)
        {
            AccessibilityNodeInfo root = GetRoot(e.Source);
            AccessibilityNodeInfo message = GetMessageBox(root);

            if (NeedGoBack)
            {
                if (Back(root))
                {
                    NeedGoBack = false;
                    ScreenReader.EventProcessor = DependencyHolder.Dependencies.Resolve<OrderListPageEventProcessor>();
                }

                return;
            }

            if (message.Text == ORDER_TAKEN_MESSAGE_TEXT)
            {
                BotInfo.ActiveOrders.Add(BotInfo.DiscoveredOrder);

                string orderJson = JsonConvert.SerializeObject(BotInfo.DiscoveredOrder);
                await Notifier.NotifyMessage(orderJson);

                BotInfo.DiscoveredOrder = null;
                NeedGoBack = true;
            }
        }
    }
}