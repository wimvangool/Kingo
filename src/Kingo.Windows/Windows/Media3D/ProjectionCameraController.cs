using System;
using System.ComponentModel;
using System.Windows.Media.Media3D;
using Kingo.Resources;
using static Kingo.Windows.Media3D.EulerAngleRotationSet;

namespace Kingo.Windows.Media3D
{
    /// <summary>
    /// Represents a controller for <see cref="ProjectionCamera">ProjectionCameras</see> to translate and rotate it.
    /// </summary>
    public class ProjectionCameraController : ProjectionCameraControllerBase
    {
        #region [====== State ======]        

        private abstract class State
        {
            public abstract ProjectionCamera Camera
            {
                get;
            }

            public abstract Vector3D Up
            {
                get;
            }

            public abstract Vector3D Left
            {
                get;
            }

            public abstract Vector3D Forward
            {
                get;
            }

            public abstract Quaternion Rotation
            {
                get;
            }

            public abstract void Move(Vector3D direction);

            public abstract void Rotate(AxisAngleRotation3D rotation);

            public abstract void Zoom(double zoomFactor);
        }

        #endregion

        #region [====== NullCameraState ======]

        private sealed class NullCameraState : State
        {
            public override ProjectionCamera Camera => null;

            public override Vector3D Up => _DefaultUpDirection;

            public override Vector3D Left => _DefaultLeftDirection;

            public override Vector3D Forward => _DefaultLookDirection;

            public override Quaternion Rotation => Quaternion.Identity;

            public override void Move(Vector3D direction)
            {
                throw NewCameraNotSetException();
            }

            public override void Rotate(AxisAngleRotation3D rotation)
            {
                throw NewCameraNotSetException();
            }

            public override void Zoom(double zoomFactor)
            {
                throw NewCameraNotSetException();
            }

            private static InvalidOperationException NewCameraNotSetException()
            {
                return new InvalidOperationException(ExceptionMessages.ProjectionCameraController_NoCameraSet);
            }
        }

        #endregion

        #region [====== ProjectionCameraState ======]

        private abstract class ProjectionCameraState : State
        {
            private Quaternion? _currentRotationCache;

            protected abstract ProjectionCameraController Controller
            {
                get;
            }

            public override Vector3D Up => Normalize(Vector3D.CrossProduct(Forward, Left));

            public override Vector3D Left
            {
                get
                {
                    var left = Vector3D.CrossProduct(Camera.UpDirection, Camera.LookDirection);

                    if (IsAlmost(0, left.Length))
                    {
                        left = Vector3D.CrossProduct(_DefaultUpDirection, Camera.LookDirection);
                    }
                    return Normalize(left);
                }
            }

            public override Vector3D Forward => Normalize(Camera.LookDirection);

            public override Quaternion Rotation
            {
                get
                {
                    if (!_currentRotationCache.HasValue)
                    {
                        _currentRotationCache = FromReferenceSystems(_DefaultRightDirection, _DefaultUpDirection, Controller.Right, Controller.Up).ToQuaternion();
                    }
                    return _currentRotationCache.Value;
                }
            }

            public override void Move(Vector3D direction)
            {
                Camera.Position += direction;
            }

            public override void Rotate(AxisAngleRotation3D rotation)
            {
                var newRotation = new Quaternion(rotation.Axis, rotation.Angle) * Rotation;
                var rotationTransformation = CreateRotationTransformation(newRotation);

                Camera.LookDirection = rotationTransformation.Transform(_DefaultLookDirection);
                Camera.UpDirection = rotationTransformation.Transform(_DefaultUpDirection);

                _currentRotationCache = newRotation;

                Controller.OnPropertyChanged(nameof(Rotation));
                
                Controller.OnPropertyChanged(nameof(Left));
                Controller.OnPropertyChanged(nameof(Right));
                
                Controller.OnPropertyChanged(nameof(Up));
                Controller.OnPropertyChanged(nameof(Down));
                
                Controller.OnPropertyChanged(nameof(Forward));
                Controller.OnPropertyChanged(nameof(Backward));
            }

            private static Vector3D Normalize(Vector3D vector)
            {
                vector.Normalize();
                return vector;
            }

            private static RotateTransform3D CreateRotationTransformation(Quaternion rotation)
            {
                return new RotateTransform3D(new QuaternionRotation3D(rotation));
            }
        }

        #endregion

        #region [====== OrthographicCameraState ======]

        private sealed class OrthographicCameraState : ProjectionCameraState
        {
            private readonly OrthographicCamera _camera;

