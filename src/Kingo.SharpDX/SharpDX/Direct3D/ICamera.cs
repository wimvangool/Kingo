using SharpDX;

namespace Kingo.SharpDX.Direct3D
{
    /// <summary>
    /// When implemented by a class, represents a camera within a 3D coordinate system that is used to observe the virtual world.
    /// </summary>
    public interface ICamera : IMoveableObject, IRotatableObject
    {
        #region [====== LookAt ======]

        /// <summary>
        /// Rotates the camera to focuses on the specified target point.
        /// </summary>
        /// <param name="target">The point or position to focus on.</param>        
        void LookAt(Vector3 target);

        #endregion

        #region [====== Panning ======]

        /// <summary>
        /// Slides or shifts the camera to the left or right while maintaining its current orientation/rotation.
        /// </summary>
        /// <param name="distance">The distance to move.</param>
        void PanHorizontal(float distance);

        /// <summary>
        /// Slides or shifts the camera to the up or down while maintaining its current orientation/rotation.
        /// </summary>
        /// <param name="distance">The distance to move.</param>
        void PanVertical(float distance);

        #endregion           
    }
}
