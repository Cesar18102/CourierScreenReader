using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Android.Views.Accessibility;

using Autofac;

using ScreenReaderService.Telegram;
using ScreenReaderService.Services;
using ScreenReaderService.Data.Services;

namespace ScreenReaderService.AccessibilityEventProcessors
{
    public abstract class AccessabilityEventProcessorBase : IAccessabilityEventProcessor
    {
        protected const string TOOLBAR_ID = "ua.ipost.work:id/toolbar";
        protected const int TOOLBAR_CHILDREN_MIN_COUNT = 1;
        protected const int TOOLBAR_BACK_BUTTON_NUM = 0;

        protected const string MESSAGE_BOX_ID = "android:id/message";
        protected const string BUTTON_PANEL_ID = "ua.ipost.work:id/buttonPanel";

        protected const string ACCEPT_MESSAGE_BOX_BUTTON = "android:id/button1";
        protected const string REFUSE_MESSAGE_BOX_BUTTON = "android:id/button2";

        protected BotService BotService = DependencyHolder.Dependencies.Resolve<BotService>();
        protected TelegramNotifier Notifier = DependencyHolder.Dependencies.Resolve<TelegramNotifier>();

        public abstract bool IsValuableEvent(AccessibilityEvent e);
        public abstract void ProcessEvent(AccessibilityEvent e);

        protected AccessibilityNodeInfo GetRoot(AccessibilityNodeInfo node)
        {
            if (node.Parent == null)
                return node;

            return GetRoot(node.Parent);
        }

        protected bool Click(AccessibilityNodeInfo node)
        {
            if (node == null)
                return false;

            while (!node.Clickable)
                node = node.Parent;

            return node.PerformAction(Action.Click);
        }

        protected bool Back(AccessibilityNodeInfo root)
        {
            AccessibilityNodeInfo toolbar = root.FindAccessibilityNodeInfosByViewId(TOOLBAR_ID).FirstOrDefault();

            if (toolbar == null || toolbar.ChildCount < TOOLBAR_CHILDREN_MIN_COUNT)
                return false;

            AccessibilityNodeInfo backButton = toolbar.GetChild(TOOLBAR_BACK_BUTTON_NUM);

            return Click(backButton);
        }

        protected bool ConfirmMessageBox(AccessibilityNodeInfo root)
        {
            return HandleMessageBox(root, ACCEPT_MESSAGE_BOX_BUTTON);
        }

        protected bool RefuseMessageBox(AccessibilityNodeInfo root)
        {
            return HandleMessageBox(root, REFUSE_MESSAGE_BOX_BUTTON);
        }

        private bool HandleMessageBox(AccessibilityNodeInfo root, string clickId)
        {
            AccessibilityNodeInfo panel = GetButtonPanel(root);

            if (panel == null)
                return false;

            AccessibilityNodeInfo button = panel.FindAccessibilityNodeInfosByViewId(clickId).FirstOrDefault();

            return Click(button);
        }

        protected AccessibilityNodeInfo GetMessageBox(AccessibilityNodeInfo root)
        {
            return root.FindAccessibilityNodeInfosByViewId(MESSAGE_BOX_ID).FirstOrDefault();
        }

        protected AccessibilityNodeInfo GetButtonPanel(AccessibilityNodeInfo root)
        {
            return root.FindAccessibilityNodeInfosByViewId(BUTTON_PANEL_ID).FirstOrDefault();
        }

        protected async Task NotifyException(System.Exception e)
        {
            await Notifier.NotifyMessage(
                $"{BotService.CredentialsService.Credentials.TelegramUsername}, your bot is stuck due to:" +
                $"\n{e.Message}\n\n\n{e.StackTrace}"
            );
        }

        protected string GetAllText(AccessibilityNodeInfo node)
        {
            if (node == null) 
                return "{ }";

            StringBuilder log = new StringBuilder("{");

            log.Append($"\"id\" : \"{node.ViewIdResourceName}\", \"text\" : \"{node.Text}\", \"children\" : [");

            for (int i = 0; i < node.ChildCount - 1; ++i)
            {
                string child = GetAllText(node.GetChild(i));
                log.Append(child).Append(",");
            }

            if (node.ChildCount != 0)
            {
                string child = GetAllText(node.GetChild(node.ChildCount - 1));
                log.Append(child);
            }

            return log.Append("] }").ToString();
        }
    }
}