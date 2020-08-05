using Android.App;
using Android.Content;
using Android.Views.Accessibility;
using Android.AccessibilityServices;

using ScreenReaderService.AccessibilityEventProcessors;

namespace ScreenReaderService
{
    [Service(Label = "ScreenReaderService", Permission = "android.permission.BIND_ACCESSIBILITY_SERVICE")]
    [IntentFilter(new[] { "android.accessibilityservice.AccessibilityService" })]
    [MetaData("android.accessibilityservice", Resource = "@xml/serviceconfig")]
    public class ScreenReader : AccessibilityService
    {
        public static IAccessabilityEventProcessor EventProcessor { get; set; }

        public override void OnAccessibilityEvent(AccessibilityEvent e)
        {
            if (!EventProcessor.IsValuableEvent(e))
                return;

            EventProcessor.ProcessEvent(e);
        }

        public override void OnInterrupt()
        {

        }
    }
}