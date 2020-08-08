using System;
using System.Linq;

using Autofac;

using Android.Views.Accessibility;

namespace ScreenReaderService.AccessibilityEventProcessors
{
    public class TakenOrderPageEventProcessor : AccessabilityEventProcessorBase
    {
        private bool NeedGoBack { get; set; }

        private const string ORDER_TAKEN_MESSAGE_ID = "ua.ipost.work:id/tvStatus";
        private const string ORDER_TAKEN_MESSAGE_TEXT = "Доставка принята!";

        public override bool IsValuableEvent(AccessibilityEvent e)
        {
            return e.Source != null;
        }

        public override async void ProcessEvent(AccessibilityEvent e)
        {
            try
            {
                AccessibilityNodeInfo root = GetRoot(e.Source);
                AccessibilityNodeInfo message = root.FindAccessibilityNodeInfosByViewId(ORDER_TAKEN_MESSAGE_ID).FirstOrDefault();

                /*/////TEMP
                string endPageJson = GetAllText(root);
                await Notifier.NotifyMessage("**END PAGE JSON**\n" + endPageJson);
                //////TEMP*/

                if (message != null && message.Text == ORDER_TAKEN_MESSAGE_TEXT)
                {
                    BotService.WorkService.WorkInfo.ActiveOrders.Add(
                        BotService.StateService.StateInfo.DiscoveredOrder
                    );
                    BotService.WorkService.Save();

                    ConfirmMessageBox(root);

                    await Notifier.NotifyMessage(
                        $"@{BotService.CredentialsService.Credentials.TelegramUsername}, your bot've taken an order: \n" +
                        $"{BotService.StateService.StateInfo.DiscoveredOrder.ToString()}"
                    );

                    BotService.StateService.StateInfo.DiscoveredOrder = null;
                    BotService.StateService.Save();

                    NeedGoBack = true;
                    return;
                }

                if (NeedGoBack && Back(root))
                {
                    NeedGoBack = false;
                    ScreenReader.EventProcessor = DependencyHolder.Dependencies.Resolve<OrderListPageEventProcessor>();
                }
            }
            catch(Exception ex) { await NotifyException(ex); }
        }
    }
}