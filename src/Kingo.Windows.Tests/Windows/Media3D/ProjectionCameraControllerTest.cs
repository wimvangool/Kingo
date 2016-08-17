using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static System.Math;

namespace Kingo.Windows.Media3D
{
    [TestClass]
    public sealed class ProjectionCameraControllerTest
    {
        private static readonly double _HalfSquared = Round(Sqrt(0.5), 4);
        private ProjectionCameraController _controller;        

        [TestInitialize]
        public void Setup()
        {
            _controller = new ProjectionCameraController();            
        }  
        
        private ProjectionCamera ProjectionCamera => _controller.Camera as ProjectionCamera;

        #region [====== Camera ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SetCamera_Throws_IfCameraTypeIsNotSupported()
        {
            _controller.Camera = new MatrixCamera();
        }

        #endregion

        #region [====== PropertyChanged ======]

        [TestMethod]
        public void PropertyChanged_IsRaised_WhenCameraIsChanged()
        {
            var properties = new List<string>();

            _controller.PropertyChanged += (s, e) => properties.Add(e.PropertyName);

            _controller.Camera = new PerspectiveCamera();

            Assert.AreEqual(1, properties.Count);
            Assert.IsNull(properties[0]);            
        }

        #endregion

        #region [====== MoveLeftRight ======]

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void MoveLeftRight_Throws_IfNoCameraHasBeenSet()
        {
            _controller.MoveLeftRight(RandomDistance());            
        }        

        [TestMethod]
        public void MoveLeftRight_MovesTheCameraAlongItsHorizontalAxis_IfDistanceIsNotZero()
        {
            var distance = RandomDistance();

            _controller.Camera = new PerspectiveCamera();
            _controller.MoveLeftRight(distance);
            
            AssertAreEqual(distance, 0, 0, ProjectionCamera.Position);
        }

        [TestMethod]
        public void MoveLeftRight_MovesTheCameraAlongItsHorizontalAxis_IfDistanceIsNotZero_And_CurrentPositionIsNotOrigin()
        {
            var position = RandomPosition();
            var distance = RandomDistance();

            _controller.Camera = new PerspectiveCamera()
            {
                Position = position
            };            
            _controller.MoveLeftRight(distance);            
            
            AssertAreEqual(position.X + distance, position.Y, position.Z, ProjectionCamera.Position);
        }

        #endregion

        #region [====== MoveUpDown ======]

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void MoveUpDown_Throws_IfNoCameraHasBeenSet()
        {
            _controller.MoveUpDown(RandomDistance());
        }

        [TestMethod]
        public void MoveUpDown_MovesTheCameraAlongItsVerticalAxis_IfDistanceIsNotZero()
        {
            var distance = RandomDistance();

            _controller.Camera = new PerspectiveCamera();
            _controller.MoveUpDown(distance);
            
            AssertAreEqual(0, distance, 0, ProjectionCamera.Position);
        }

        [TestMethod]
        public void MoveUpDown_MovesTheCameraAlongItsVerticalAxis_IfDistanceIsNotZero_And_CurrentPositionIsNotOrigin()
        {
            var position = RandomPosition();
            var distance = RandomDistance();

            _controller.Camera = new PerspectiveCamera()
            {
                Position = position
            };
            _controller.MoveUpDown(distance);

            AssertAreEqual(position.X, position.Y + distance, position.Z, ProjectionCamera.Position);
        }

        #endregion

        #region [====== MoveForwardBackward ======]

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void MoveForwardBackward_Throws_IfNoCameraHasBeenSet()
        {
            _controller.MoveForwardBackward(RandomDistance());
        }

        [TestMethod]
        public void MoveForwardBackward_MovesTheCameraAlongItsLookDirection_IfDistanceIsNotZero()
        {
            var distance = RandomDistance();

            _controller.Camera = new PerspectiveCamera();
            _controller.MoveForwardBackward(distance);

            AssertAreEqual(0, 0, -distance, ProjectionCamera.Position);
        }

        [TestMethod]
        public void MoveForwardBackward_MovesTheCameraAlongItsLookDirection_IfDistanceIsNotZero_And_CurrentPositionIsNotOrigin()
        {
            var position = RandomPosition();
            var distance = RandomDistance();

            _controller.Camera = new PerspectiveCamera()
            {
                Position = position
            };
            _controller.MoveForwardBackward(distance);

            AssertAreEqual(position.X, position.Y, position.Z - distance, ProjectionCamera.Position);
        }

        #endregion

        #region [====== Move ======]

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Move_Throws_IfNoCameraHasBeenSet()
        {
            _controller.Move(RandomDirection());
        }

