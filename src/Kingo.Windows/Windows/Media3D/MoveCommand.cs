namespace Kingo.Windows.Media3D
{
    /// <summary>
    /// Serves as abase-class for all commands that somehow move the camera.
    /// </summary>
    /// <typeparam name="TParameter">Parameter-type of the command.</typeparam>
    public abstract class MoveCommand<TParameter> : CameraCommand<TParameter>
    {
        /// <summary>
        /// Determines whether or not the specified <paramref name="controller"/> is able to move the camera.
        /// </summary>
        /// <param name="parameter">Parameter of the command.</param>
        /// <param name="controller">A controller.</param>
        /// <returns>
        /// <c>true</c> if the specified <paramref name="controller"/> is able to move the camera; otherwise <c>false</c>.
        /// </returns>
        protected override bool CanExecuteCommand(TParameter parameter, IProjectionCameraController controller)
        {
            return controller.CanMove;
        }
    }
}
