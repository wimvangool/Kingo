using System;
using System.Windows;

namespace Kingo.Windows
{
    internal sealed class FocusManagerSpy : IFocusManager
    {
        #region [====== Watcher =====]

        private sealed class Watcher : FocusWatcher
        {
            private readonly FocusManagerSpy _focusManager;
            private readonly UIElement _element;            

            public Watcher(FocusManagerSpy focusManager, UIElement element)
            {
                _focusManager = focusManager;
                _focusManager.FocusedElementChanged += HandleFocusedElementChanged;

                _element = element;
            }

            protected override void DisposeManagedResources()
            {
                base.DisposeManagedResources();

                _focusManager.FocusedElementChanged -= HandleFocusedElementChanged;
            }

            private void HandleFocusedElementChanged(object sender, PropertyChangedEventArgs<UIElement> e)
            {
                if (ReferenceEquals(_element, e.OldValue))
                {
                    OnLostFocus();
                }
                else if (ReferenceEquals(_element, e.NewValue))
                {
                    OnGotFocus();
                }
            }
        }

        #endregion

        #region [====== FocusedElement ======]

        private UIElement _focusedElement;

        public event EventHandler<PropertyChangedEventArgs<UIElement>> FocusedElementChanged;

        private void OnFocusedElementChanged(UIElement oldValue, UIElement newValue)
        {
            FocusedElementChanged.Raise(this, new PropertyChangedEventArgs<UIElement>(oldValue, newValue));
        }

        public UIElement FocusedElement
        {
            get { return _focusedElement; }
            set
            {
                var oldValue = _focusedElement;
                var newValue = value;

                if (newValue != oldValue)
                {
                    _focusedElement = newValue;

                    OnFocusedElementChanged(oldValue, newValue);
                }
            }
        }

        #endregion

        public bool HasFocus(UIElement element)
        {
            return ReferenceEquals(FocusedElement, element);
        }

        public void Focus(UIElement element)
        {
            FocusedElement = element;
        }        

        public FocusWatcher CreateFocusWatcher(UIElement element)
        {
            return new Watcher(this, element);
        }
    }
}