        [TestMethod]
        public void Move_DoesNothing_IfDirectionIsZeroVector()
        {
            _controller.Camera = new PerspectiveCamera();
            _controller.Move(new Vector3D());
            
            AssertAreEqual(0, 0, 0, ProjectionCamera.Position);
        }

        [TestMethod]
        public void Move_MovesCameraInSpecifiedDirection_IfDirectionIsNotZero()
        {
            var direction = RandomDirection();

            _controller.Camera = new PerspectiveCamera();
            _controller.Move(direction);
            
            AssertAreEqual(direction.X, direction.Y, direction.Z, ProjectionCamera.Position);
        }

        [TestMethod]
        public void Move_MovesCameraInSpecifiedDirection_IfDirectionIsNotZero_And_CurrentPositionIsNotOrigin()
        {
            var position = RandomPosition();
            var direction = RandomDirection();

            _controller.Camera = new PerspectiveCamera()
            {
                Position = position
            };
            _controller.Move(direction);
            
            AssertAreEqual(position.X + direction.X, position.Y + direction.Y, position.Z + direction.Z, ProjectionCamera.Position);
        }

        #endregion

        #region [====== Up ======]

        [TestMethod]
        public void Up_ReturnsDefaultUpVector_IfCameraIsNotSet()
        {            
            AssertAreEqual(0, 1, 0, _controller.Up);
        }

        [TestMethod]
        public void Up_ReturnsDefaultUpVector_IfCameraHasDefaultLookDirectionAndUpVector()
        {
            _controller.Camera = new PerspectiveCamera();

            AssertAreEqual(0, 1, 0, _controller.Up);
        }

        [TestMethod]
        public void Up_ReturnsDefaultUpVector_IfCameraHasNonDefaultUpVectorPointingUpwards()
        {
            _controller.Camera = new PerspectiveCamera()
            {
                UpDirection = new Vector3D(0, 1, -1)
            };

            AssertAreEqual(0, 1, 0, _controller.Up);
        }

        [TestMethod]
        public void Up_ReturnsExpectedUpVector_IfCameraHasNonDefaultUpVectorPointingDownwards()
        {
            _controller.Camera = new PerspectiveCamera()
            {
                UpDirection = new Vector3D(0, -1, -1)
            };

            AssertAreEqual(0, -1, 0, _controller.Up);
        }

        #endregion

        #region [====== Down ======]

        [TestMethod]
        public void Down_ReturnsDefaultDownVector_IfCameraIsNotSet()
        {            
            AssertAreEqual(0, -1, 0, _controller.Down);
        }

        [TestMethod]
        public void Down_ReturnsDefaultDownVector_IfCameraHasDefaultLookDirectionAndUpVector()
        {
            _controller.Camera = new PerspectiveCamera();

            AssertAreEqual(0, -1, 0, _controller.Down);
        }

        #endregion

        #region [====== Left ======]

        [TestMethod]
        public void Left_ReturnsDefaultLeftVector_IfCameraIsNotSet()
        {            
            AssertAreEqual(-1, 0, 0, _controller.Left);
        }

        [TestMethod]
        public void Left_ReturnsDefaultLeftVector_IfCameraHasDefaultLookDirectionAndUpVector()
        {
            _controller.Camera = new PerspectiveCamera();

            AssertAreEqual(-1, 0, 0, _controller.Left);
        }        

        [TestMethod]
        public void Left_ReturnsExpectedLeftVector_IfLookDirectionIsNotDefaultLookDirection()
        {
            _controller.Camera = new PerspectiveCamera()
            {
                LookDirection = new Vector3D(1, 0, -1)
            };

            AssertAreEqual(-_HalfSquared, 0, -_HalfSquared, _controller.Left);
        }        

        [TestMethod]
        public void Left_ReturnsDefaultLeftVector_IfCameraHasUpVectorThatEqualsLookDirection()
        {
            _controller.Camera = new PerspectiveCamera()
            {
                UpDirection = new Vector3D(0, 0, -1)
            };

            AssertAreEqual(-1, 0, 0, _controller.Left);
        }

        [TestMethod]
        public void Left_ReturnsDefaultLeftVector_IfCameraHasUpVectorThatIsOppositeOfLookDirection()
        {
            _controller.Camera = new PerspectiveCamera()
            {
                UpDirection = new Vector3D(0, 0, 1)
            };

            AssertAreEqual(-1, 0, 0, _controller.Left);
        }

        [TestMethod]
        public void Left_ReturnsDefaultLeftVector_IfCameraHasNonDefaultUpVectorPointingUpwards()
        {
            _controller.Camera = new PerspectiveCamera()
            {
                UpDirection = new Vector3D(0, 1, -1)
            };

            AssertAreEqual(-1, 0, 0, _controller.Left);
        }

