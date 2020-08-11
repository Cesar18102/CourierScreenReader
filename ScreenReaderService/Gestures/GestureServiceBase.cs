using Android.AccessibilityServices;

namespace ScreenReaderService.Gestures
{
    public abstract class GestureServiceBase
    {
        private GestureDescription gesture = null;
        public GestureDescription Gesture
        {
            get
            {
                if (gesture == null)
                    gesture = BuildGesture();
                return gesture;
            }
        }

        protected abstract GestureDescription BuildGesture();
    }
}