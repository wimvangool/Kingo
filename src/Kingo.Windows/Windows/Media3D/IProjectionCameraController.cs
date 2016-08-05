using System;
using System.ComponentModel;
using System.Windows.Media.Media3D;

namespace Kingo.Windows.Media3D
{
    /// <summary>
    /// When implemented by a class, represents a controller for <see cref="ProjectionCamera">Projection Cameras</see> that is used
    /// to translate and rotate a camera.
    /// </summary>
    public interface IProjectionCameraController : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets or sets the camera that is controlled by this controller.
        /// </summary>
        ProjectionCamera Camera
        {
            get;
            set;
        }

        #region [====== Translation ======]        

        /// <summary>
        /// Moves the camera along its horizontal axis (represented by <see cref="Left"/> and <see cref="Right"/>).
        /// </summary>
        /// <param name="distance">The distance to move.</param>
        /// <exception cref="InvalidOperationException">
        /// No camera has been attached to the controller.
        /// </exception>
        void MoveLeftRight(double distance);

        /// <summary>
        /// Moves the camera along its vertical axis (represented by <see cref="Up"/> and <see cref="Down"/>).
        /// </summary>
        /// <param name="distance">The distance to move.</param>
        /// <exception cref="InvalidOperationException">
        /// No camera has been attached to the controller.
        /// </exception>
        void MoveUpDown(double distance);

        /// <summary>
        /// Moves the camera forwards or backwards (represented by <see cref="Forward"/> and <see cref="Backward"/>).
        /// </summary>
        /// <param name="distance">The distance to move.</param>
        /// <exception cref="InvalidOperationException">
        /// No camera has been attached to the controller.
        /// </exception>
        void MoveForwardBackward(double distance);

        /// <summary>
        /// Moves the camera in the specified <paramref name="direction"/>.
        /// </summary>
        /// <param name="direction">The direction to move in.</param>
        /// <exception cref="InvalidOperationException">
        /// No camera has been attached to the controller.
        /// </exception>
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
        /// <exception cref="InvalidOperationException">
        /// No camera has been attached to the controller, or the associated camera could not be rotated.
        /// </exception>
        void Pitch(Angle angle);

        /// <summary>
        /// Rotates the camera around its <see cref="Up"/> (or <see cref="Down"/>) vector in clockwise direction.
        /// </summary>
        /// <param name="angle">The rotation angle.</param>
        /// <exception cref="InvalidOperationException">
        /// No camera has been attached to the controller, or the associated camera could not be rotated.
        /// </exception>
        void Yaw(Angle angle);

        /// <summary>
        /// Rotates the camera around its <see cref="Forward"/> (or <see cref="Backward"/>) vector in clockwise direction.
        /// </summary>
        /// <param name="angle">The rotation angle.</param>
        /// <exception cref="InvalidOperationException">
        /// No camera has been attached to the controller, or the associated camera could not be rotated.
        /// </exception>
        void Roll(Angle angle);

        /// <summary>
        /// Peforms the specified rotations in one single operation.
        /// </summary>
        /// <param name="pitch">The pitch-angle.</param>
        /// <param name="yaw">The yaw-angle.</param>
        /// <param name="roll">The roll-angle.</param>
        /// <exception cref="InvalidOperationException">
        /// No camera has been attached to the controller, or the associated camera could not be rotated.
        /// </exception>
        void PitchYawRoll(Angle pitch, Angle yaw, Angle roll);

        /// <summary>
        /// Rotates the camera in clockwise direction around the specified axis.
        /// </summary>
        /// <param name="angle">The rotation-angle.</param>
        /// <param name="axis">The axis around which to rotate.</param>
        /// <exception cref="InvalidOperationException">
        /// No camera has been attached to the controller, or the associated camera could not be rotated.
        /// </exception>
        void Rotate(Angle angle, Vector3D axis);

        #endregion
    }
}