        [TestMethod]
        public void Left_ReturnsDefaultLeftVector_IfCameraHasNonDefaultUpVectorPointingDownwards()
        {
            _controller.Camera = new PerspectiveCamera()
            {
                UpDirection = new Vector3D(0, -1, -1)
            };

            AssertAreEqual(1, 0, 0, _controller.Left);
        }

        #endregion

        #region [====== Right ======]

        [TestMethod]
        public void Right_ReturnsDefaultRightVector_IfCameraIsNotSet()
        {            
            AssertAreEqual(1, 0, 0, _controller.Right);
        }

        [TestMethod]
        public void Right_ReturnsDefaultRightVector_IfCameraHasDefaultLookDirectionAndUpVector()
        {
            _controller.Camera = new PerspectiveCamera();

            AssertAreEqual(1, 0, 0, _controller.Right);
        }

        #endregion

        #region [====== Forward ======]

        [TestMethod]
        public void Forward_ReturnsDefaultForwardVector_IfCameraIsNotSet()
        {            
            AssertAreEqual(0, 0, -1, _controller.Forward);
        }

        [TestMethod]
        public void Forward_ReturnsDefaultForwardVector_IfCameraHasDefaultLookDirectionAndUpVector()
        {
            _controller.Camera = new PerspectiveCamera();

            AssertAreEqual(0, 0, -1, _controller.Forward);
        }

        #endregion

        #region [====== Backward ======]

        [TestMethod]
        public void Backward_ReturnsDefaultBackwardVector_IfCameraIsNotSet()
        {            
            AssertAreEqual(0, 0, 1, _controller.Backward);
        }

        [TestMethod]
        public void Backward_ReturnsDefaultBackwardVector_IfCameraHasDefaultLookDirectionAndUpVector()
        {
            _controller.Camera = new PerspectiveCamera();

            AssertAreEqual(0, 0, 1, _controller.Backward);
        }

        #endregion

        #region [====== Rotate ======]

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Rotate_Throws_IfCameraIsNotSet()
        {
            _controller.Rotate(RandomDirection(), RandomAngle());
        }

        [TestMethod]
        public void Rotate_RaisesPropertyChangedEventForEveryDirection()
        {
            var properties = new List<string>();
            
            _controller.Camera = new PerspectiveCamera();
            _controller.PropertyChanged += (s, e) => properties.Add(e.PropertyName);
            _controller.Rotate(RandomDirection(), RandomAngle());

            Assert.AreEqual(7, properties.Count);
            Assert.IsTrue(properties.Contains(nameof(_controller.Rotation)));

            Assert.IsTrue(properties.Contains(nameof(_controller.Left)));
            Assert.IsTrue(properties.Contains(nameof(_controller.Right)));

            Assert.IsTrue(properties.Contains(nameof(_controller.Up)));
            Assert.IsTrue(properties.Contains(nameof(_controller.Down)));

            Assert.IsTrue(properties.Contains(nameof(_controller.Forward)));
            Assert.IsTrue(properties.Contains(nameof(_controller.Backward)));
        }

        [TestMethod]
        public void Rotate_RotatesCameraAsExpected_IfRotationIsAroundXAxis_And_CurrentRotationIsZero()
        {
            var camera = new PerspectiveCamera();

            _controller.Camera = camera;
            _controller.Rotate(_controller.Left, 90);

            AssertAreEqual(0, -1, 0, ProjectionCamera.LookDirection);
            AssertAreEqual(0, 0, -1, ProjectionCamera.UpDirection);
        }
        
        [TestMethod]
        public void Rotate_RotatesCameraAsExpected_IfRotationIsAroundXAxis_And_CameraIsRotatedTwice()
        {
            var camera = new PerspectiveCamera();

            _controller.Camera = camera;
            _controller.Rotate(_controller.Left, 45);
            _controller.Rotate(_controller.Left, 45);

            AssertAreEqual(0, -1, 0, ProjectionCamera.LookDirection);
            AssertAreEqual(0, 0, -1, ProjectionCamera.UpDirection);
        }                    

        [TestMethod]
        public void Rotate_RotatesCameraAsExpected_IfCameraIsAlreadyRotatedClockwiseAroundTheYAxis_90_Degrees()
        {
            // Camera is rotated 90 degrees clockwise around the Y-axis.
            var camera = new PerspectiveCamera()
            {
                LookDirection = new Vector3D(-1, 0, 0)
            };

            // Camera is now rotated back to its default rotation.
            _controller.Camera = camera;
            _controller.Rotate(_controller.Up, -90);

            AssertAreEqual(0, 0, -1, camera.LookDirection);
            AssertAreEqual(0, 1, 0, camera.UpDirection);
        }

