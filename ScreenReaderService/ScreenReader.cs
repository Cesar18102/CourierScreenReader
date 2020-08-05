using System.Threading.Tasks;

using Android.App;
using Android.Content;
using Android.Graphics;
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
        private const int UPDATE_DELAY = 1000;
        private const int UPDATE_GESTURE_DURATION = 500;

        public static bool UpdateNeeded { get; set; }
        public static IAccessabilityEventProcessor EventProcessor { get; set; }

        protected override void OnServiceConnected()
        {
            base.OnServiceConnected();
            //StartUpdating();
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


        public async void StartUpdating()
        {
            while(true)
            {
                await Task.Delay(UPDATE_DELAY);

                if(UpdateNeeded)
                {
                    Update();
                    UpdateNeeded = false;
                }
            }
        }

        public void Update()
        {
            float x = Resources.DisplayMetrics.WidthPixels / 2;
            float yStart = Resources.DisplayMetrics.HeightPixels / 3;
            float yEnd = Resources.DisplayMetrics.HeightPixels * 2 / 3;

            Path path = new Path();

            path.MoveTo(x, yStart);
            path.LineTo(x, yEnd);

            GestureDescription.StrokeDescription stroke = new GestureDescription.StrokeDescription(
                path, 0, UPDATE_GESTURE_DURATION
            );

            GestureDescription.Builder gestureBuilder = new GestureDescription.Builder();
            gestureBuilder.AddStroke(stroke);

            DispatchGesture(gestureBuilder.Build(), null, null);
        }
    }
}