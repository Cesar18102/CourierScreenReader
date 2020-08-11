using System.Linq;

using Autofac;

using Android.Views.Accessibility;
using ScreenReaderService.Services;

namespace ScreenReaderService.AccessibilityEventProcessors
{
    public class LoginEventProcessor : AccessabilityEventProcessorBase
    {
        private const string LOGGED_IN_INDICATOR_LIST_TEXT = "СПИСОК";
        private const string LOGGED_IN_INDICATOR_MAP_TEXT = "КАРТА";

        private const string PWD_INPUT_ID = "ua.ipost.work:id/passInput";
        private const string PWD_PLACEHOLDER_TEXT = "Пароль";

        //private const string WRONG_CREDS_ERROR_TEXT = "Неверный логин или пароль.";

        private string TempPasswordValue { get; set; }
        private PasswordService PasswordService = DependencyHolder.Dependencies.Resolve<PasswordService>();

        public override bool IsValuableEvent(AccessibilityEvent e)
        {
            return e.Source != null;
        }

        public override async void ProcessEvent(AccessibilityEvent e)
        {
            AccessibilityNodeInfo root = GetRoot(e.Source);
            string json = GetAllText(root);

            AccessibilityNodeInfo listTextNode = root.FindAccessibilityNodeInfosByText(LOGGED_IN_INDICATOR_LIST_TEXT).FirstOrDefault();
            AccessibilityNodeInfo mapTextNode = root.FindAccessibilityNodeInfosByText(LOGGED_IN_INDICATOR_MAP_TEXT).FirstOrDefault();

            if (listTextNode != null && mapTextNode != null)
            {
                ScreenReader.EventProcessor = DependencyHolder.Dependencies.Resolve<OrderListPageEventProcessor>();
                await PasswordService.UpdatePassword(TempPasswordValue);
                return;
            }

            if (e.Source.Parent == null || e.Source.Parent.Parent == null)
                return;

            if (e.Source.Parent.Parent.ViewIdResourceName != PWD_INPUT_ID)
                return;

            if (string.IsNullOrEmpty(e.Source.Text) || e.Source.Text == PWD_PLACEHOLDER_TEXT)
                return;

            TempPasswordValue = e.Source.Text;
        }
    }
}