        [TestMethod]
        public void Rotate_RotatesCameraAsExpected_IfCameraIsAlreadyRotatedClockwiseAroundTheYAxis_180_Degrees()
        {
            // Camera is rotated 180 degrees clockwise around the Y-axis.
            var camera = new PerspectiveCamera()
            {
                LookDirection = new Vector3D(0, 0, 1)
            };

            // Camera is now rotated back to its default rotation.
            _controller.Camera = camera;
            _controller.Rotate(_controller.Up, 180);

            AssertAreEqual(0, 0, -1, camera.LookDirection);
            AssertAreEqual(0, 1, 0, camera.UpDirection);
        }        

        [TestMethod]
        public void Rotate_RotatesCameraAsExpected_IfCameraIsAlreadyRotatedCounterClockwiseAroundTheZAxis_90_Degrees()
        {
            // Camera is rotated 90 degrees counter-clockwise around the (negative) Z-axis.
            var camera = new PerspectiveCamera()
            {
                UpDirection = new Vector3D(-1, 0, 0)
            };

            // Camera is now rotated back to its default rotation.
            _controller.Camera = camera;
            _controller.Rotate(_controller.Forward, 90);

            AssertAreEqual(0, 0, -1, camera.LookDirection);
            AssertAreEqual(0, 1, 0, camera.UpDirection);
        }

        [TestMethod]
        public void Rotate_RotatesCameraAsExpected_IfCameraIsAlreadyRotatedClockwiseAroundTheZAxis_90_Degrees()
        {
            // Camera is rotated 90 degrees clockwise around the (negative) Z-axis.
            var camera = new PerspectiveCamera()
            {                
                UpDirection = new Vector3D(1, 0, 0)
            };

            // Camera is now rotated back to its default rotation.
            _controller.Camera = camera;
            _controller.Rotate(_controller.Forward, -90);

            AssertAreEqual(0, 0, -1, camera.LookDirection);
            AssertAreEqual(0, 1, 0, camera.UpDirection);
        }

        [TestMethod]
        public void Rotate_RotatesCameraAsExpected_IfCameraIsAlreadyRotatedClockwiseAroundTheZAxis_180_Degrees()
        {
            // Camera is rotated 180 degrees clockwise around the (negative) Z-axis.
            var camera = new PerspectiveCamera()
            {
                UpDirection = new Vector3D(0, -1, 0)
            };

            // Camera is now rotated back to its default rotation.
            _controller.Camera = camera;
            _controller.Rotate(_controller.Forward, 180);

            AssertAreEqual(0, 0, -1, camera.LookDirection);
            AssertAreEqual(0, 1, 0, camera.UpDirection);
        }        

        #endregion

        #region [====== Asserts ======]

        private static void AssertAreEqual(double x, double y, double z, Point3D position)
        {
            AssertAreEqual(x, position.X, FormatMessage(nameof(x)));
            AssertAreEqual(y, position.Y, FormatMessage(nameof(y)));
            AssertAreEqual(z, position.Z, FormatMessage(nameof(z)));
        }

        private static void AssertAreEqual(double x, double y, double z, Vector3D direction)
        {
            AssertAreEqual(x, direction.X, FormatMessage(nameof(x)));
            AssertAreEqual(y, direction.Y, FormatMessage(nameof(y)));
            AssertAreEqual(z, direction.Z, FormatMessage(nameof(z)));
        }

        private static void AssertAreEqual(double expected, double actual, string message)
        {
            Assert.AreEqual(expected, Round(actual, 4), message);
        }

        private static string FormatMessage(string axis)
        {
            return $"{axis.ToUpperInvariant()}-values of don't match.";
        }

        #endregion

        #region [====== Random Values ======]

        private static readonly Random _Random = new Random();

        private static Point3D RandomPosition()
        {
            return new Point3D(RandomDistance(), RandomDistance(), RandomDistance());
        }

        private static Vector3D RandomDirection()
        {
            return new Vector3D(RandomDistance(), RandomDistance(), RandomDistance());
        }

        private static Angle RandomAngle()
        {
            lock (_Random)
            {
                return Angle.FromDegrees(_Random.Next(-360, 360));
            }
        }

        private static double RandomDistance()
        {
            lock (_Random)
            {
                int value;

                do
                {
                    value = _Random.Next(-100, 100);
                } while (value == 0);

                return value;
            }
        }

        #endregion
    }
}
