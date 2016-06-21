using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.SharpDX.Direct3D
{
    [TestClass]
    public sealed class ProjectionPlanesTest
    {
        #region [====== Constructor (By Floats) ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Constructor_ByFloats_Throws_IfNearPlaneIsSmallerThanZero()
        {
            new ProjectionPlanes(-1, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Constructor_ByFloats_Throws_IfNearPlaneIsEqualToZero()
        {
            new ProjectionPlanes(0, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Constructor_ByFloats_Throws_IfNearPlaneIsEqualToFarPlane()
        {
            new ProjectionPlanes(2, 2);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Constructor_ByFloats_Throws_IfNearPlaneIsLargerThanFarPlane()
        {
            new ProjectionPlanes(2, 1);
        }

        [TestMethod]        
        public void Constructor_ByFloats_ReturnsExpectedPlanes_IfNearPlaneIsSmallerThanFarPlane()
        {
            var planes = new ProjectionPlanes(1, 2);

            Assert.AreEqual(new Length(1), planes.NearPlane);
            Assert.AreEqual(new Length(2), planes.FarPlane);
        }

        #endregion

        #region [====== Constructor (By Length) ======]       

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Constructor_ByLengths_Throws_IfNearPlaneIsEqualToZero()
        {
            new ProjectionPlanes(Length.Zero, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Constructor_ByLengths_Throws_IfNearPlaneIsEqualToFarPlane()
        {
            new ProjectionPlanes(new Length(2), new Length(2));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Constructor_ByLengths_Throws_IfNearPlaneIsLargerThanFarPlane()
        {
            new ProjectionPlanes(new Length(2), new Length(1));
        }

        [TestMethod]
        public void Constructor_ByLengths_ReturnsExpectedPlanes_IfNearPlaneIsSmallerThanFarPlane()
        {
            var planes = new ProjectionPlanes(new Length(1), new Length(2));

            Assert.AreEqual(new Length(1), planes.NearPlane);
            Assert.AreEqual(new Length(2), planes.FarPlane);
        }

        #endregion

        #region [====== ToString ======]

        [TestMethod]
        public void ToString_ReturnsExpectedValue()
        {
            Assert.AreEqual("[1 - 20]", new ProjectionPlanes(1, 20).ToString());
        }

        #endregion

        #region [====== MoveNearPlane ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void MoveNearPlane_Throws_IfNearPlaneIsMovedBehindCamera()
        {
            var planes = new ProjectionPlanes(1, 10);

            planes.Move(-2);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void MoveNearPlane_Throws_IfNearPlaneIsMovedToCamera()
        {
            var planes = new ProjectionPlanes(1, 10);

            planes.Move(-1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void MoveNearPlane_Throws_IfNearPlaneIsMovedBeyondFarPlane()
        {
            var planes = new ProjectionPlanes(1, 10);

            planes.MoveNearPlane(10);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void MoveNearPlane_Throws_IfNearPlaneIsMovedToFarPlane()
        {
            var planes = new ProjectionPlanes(1, 10);

            planes.MoveNearPlane(9);
        }

        [TestMethod]        
        public void MoveNearPlane_ReturnsExpectedPlanes_IfNearPlaneIsMovedBetweenCameraAndFarPlane()
        {
            var planes = new ProjectionPlanes(1, 10);
            var movedPlanes = planes.MoveNearPlane(8);
            
            Assert.AreEqual(new Length(9), movedPlanes.NearPlane);
            Assert.AreEqual(new Length(10), movedPlanes.FarPlane);
        }

        #endregion

        #region [====== MoveFarPlane ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void MoveFarPlane_Throws_IfFarPlaneIsMovedBehindNearPlane()
        {
            var planes = new ProjectionPlanes(3, 10);

            planes.Move(-8);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void MoveFarPlane_Throws_IfFarPlaneIsMovedToNearPlane()
        {
            var planes = new ProjectionPlanes(1, 10);

            planes.Move(-9);
        }               

        [TestMethod]
        public void MoveFarPlane_ReturnsExpectedPlanes_IfFarPlaneIsMovedBeyondNearPlane()
        {
            var planes = new ProjectionPlanes(1, 10);
            var movedPlanes = planes.MoveFarPlane(8);

            Assert.AreEqual(new Length(1), movedPlanes.NearPlane);
            Assert.AreEqual(new Length(18), movedPlanes.FarPlane);
        }

        #endregion

        #region [====== Move ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Move_Throws_IfNearPlaneIsMovedBehindCamera()
        {
            var planes = new ProjectionPlanes(1, 4);

            planes.Move(-2);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Move_Throws_IfNearPlaneIsMovedToCamera()
        {
            var planes = new ProjectionPlanes(1, 4);

            planes.Move(-1);
        }

        [TestMethod]
        public void Move_ReturnsExpectedPlanes_IfPlanesAreMovedBeyondCamera()
        {
            var planes = new ProjectionPlanes(1, 5);
            var movedPlanes = planes.Move(6);

            Assert.AreEqual(new Length(7), movedPlanes.NearPlane);
            Assert.AreEqual(new Length(11), movedPlanes.FarPlane);
        }

        #endregion
    }
}
