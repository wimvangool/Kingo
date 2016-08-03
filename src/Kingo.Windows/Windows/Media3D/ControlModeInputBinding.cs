using System.Windows;

namespace Kingo.Windows.Media3D
{
    /// <summary>
    /// Represents a binding between a certain input like a Keyboard- or Mouse-event and a <see cref="ProjectionCameraController" />-command.
    /// </summary>
    public abstract class ControlModeInputBinding : DependencyObject
    {        
        #region [====== Activate & Deactivate ======]

        /// <summary>
        /// Gets the <see cref="ControlMode" /> this input-binding is part of.
        /// </summary>
        protected ControlMode ControlMode
        {
            get;
            private set;
        }

        internal void Activate(ControlMode controlMode)
        {
            ControlMode = controlMode;

            ActivateBinding();
        }

        /// <summary>
        /// Activates the input binding.
        /// </summary>
        protected abstract void ActivateBinding();

        internal void Deactivate()
        {
            DeactivateBinding();

            ControlMode = null;
        }

        /// <summary>
        /// Deactivates the input binding.
        /// </summary>
        protected abstract void DeactivateBinding();

        #endregion
    }
}
