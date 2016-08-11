using System.Windows;
using System.Windows.Input;

namespace Kingo.Windows.Media3D
{
    /// <summary>
    /// Represents a binding between a <see cref="UIElement.KeyDown" />-event and a command.
    /// </summary>
    public class KeyDownBinding : ControlModeInputBinding
    {
        #region [====== KeyGesture ======]

        /// <summary>
        /// Backing-field of the <see cref="KeyGesture"/>-property.
        /// </summary>
        public static readonly DependencyProperty KeyGestureProperty =
            DependencyProperty.Register(nameof(KeyGesture), typeof(KeyGesture), typeof(KeyDownBinding));

        /// <summary>
        /// Gets or sets the <see cref="KeyGesture" /> that triggers the command..
        /// </summary>
        public KeyGesture KeyGesture
        {
            get { return (KeyGesture) GetValue(KeyGestureProperty); }
            set { SetValue(KeyGestureProperty, value); }
        }

        #endregion

        #region [====== Activate & Deactivate ======]

        /// <summary>
        /// Attaches an event handler to the <paramref name="inputSource"/>'s <see cref="UIElement.KeyDown" />-event.
        /// </summary>
        /// <param name="inputSource">The source to attach to.</param>
        protected override void ActivateBinding(UIElement inputSource)
        {
            if (inputSource != null)
            {               
                inputSource.KeyDown += HandleKeyDown;
            }            
        }

        /// <summary>
        /// Detaches an event handler from the <paramref name="inputSource"/>'s <see cref="UIElement.KeyDown" />-event.
        /// </summary>
        /// <param name="inputSource">The source to detach from.</param>
        protected override void DeactivateBinding(UIElement inputSource)
        {
            if (inputSource != null)
            {
                inputSource.KeyDown -= HandleKeyDown;
            }
        }

        private void HandleKeyDown(object sender, KeyEventArgs e)
        {
            if (KeyGesture.Matches(sender, e))
            {
                e.Handled = true;

                OnCommandTriggerRaised();
            }
        }

        #endregion
    }
}
