using System;
using System.Linq;

using Android.Views.Accessibility;

using Autofac;

namespace ScreenReaderService.AccessibilityEventProcessors
{
    public class OrderPageEventProcessor : AccessabilityEventProcessorBase
    {
        private bool NeedGoBack { get; set; }

        private const int INIT_STATE_MENU_VIEW_COUNT = 2;
        private const int OPEN_OPTIONS_BUTTON_NUMBER = 0;

        private const string ERROR_MESSAGE_TEXT = "Этот заказ больше недоступен.";
        private const string ERROR_MONEY_PREFIX = "Для того чтобы взять этот заказ, Вам необходимо пополнить баланс не меньше чем на";

        private const string MESSAGE_TEXT_SIMPLE = "Вы уверены, что хотите принять эту доставку в работу?";
        private const string MESSAGE_TEXT_MONEY_PREFIX = "Для того чтобы принять этот заказ, у вас должно быть ";
        private const string MESSAGE_TEXT_MONEY_ENDIAN = " гривен наличных денег для выкупа отправления. Вы готовы выполнить этот заказ?";

        private const string OPTIONS_BUTTONS_WRAPPER_ID = "ua.ipost.work:id/fab_menu";
        private const string ACCEPT_ORDER_TEXT = "Принять заказ";

        private enum ConfirmationState
        {
            INIT,
            OPENED,
            ACCEPTED,
            CONFIRMED
        };

        private ConfirmationState State { get; set; } = ConfirmationState.INIT;

        public override bool IsValuableEvent(AccessibilityEvent e)
        {
            return e.Source != null &&
                   e.Source.ViewIdResourceName != OPTIONS_BUTTONS_WRAPPER_ID &&
                  (e.Source.Parent == null || e.Source.Parent.ViewIdResourceName == OPTIONS_BUTTONS_WRAPPER_ID);
        }

        public override async void ProcessEvent(AccessibilityEvent e)
        {
            try
            {
                AccessibilityNodeInfo root = GetRoot(e.Source);
                string jsin = GetAllText(root);

                if (NeedGoBack)
                {
                    if (Back(root))
                    {
                        NeedGoBack = false;
                        State = ConfirmationState.INIT;
                        ScreenReader.EventProcessor = DependencyHolder.Dependencies.Resolve<OrderListPageEventProcessor>();
                    }

                    return;
                }

                if (HandleErrorIfOccured(root))
                    return;

                switch (State)
                {
                    case ConfirmationState.INIT:

                        if (OpenOptions(root))
                            State = ConfirmationState.OPENED;

                        break;

                    case ConfirmationState.OPENED:

                        if (AcceptOrder(root))
                            State = ConfirmationState.ACCEPTED;

                        break;

                    case ConfirmationState.ACCEPTED:

                        if (ConfirmOrder(root))
                            State = ConfirmationState.CONFIRMED;

                        await Notifier.NotifyMessage(GetAllText(root));

                        break;

                    case ConfirmationState.CONFIRMED:

                        State = ConfirmationState.INIT;
                        ScreenReader.EventProcessor = DependencyHolder.Dependencies.Resolve<TakenOrderPageEventProcessor>();

                        break;
                }
            }
            catch(Exception ex) { await NotifyException(ex); }
        }

        private bool OpenOptions(AccessibilityNodeInfo root)
        {
            AccessibilityNodeInfo menu = root.FindAccessibilityNodeInfosByViewId(OPTIONS_BUTTONS_WRAPPER_ID).FirstOrDefault();

            if (menu != null && menu.ChildCount == INIT_STATE_MENU_VIEW_COUNT)
                return Click(menu.GetChild(OPEN_OPTIONS_BUTTON_NUMBER));

            return false;
        }

        private bool AcceptOrder(AccessibilityNodeInfo root)
        {
            AccessibilityNodeInfo accept = root.FindAccessibilityNodeInfosByText(ACCEPT_ORDER_TEXT).FirstOrDefault();

            if (accept != null)
                return Click(accept);

            return false;
        }

        private bool ConfirmOrder(AccessibilityNodeInfo root)
        {
            AccessibilityNodeInfo message = GetMessageBox(root);

            if (message == null)
                return false;

            bool isConfirmationMessageWithMoneySpec = message.Text.StartsWith(MESSAGE_TEXT_MONEY_PREFIX) &&
                                                      message.Text.EndsWith(MESSAGE_TEXT_MONEY_ENDIAN);

            if (isConfirmationMessageWithMoneySpec || message.Text == MESSAGE_TEXT_SIMPLE)
                return ConfirmMessageBox(root);

            return false;
        }

        private bool HandleErrorIfOccured(AccessibilityNodeInfo root)
        {
            AccessibilityNodeInfo message = GetMessageBox(root);

            if (message != null && (message.Text == ERROR_MESSAGE_TEXT || message.Text.StartsWith(ERROR_MONEY_PREFIX)))
            {
                RefuseMessageBox(root);
                NeedGoBack = true;
                return true;
            }

            return false;
        }
    }
}