namespace Kingo.Windows.Media3D
{
    /// <summary>
    /// Represents a command that moves a camera to the left or to the right with respect to its current lookdirection.
    /// </summary>
    public class MoveLeftRightCommand : MoveCommand<double>
    {
        /// <summary>
        /// Instructs the specified <paramref name="controller"/> to move the camera to the left or to the right.
        /// </summary>
        /// <param name="parameter">Distance and direction to move.</param>
        /// <param name="controller">A controller.</param>
        protected override void ExecuteCommand(double parameter, IProjectionCameraController controller)
        {
            controller.MoveLeftRight(parameter);
        }
    }
}
