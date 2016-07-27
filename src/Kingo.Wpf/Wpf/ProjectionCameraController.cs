using System.Windows;
using System.Windows.Media.Media3D;

namespace Kingo.Wpf
{
    /// <summary>
    /// Represents a controller for <see cref="ProjectionCamera">ProjectionCameras</see> to translate and rotate it.
    /// </summary>
    public class ProjectionCameraController : DependencyObject, IProjectionCameraController
    {
        private static readonly Vector3D _DefaultLookDirection = new Vector3D(0, 0, -1);
        private static readonly Vector3D _DefaultUpDirection = new Vector3D(0, 1, 0);
        private Quaternion _rotation;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectionCameraController" /> class.
        /// </summary>
        public ProjectionCameraController()
        {
            _rotation = Quaternion.Identity;
        }

        #region [====== Position ======]

        /// <summary>
        /// Backing-field of the <see cref="Position"/>-property.
        /// </summary>
        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.Register(nameof(Position), typeof(Point3D), typeof(ProjectionCameraController));

        /// <summary>
        /// Gets or sets the position of the camera.
        /// </summary>
        public Point3D Position
        {
            get { return (Point3D) GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
        }

        #endregion

        #region [====== LookDirection ======]

        /// <summary>
        /// Backing-field of the <see cref="LookDirection"/>-property.
        /// </summary>
        public static readonly DependencyProperty LookDirectionProperty =
            DependencyProperty.Register(nameof(LookDirection), typeof(Vector3D), typeof(ProjectionCameraController), new FrameworkPropertyMetadata(_DefaultLookDirection));

        /// <summary>
        /// Gets the current look-direction of the camera.
        /// </summary>
        public Vector3D LookDirection
        {
            get { return (Vector3D) GetValue(LookDirectionProperty); }
            private set { SetValue(LookDirectionProperty, value); }
        }

        #endregion

        #region [====== UpDirection ======]

        /// <summary>
        /// Backing-field of the <see cref="UpDirection"/>-property.
        /// </summary>
        public static readonly DependencyProperty UpDirectionProperty =
            DependencyProperty.Register(nameof(UpDirection), typeof(Vector3D), typeof(ProjectionCameraController), new FrameworkPropertyMetadata(_DefaultUpDirection));

        /// <summary>
        /// Gets the current up-direction of the camera.
        /// </summary>
        public Vector3D UpDirection
        {
            get { return (Vector3D) GetValue(UpDirectionProperty); }
            private set { SetValue(UpDirectionProperty, value); }
        }

        #endregion        

        #region [====== Translation ======]

        /// <inheritdoc />
        public void MoveHorizontal(double distance)
        {
            Move(distance * Right);
        }

        /// <inheritdoc />
        public void MoveVertical(double distance)
        {
            Move(distance * Up);
        }

        /// <inheritdoc />
        public void Move(double distance)
        {
            Move(distance * Forward);
        }

        /// <inheritdoc />
        public void Move(Vector3D direction)
        {
            Position = Position + direction;            
        }

        #endregion

        #region [====== Rotation ======]

        /// <inheritdoc />
        public Vector3D Up => Normalize(UpDirection);

        /// <inheritdoc />
        public Vector3D Down => Negate(Up);

        /// <inheritdoc />
        public Vector3D Left => Normalize(Vector3D.CrossProduct(Up, Forward));

        /// <inheritdoc />
        public Vector3D Right => Negate(Left);

        /// <inheritdoc />
        public Vector3D Forward => Normalize(LookDirection);

        /// <inheritdoc />
        public Vector3D Backward => Negate(Forward);        

        private Quaternion Rotation
        {
            get { return _rotation; }
            set
            {
                var oldValue = _rotation;
                var newValue = value;

                if (oldValue != newValue)
                {
                    _rotation = newValue;

                    OnRotationChanged();
                }
            }
        }

        private void OnRotationChanged()
        {
            var rotation = new QuaternionRotation3D(_rotation);
            var rotationTransformation = new RotateTransform3D(rotation);

            LookDirection = rotationTransformation.Transform(_DefaultLookDirection);
            UpDirection = rotationTransformation.Transform(_DefaultUpDirection);
        }

        /// <inheritdoc />
        public void Pitch(Angle angle)
        {
            Rotate(angle, Left);
        }

        /// <inheritdoc />
        public void Yaw(Angle angle)
        {
            Rotate(angle, Up);
        }

        /// <inheritdoc />
        public void Roll(Angle angle)
        {
            Rotate(angle, Forward);
        }

        /// <inheritdoc />
        public void PitchYawRoll(Angle pitch, Angle yaw, Angle roll)
        {
            var left = Left;
            var up = Up;
            var forward = Forward;

            Rotate(pitch, left);
            Rotate(yaw, up);
            Rotate(roll, forward);
        }

        /// <inheritdoc />
        public void Rotate(Angle angle, Vector3D axis)
        {
            Rotation = Quaternion.Multiply(Rotation, new Quaternion(axis, angle.ToDegrees()));
        }

        private static Vector3D Normalize(Vector3D vector)
        {
            vector.Normalize();
            return vector;
        }

        private static Vector3D Negate(Vector3D vector)
        {
            vector.Negate();
            return vector;
        }

        #endregion
    }
}
