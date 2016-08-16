using System;

namespace Kingo.Windows.Media3D
{
    /// <summary>
    /// Represents a <see cref="IProjectionCameraController" />-decorator that prints all camera-movements to the debug-console.
    /// </summary>
    public class DebugCameraController : ProjectCameraControllerDecorator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DebugCameraController" /> class.
        /// </summary>
        /// <param name="controller">The controller to decorate</param>.
        /// <exception cref="ArgumentNullException">
        /// <paramref name="controller"/> is <c>null</c>.
        /// </exception>
        public DebugCameraController(IProjectionCameraController controller)
        {
            if (controller == null)
            {
                throw new ArgumentNullException(nameof(controller));
            }
            Controller = controller;
        }

        /// <inheritdoc />
        protected override IProjectionCameraController Controller
        {
            get;
        }        
    }
}
