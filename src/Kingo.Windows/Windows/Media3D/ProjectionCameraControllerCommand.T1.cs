using System;

namespace Kingo.Windows.Media3D
{
    /// <summary>
    /// When implemented by a derived class, represents a command that executes against a <see cref="IProjectionCameraController"/>.
    /// </summary>
    /// <typeparam name="TParameter">Parameter-type of this command.</typeparam>
    public abstract class ProjectionCameraControllerCommand<TParameter> : Command<TParameter>, IProjectionCameraControllerCommand
    {
        private IProjectionCameraController _controller;

        /// <inheritdoc />
        public IProjectionCameraController Controller
        {
            get { return _controller; }
            private set
            {
                throw new NotImplementedException();
            }
        }

        /// <inheritdoc />
        public void Attach(IProjectionCameraController controller)
        {
            
        }

        /// <inheritdoc />
        public void Detach()
        {
            
        }
    }
}
