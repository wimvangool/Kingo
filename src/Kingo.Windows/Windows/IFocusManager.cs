using System.Windows;

namespace Kingo.Windows
{
    internal interface IFocusManager
    {
        bool HasFocus(UIElement element);

        void Focus(UIElement element);

        FocusWatcher CreateFocusWatcher(UIElement element);
    }
}
