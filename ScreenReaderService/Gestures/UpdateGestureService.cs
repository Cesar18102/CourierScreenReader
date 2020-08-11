using Android.Graphics;
using Android.AccessibilityServices;

using Xamarin.Essentials;

namespace ScreenReaderService.Gestures
{
    public class UpdateGestureService : GestureServiceBase
    {
        private const int UPDATE_GESTURE_DURATION = 100;

        protected override GestureDescription BuildGesture()
        {
            float x = (float)DeviceDisplay.MainDisplayInfo.Width / 2;
            float yStart = (float)DeviceDisplay.MainDisplayInfo.Height / 3;
            float yEnd = (float)DeviceDisplay.MainDisplayInfo.Height * 2 / 3;

            Path path = new Path();

            path.MoveTo(x, yStart);
            path.LineTo(x, yEnd);

            GestureDescription.StrokeDescription stroke = new GestureDescription.StrokeDescription(
                path, 0, UPDATE_GESTURE_DURATION
            );

            GestureDescription.Builder gestureBuilder = new GestureDescription.Builder();
            gestureBuilder.AddStroke(stroke);

            return gestureBuilder.Build();
        }
    }
}