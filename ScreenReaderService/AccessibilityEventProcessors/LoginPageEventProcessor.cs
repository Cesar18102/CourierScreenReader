using System.Linq;

using Autofac;

using Android.Views.Accessibility;

namespace ScreenReaderService.AccessibilityEventProcessors
{
    public class LoginPageEventProcessor : AccessabilityEventProcessorBase
    {
        private const string PWD_INPUT_ID = "ua.ipost.work:id/passInput";
        private const int PWD_WRAPPER_NUM = 0;
        private const int PWD_NUM = 1;

        //PWD SERVICE

        public override bool IsValuableEvent(AccessibilityEvent e)
        {
            return e.Source != null;
        }

        public override void ProcessEvent(AccessibilityEvent e)
        {
            AccessibilityNodeInfo root = GetRoot(e.Source);
            AccessibilityNodeInfo pwdInput = root.FindAccessibilityNodeInfosByViewId(PWD_INPUT_ID).FirstOrDefault();

            if (pwdInput == null)
                return;

            AccessibilityNodeInfo pwdWrapper = pwdInput.GetChild(PWD_WRAPPER_NUM);

            if (pwdWrapper == null) 
                return;

            AccessibilityNodeInfo pwd = pwdWrapper.GetChild(PWD_NUM);

            if (pwd == null || string.IsNullOrEmpty(pwd.Text))
                return;

            //UPDATE PWD AT SERVER

            ScreenReader.EventProcessor = DependencyHolder.Dependencies.Resolve<OrderListPageEventProcessor>();
        }
    }
}