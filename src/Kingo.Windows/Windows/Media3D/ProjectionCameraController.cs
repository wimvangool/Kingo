using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media.Media3D;
using Kingo.Resources;
using static Kingo.Windows.Media3D.EulerAngleRotationSet;

namespace Kingo.Windows.Media3D
{
    /// <summary>
    /// Represents a controller for <see cref="ProjectionCamera">ProjectionCameras</see> to translate and rotate it.
    /// </summary>
    public class ProjectionCameraController : DependencyObject, IProjectionCameraController
    {                        
        #region [====== NotifyPropertyChanged ======]

        /// <inheritdoc />
        public event PropertyChangedEventHandler PropertyChanged;
        
        /// <summary>
        /// Raised the <see cref="PropertyChanged"/> event for the specified <paramref name="propertyName"/>.
        /// </summary>
        /// <param name="propertyName">Name of the property that changed.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region [====== Camera ======]

        /// <summary>
        /// Backing-field of the <see cref="Camera"/>-property.
        /// </summary>
        public static readonly DependencyProperty CameraProperty =
            DependencyProperty.Register(nameof(Camera), typeof(ProjectionCamera), typeof(ProjectionCameraController), new FrameworkPropertyMetadata(HandleCameraChanged));

        /// <summary>
        /// Gets or sets the camera that is controlled by this controller.
        /// </summary>
        public ProjectionCamera Camera
        {
            get { return (ProjectionCamera) GetValue(CameraProperty); }
            set { SetValue(CameraProperty, value); }
        }

        private static void HandleCameraChanged(DependencyObject instance, DependencyPropertyChangedEventArgs e)
        {
            var controller = instance as ProjectionCameraController;
            if (controller != null)
            {
                controller._currentRotation = null;
                controller.OnPropertyChanged(null);
            }
        }

        private static Exception NewNoCameraSetException()
        {
            return new InvalidOperationException(ExceptionMessages.ProjectionCameraController_NoCameraSet);
        }

        #endregion     

        #region [====== Translation ======]

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
        public void Move(Vector3D direction)
        {
            var camera = Camera;
            if (camera != null)
            {
                camera.Position += direction;
                return;                
            }
            throw NewNoCameraSetException();
        }

        #endregion

        #region [====== Rotation - Orientation ======]

        private static readonly Vector3D _DefaultLookDirection = new Vector3D(0, 0, -1);
        private static readonly Vector3D _DefaultUpDirection = new Vector3D(0, 1, 0);
        private static readonly Vector3D _DefaultLeftDirection = new Vector3D(-1, 0, 0);
        private static readonly Vector3D _DefaultRightDirection = new Vector3D(1, 0, 0);        

        /// <inheritdoc />
        public Vector3D Up => Normalize(Vector3D.CrossProduct(Forward, Left));

        /// <inheritdoc />
        public Vector3D Down => Negate(Up);

        /// <inheritdoc />
        public Vector3D Left
        {
            get
            {
                var camera = Camera;
                if (camera == null)
                {
                    return _DefaultLeftDirection;
                }
                var left = Vector3D.CrossProduct(camera.UpDirection, camera.LookDirection);

                if (IsAlmost(0, left.Length))
                {
                    left = Vector3D.CrossProduct(_DefaultUpDirection, camera.LookDirection);
                }
                return Normalize(left);
            }
        }

        /// <inheritdoc />
        public Vector3D Right => Negate(Left);

        /// <inheritdoc />
        public Vector3D Forward
        {
            get
            {
                var camera = Camera;
                if (camera == null)
                {
                    return _DefaultLookDirection;
                }
                return Normalize(camera.LookDirection);
            } 
        }

        /// <inheritdoc />
        public Vector3D Backward => Negate(Forward);

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

        #region [====== Rotation - Yaw ======]

        private Vector3D YawAxis => Up;

        /// <inheritdoc />
        public void Yaw(Angle angle)
        {
            Yaw(angle.ToDegrees());
        }

        /// <inheritdoc />
        public void Yaw(double angleInDegrees)
        {
            Rotate(YawAxis, angleInDegrees);
        }

        #endregion

        #region [====== Rotation - Pitch ======]

        private Vector3D PitchAxis => Right;

        /// <inheritdoc />
        public void Pitch(Angle angle)
        {
            Pitch(angle.ToDegrees());
        }

        /// <inheritdoc />
        public void Pitch(double angleInDegrees)
        {
            Rotate(PitchAxis, angleInDegrees);
        }

        #endregion

        #region [====== Rotation - Roll ======]

        private Vector3D RollAxis => Forward;

        /// <inheritdoc />
        public void Roll(Angle angle)
        {
            Roll(angle.ToDegrees());
        }

        /// <inheritdoc />
        public void Roll(double angleInDegrees)
        {
            Rotate(RollAxis, angleInDegrees);
        }

        #endregion

        #region [====== Rotation - YawPitchRoll ======]

        /// <inheritdoc />
        public void YawPitchRoll(Angle yaw, Angle pitch, Angle roll)
        {
            YawPitchRoll(yaw.ToDegrees(), pitch.ToDegrees(), roll.ToDegrees());
        }

        /// <inheritdoc />
        public void YawPitchRoll(double yawInDegrees, double pitchInDegrees, double rollInDegrees)
        {
            YawPitchRoll(
                new AxisAngleRotation3D(YawAxis, yawInDegrees),
                new AxisAngleRotation3D(PitchAxis, pitchInDegrees),
                new AxisAngleRotation3D(RollAxis, rollInDegrees));
        }

        private void YawPitchRoll(AxisAngleRotation3D yaw, AxisAngleRotation3D pitch, AxisAngleRotation3D roll)
        {
            Rotate(yaw);
            Rotate(pitch);
            Rotate(roll);
        }

        #endregion

        #region [====== Rotate ======]

        private Quaternion? _currentRotation;

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
        public void Rotate(AxisAngleRotation3D rotation)
        {
            var camera = Camera;
            if (camera != null)
            {
                var oldRotation = CurrentRotation();
                var newRotation = new Quaternion(rotation.Axis, rotation.Angle) * oldRotation;
                var rotationTransformation = CreateRotationTransformation(newRotation);

                camera.LookDirection = rotationTransformation.Transform(_DefaultLookDirection);
                camera.UpDirection = rotationTransformation.Transform(_DefaultUpDirection);

                _currentRotation = newRotation;
                return;
            }
            throw NewNoCameraSetException();
        }        

        private Quaternion CurrentRotation()
        {
            if (_currentRotation.HasValue)
            {
                return _currentRotation.Value;
            }
            return FromReferenceSystems(_DefaultRightDirection, _DefaultUpDirection, Right, Up).ToQuaternion();        
        }                  

        private static RotateTransform3D CreateRotationTransformation(Quaternion rotation)
        {                        
            return new RotateTransform3D(new QuaternionRotation3D(rotation));
        }

        #endregion
    }
}
