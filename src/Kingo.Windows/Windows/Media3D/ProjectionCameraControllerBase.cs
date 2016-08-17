using System.ComponentModel;
using System.Windows.Media.Media3D;

namespace Kingo.Windows.Media3D
{
    /// <summary>
    /// Serves as a base-class for all <see cref="IProjectionCameraController"/>-implementations.
    /// </summary>
    public abstract class ProjectionCameraControllerBase : IProjectionCameraController
    {
        #region [====== Basic Properties ======]

        /// <inheritdoc />
        public abstract event PropertyChangedEventHandler PropertyChanged;

        /// <inheritdoc />
        public abstract Camera Camera
        {
            get;
            set;
        }

        #endregion

        #region [====== Translation ======]        

        /// <inheritdoc />
        /// <inheritdoc />
        public void MoveLeftRight(double distance)
        {
            Move(distance * Right);
        }

        /// <inheritdoc />
        public void MoveUpDown(double distance)
        {
            Move(distance * Up);
        }

        /// <inheritdoc />
        public void MoveForwardBackward(double distance)
        {
            Move(distance * Forward);
        }

        /// <inheritdoc />
        public abstract void Move(Vector3D direction);        

        #endregion

        #region [====== Rotation - Orientation ======]

        /// <inheritdoc />
        public abstract Vector3D Up
        {
            get;
        }

        /// <inheritdoc />
        public Vector3D Down => Negate(Up);

        /// <inheritdoc />
        public abstract Vector3D Left
        {
            get;
        }

        /// <inheritdoc />
        public Vector3D Right => Negate(Left);

        /// <inheritdoc />
        public abstract Vector3D Forward
        {
            get;
        }

        /// <inheritdoc />
        public Vector3D Backward => Negate(Forward);

        private static Vector3D Negate(Vector3D vector)
        {
            vector.Negate();
            return vector;
        }

        #endregion

        #region [====== Rotation - Yaw, Pitch & Roll ======]

        /// <inheritdoc />
        public void Yaw(Angle angle)
        {
            Yaw(angle.ToDegrees());
        }

        /// <inheritdoc />
        public abstract void Yaw(double angleInDegrees);        

        /// <inheritdoc />
        public void Pitch(Angle angle)
        {
            Pitch(angle.ToDegrees());
        }

        /// <inheritdoc />
        public abstract void Pitch(double angleInDegrees);        

        /// <inheritdoc />
        public void Roll(Angle angle)
        {
            Roll(angle.ToDegrees());
        }

        /// <inheritdoc />
        public abstract void Roll(double angleInDegrees);

        /// <inheritdoc />
        public void YawPitchRoll(Angle yaw, Angle pitch, Angle roll)
        {
            YawPitchRoll(yaw.ToDegrees(), pitch.ToDegrees(), roll.ToDegrees());
        }

        /// <inheritdoc />
        public abstract void YawPitchRoll(double yawInDegrees, double pitchInDegrees, double rollInDegrees);        

        #endregion

        #region [====== Rotation ======]

        /// <inheritdoc />
        public abstract Quaternion Rotation
        {
            get;
        }        

        /// <inheritdoc />
        public void Rotate(Vector3D axis, Angle angle)
        {
            Rotate(axis, angle.ToDegrees());
        }

        /// <inheritdoc />
        public void Rotate(Vector3D axis, double angleInDegrees)
        {
            Rotate(new AxisAngleRotation3D(axis, angleInDegrees));
        }

        /// <inheritdoc />
        public abstract void Rotate(AxisAngleRotation3D rotation);

        #endregion

        #region [====== Zooming ======]

        /// <inheritdoc />
        public abstract void Zoom(double zoomFactor);

        #endregion
    }
}
