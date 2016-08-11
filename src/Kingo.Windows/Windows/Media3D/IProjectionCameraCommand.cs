using System;
using System.Windows.Input;

namespace Kingo.Windows.Media3D
{
    /// <summary>
    /// When implemented by a derived class, represents a command that executes against a <see cref="IProjectionCameraController"/>.
    /// </summary>
    public interface IProjectionCameraCommand : ICommand
    {
        /// <summary>
        /// Adds the specified <paramref name="controller"/> to this command.
        /// </summary>
        /// <param name="controller">The controller to add.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="controller"/> is <c>null</c>.
        /// </exception>
        void Add(IProjectionCameraController controller);

        /// <summary>
        /// Removes the specified <paramref name="controller"/> from this command.
        /// </summary>
        /// <param name="controller">The controller to remove.</param>
        void Remove(IProjectionCameraController controller);
    }
}
