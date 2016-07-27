using System.Windows.Media.Media3D;

namespace Kingo.Wpf
{
    /// <summary>
    /// When implemented by a class, represents a controller for <see cref="ProjectionCamera">Projection Cameras</see> that is used
    /// to translate and rotate a camera.
    /// </summary>
    public interface IProjectionCameraController
    {        
        #region [====== Translation ======]

        /// <summary>
        /// The current position of the camera.
        /// </summary>
        Point3D Position
        {
            get;
        }

        /// <summary>
        /// Moves the camera along its horizontal axis (represented by <see cref="Left"/> and <see cref="Right"/>).
        /// </summary>
        /// <param name="distance">The distance to move.</param>
        void MoveHorizontal(double distance);

        /// <summary>
        /// Moves the camera along its vertical axis (represented by <see cref="Up"/> and <see cref="Down"/>).
        /// </summary>
        /// <param name="distance">The distance to move.</param>
        void MoveVertical(double distance);

        /// <summary>
        /// Moves the camera forwards or backwards (represented by <see cref="Forward"/> and <see cref="Backward"/>).
        /// </summary>
        /// <param name="distance">The distance to move.</param>
        void Move(double distance);

        /// <summary>
        /// Moves the camera in the specified <paramref name="direction"/>.
        /// </summary>
        /// <param name="direction">The direction to move in.</param>
        void Move(Vector3D direction);

        #endregion

        #region [====== Rotation ======]     

        /// <summary>
        /// Returns the normalized Up-vector of the camera.
        /// </summary>
        Vector3D Up
        {
            get;
        }

        /// <summary>
        /// Returns the normalized Down-vector of the camera.
        /// </summary>
        Vector3D Down
        {
            get;
        }

        /// <summary>
        /// Returns the normalized Left-vector of the camera.
        /// </summary>
        Vector3D Left
        {
            get;
        }

        /// <summary>
        /// Returns the normalized Right-vector of the camera.
        /// </summary>
        Vector3D Right
        {
            get;
        }

        /// <summary>
        /// Returns the normalized Forward-vector of the camera.
        /// </summary>
        Vector3D Forward
        {
            get;
        }

        /// <summary>
        /// Returns the normalized Backward-vector of the camera.
        /// </summary>
        Vector3D Backward
        {
            get;
        }

        /// <summary>
        /// Rotates the camera around its <see cref="Left"/> (or <see cref="Right"/>) vector in clockwise direction.
        /// </summary>
        /// <param name="angle">The rotation angle.</param>
        void Pitch(Angle angle);

        /// <summary>
        /// Rotates the camera around its <see cref="Up"/> (or <see cref="Down"/>) vector in clockwise direction.
        /// </summary>
        /// <param name="angle">The rotation angle.</param>
        void Yaw(Angle angle);

        /// <summary>
        /// Rotates the camera around its <see cref="Forward"/> (or <see cref="Backward"/>) vector in clockwise direction.
        /// </summary>
        /// <param name="angle">The rotation angle.</param>
        void Roll(Angle angle);

        /// <summary>
        /// Peforms the specified rotations in one single operation.
        /// </summary>
        /// <param name="pitch">The pitch-angle.</param>
        /// <param name="yaw">The yaw-angle.</param>
        /// <param name="roll">The roll-angle.</param>
        void PitchYawRoll(Angle pitch, Angle yaw, Angle roll);

        /// <summary>
        /// Rotates the camera in clockwise direction around the specified axis.
        /// </summary>
        /// <param name="angle">The rotation-angle.</param>
        /// <param name="axis">The axis around which to rotate.</param>
        void Rotate(Angle angle, Vector3D axis);

        #endregion
    }
}
