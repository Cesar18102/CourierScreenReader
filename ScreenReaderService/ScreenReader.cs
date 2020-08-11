using System.Threading.Tasks;
using System.Collections.Generic;

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
        private const int GESTURE_DELAY = 1000;

        public static IAccessabilityEventProcessor EventProcessor { get; set; }
        public static Queue<GestureDescription> GesturesToPerform { get; private set; } = new Queue<GestureDescription>();

        protected override void OnServiceConnected()
        {
            base.OnServiceConnected();
            StartWaitingForGestures();
        }

        public override void OnAccessibilityEvent(AccessibilityEvent e)
        {
            if (EventProcessor == null || !EventProcessor.IsValuableEvent(e))
                return;

            EventProcessor.ProcessEvent(e);
        }

        public override void OnInterrupt()
        {

        }

        public async void StartWaitingForGestures()
        {
            while(true)
            {
                await Task.Delay(GESTURE_DELAY);

                if(GesturesToPerform.Count != 0)
                    PerformGesture(GesturesToPerform.Dequeue());
            }
        }

        public void PerformGesture(GestureDescription gesture)
        {
            DispatchGesture(gesture, null, null);
        }
    }
}