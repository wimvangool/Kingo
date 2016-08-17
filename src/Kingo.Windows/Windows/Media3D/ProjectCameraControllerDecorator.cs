using System.ComponentModel;
using System.Windows.Media.Media3D;

namespace Kingo.Windows.Media3D
{
    /// <summary>
    /// When implemented by a derived class, represents a decorator of the a <see cref="IProjectionCameraController"/>.
    /// </summary>
    public abstract class ProjectCameraControllerDecorator : ProjectionCameraControllerBase
    {
        #region [====== Basic Properties ======]

        /// <summary>
        /// Gets the decorated controller.
        /// </summary>
        protected abstract IProjectionCameraController Controller
        {
            get;
        }

        /// <inheritdoc />
        public override event PropertyChangedEventHandler PropertyChanged
        {
            add { Controller.PropertyChanged += value; }
            remove { Controller.PropertyChanged -= value; }
        }


        /// <inheritdoc />
        public override Camera Camera
        {
            get { return Controller.Camera; }
            set { Controller.Camera = value; }
        }

        #endregion

        #region [====== Translation ======]        

        /// <inheritdoc />
        public override void Move(Vector3D direction)
        {
            Controller.Move(direction);
        }

        #endregion

        #region [====== Rotation - Orientation ======]

        /// <inheritdoc />
        public override Vector3D Up => Controller.Up;

        /// <inheritdoc />
        public override Vector3D Left => Controller.Left;

        /// <inheritdoc />
        public override Vector3D Forward => Controller.Forward;

        #endregion

        #region [====== Rotation - Yaw, Pitch & Roll ======]

        /// <inheritdoc />
        public override void Yaw(double angleInDegrees)
        {
            Controller.Yaw(angleInDegrees);
        }

        /// <inheritdoc />
        public override void Pitch(double angleInDegrees)
        {
            Controller.Pitch(angleInDegrees);
        }

        /// <inheritdoc />
        public override void Roll(double angleInDegrees)
        {
            Controller.Roll(angleInDegrees);
        }

        /// <inheritdoc />
        public override void YawPitchRoll(double yawInDegrees, double pitchInDegrees, double rollInDegrees)
        {
            Controller.YawPitchRoll(yawInDegrees, pitchInDegrees, rollInDegrees);
        }

        #endregion

        #region [====== Rotation ======]

        /// <inheritdoc />
        public override Quaternion Rotation => Controller.Rotation;        

        /// <inheritdoc />
        public override void Rotate(AxisAngleRotation3D rotation)
        {
            Controller.Rotate(rotation);
        }

        #endregion

        #region [====== Zooming ======]

        /// <inheritdoc />
        public override void Zoom(double zoomFactor)
        {
            Controller.Zoom(zoomFactor);
        }

        #endregion
    }
}
