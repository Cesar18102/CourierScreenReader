using System;

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
            try
            {
                AccessibilityNodeInfo root = GetRoot(e.Source);
                AccessibilityNodeInfo message = GetMessageBox(root);


                /////TEMP
                string endPageJson = GetAllText(root);
                await Notifier.NotifyMessage("**END PAGE JSON**\n" + endPageJson);
                //////TEMP


                if (NeedGoBack)
                {
                    if (Back(root))
                    {
                        NeedGoBack = false;
                        ScreenReader.EventProcessor = DependencyHolder.Dependencies.Resolve<OrderListPageEventProcessor>();
                    }

                    return;
                }

                //message not null
                if (message.Text == ORDER_TAKEN_MESSAGE_TEXT)
                {
                    BotInfo.ActiveOrders.Add(BotInfo.DiscoveredOrder);
                    //add  confirming popup
                    await Notifier.NotifyMessage(
                        $"@{CredentialsConfigService.Credentials.TelegramUsername}, your bot've taken an order: \n" +
                        $"{BotInfo.DiscoveredOrder.ToString()}"
                    );

                    BotInfo.DiscoveredOrder = null;
                    NeedGoBack = true;
                }
            }
            catch(Exception ex) { await NotifyException(ex); }
        }
    }
}
