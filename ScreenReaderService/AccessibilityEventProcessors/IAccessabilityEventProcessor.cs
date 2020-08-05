using System;

using Android.Views.Accessibility;

namespace ScreenReaderService.AccessibilityEventProcessors
{
    public interface IAccessabilityEventProcessor
    {
        void ProcessEvent(AccessibilityEvent e);
        bool IsValuableEvent(AccessibilityEvent e);
    }
}