            public OrthographicCameraState(ProjectionCameraController controller, OrthographicCamera camera)
            {
                Controller = controller;

                _camera = camera;
            }

            public override ProjectionCamera Camera => _camera;

            protected override ProjectionCameraController Controller
            {
                get;
            }

            public override void Zoom(double zoomFactor)
            {
                _camera.Width -= zoomFactor;
            }
        }

        #endregion

        #region [====== PerspectiveCameraState ======]

        private sealed class PerspectiveCameraState : ProjectionCameraState
        {
            private readonly PerspectiveCamera _camera;

            public PerspectiveCameraState(ProjectionCameraController controller, PerspectiveCamera camera)
            {
                Controller = controller;

                _camera = camera;
            }

            public override ProjectionCamera Camera => _camera;

            protected override ProjectionCameraController Controller
            {
                get;
            }

            public override void Zoom(double zoomFactor)
            {
                _camera.FieldOfView -= zoomFactor;
            }
        }

        #endregion

        private State _currentState;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectionCameraController" /> class.
        /// </summary>
        public ProjectionCameraController()
        {
            _currentState = new NullCameraState();
        }

        #region [====== NotifyPropertyChanged ======]

        /// <inheritdoc />
        public override event PropertyChangedEventHandler PropertyChanged;
        
        /// <summary>
        /// Raised the <see cref="PropertyChanged"/> event for the specified <paramref name="propertyName"/>.
        /// </summary>
        /// <param name="propertyName">Name of the property that changed.</param>
        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region [====== Camera ======]       

        /// <inheritdoc />
        public override ProjectionCamera Camera
        {
            get { return _currentState.Camera; }
            set
            {
                var oldValue = _currentState.Camera;
                var newValue = value;

                if (newValue != oldValue)
                {
                    _currentState = MoveToNewState(newValue);

                    OnPropertyChanged();
                }
            }
        }    
        
        private State MoveToNewState(object camera)
        {
            var orthographicCamera = camera as OrthographicCamera;
            if (orthographicCamera != null)
            {
                return new OrthographicCameraState(this, orthographicCamera);
            }
            var perspectiveCamera = camera as PerspectiveCamera;
            if (perspectiveCamera != null)
            {
                return new PerspectiveCameraState(this, perspectiveCamera);
            }
            return new NullCameraState();
        }

        #endregion     

        #region [====== Translation ======]                

        /// <inheritdoc />
        public override void Move(Vector3D direction)
        {
            _currentState.Move(direction);
        }       

        #endregion

        #region [====== Rotation - Orientation ======]

        private static readonly Vector3D _DefaultLookDirection = new Vector3D(0, 0, -1);
        private static readonly Vector3D _DefaultUpDirection = new Vector3D(0, 1, 0);
        private static readonly Vector3D _DefaultLeftDirection = new Vector3D(-1, 0, 0);
        private static readonly Vector3D _DefaultRightDirection = new Vector3D(1, 0, 0);        

        /// <inheritdoc />
        public override Vector3D Up => _currentState.Up;

        /// <inheritdoc />
        public override Vector3D Left => _currentState.Left;

        /// <inheritdoc />
        public override Vector3D Forward => _currentState.Forward;                     

        #endregion

        #region [====== Rotation - Yaw, Pitch & Roll ======]

        private Vector3D YawAxis => Up;

        private Vector3D PitchAxis => Right;

        private Vector3D RollAxis => Forward;        

        /// <inheritdoc />
        public override void Yaw(double angleInDegrees)
        {
            Rotate(YawAxis, angleInDegrees);
        }        

        /// <inheritdoc />
        public override void Pitch(double angleInDegrees)
        {
            Rotate(PitchAxis, angleInDegrees);
        }        

        /// <inheritdoc />
        public override void Roll(double angleInDegrees)
        {
            Rotate(RollAxis, angleInDegrees);
        }        

        /// <inheritdoc />
        public override void YawPitchRoll(double yawInDegrees, double pitchInDegrees, double rollInDegrees)
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

        /// <summary>
        /// Gets the current rotation of the camera.
        /// </summary>
        public override Quaternion Rotation => _currentState.Rotation;           

        /// <inheritdoc />
        public override void Rotate(AxisAngleRotation3D rotation)
        {                      
            _currentState.Rotate(rotation);
        }                                                

        #endregion

        #region [====== Zooming ======]

        /// <inheritdoc />
        public override void Zoom(double zoomFactor)
        {
            _currentState.Zoom(zoomFactor);
        }        

        #endregion
    }
}
