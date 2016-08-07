using System;
using System.Windows.Input;

namespace Kingo.Windows.Media3D
{
    /// <summary>
    /// When implemented by a derived class, represents a command that executes against a <see cref="IProjectionCameraController"/>.
    /// </summary>
    public interface IProjectionCameraControllerCommand : ICommand
    {
        /// <summary>
        /// Gets the controller this command is executed against.
        /// </summary>
        IProjectionCameraController Controller
        {
            get;
        }

        /// <summary>
        /// Activates this command by attaching it to the specified <paramref name="controller"/>.
        /// </summary>
        /// <param name="controller">The controller to attach.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="controller"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// This command is already attached to a controller.
        /// </exception>
        void Attach(IProjectionCameraController controller);

        /// <summary>
        /// Deactivates this command by detaching it from the currently attached controller.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// This command is not attached to a controller.
        /// </exception>
        void Detach();
    }
}
