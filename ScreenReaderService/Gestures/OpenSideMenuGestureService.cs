using Android.Graphics;
using Android.AccessibilityServices;

using Xamarin.Essentials;

namespace ScreenReaderService.Gestures
{
    public class OpenSideMenuGestureService : GestureServiceBase
    {
        private const int OPEN_GESTURE_DURATION = 500;
        protected override GestureDescription BuildGesture()
        {
            float xStart = 0;
            float xEnd = (float)DeviceDisplay.MainDisplayInfo.Width / 2;
            float y = (float)DeviceDisplay.MainDisplayInfo.Height / 2;

            Path path = new Path();

            path.MoveTo(xStart, y);
            path.LineTo(xEnd, y);

            GestureDescription.StrokeDescription stroke = new GestureDescription.StrokeDescription(
                path, 0, OPEN_GESTURE_DURATION
            );

            GestureDescription.Builder gestureBuilder = new GestureDescription.Builder();
            gestureBuilder.AddStroke(stroke);

            return gestureBuilder.Build();
        }
    }
}