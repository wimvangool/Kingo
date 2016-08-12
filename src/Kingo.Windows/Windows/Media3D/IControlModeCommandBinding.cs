using System;

namespace Kingo.Windows.Media3D
{
    /// <summary>
    /// When implemented by a class, represents a binding between a certain trigger and a command executed against a <see cref="IProjectionCameraController"/>.
    /// </summary>
    public interface IControlModeCommandBinding
    {
        /// <summary>
        /// Gets the <see cref="ControlMode" /> this binding belongs to when it has been activated.
        /// </summary>
        ControlMode ControlMode
        {
            get;
        }

        /// <summary>
        /// Activates the binding for the specified <paramref name="controlMode"/>.
        /// </summary>
        /// <param name="controlMode">The control mode this binding is a part of.</param>
        /// <exception cref="InvalidOperationException">
        /// This binding is aleady active.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="controlMode"/> is <c>null</c>.
        /// </exception>
        void Activate(ControlMode controlMode);

        /// <summary>
        /// Deactivates this binding.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// This binding is aleady inactive.
        /// </exception>
        void Deactivate();
    }
}
