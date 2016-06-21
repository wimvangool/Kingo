using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.SharpDX.Direct3D
{
    [TestClass]
    public sealed class RotatableObjectTest
    {
        private RotatableObject _rotatableObject;
        private bool _eventWasRaised;
        private Rotation3D _oldRotation;
        private Rotation3D _newRotation;

        [TestInitialize]
        public void Setup()
        {
            _rotatableObject = new RotatableObject(this);
            _rotatableObject.RotationChanged += (s, e) =>
            {
                _eventWasRaised = true;
                _oldRotation = e.OldRotation;
                _newRotation = e.NewRotation;
            };
        }

        #region [====== Rotate ======]

        [TestMethod]
        public void RotateX_RotatesObjectToExpectedRotation_IfCurrentIsNoRotation()
        {
            var x = RandomAngle();

            _rotatableObject.RotateX(x);

            Assert.IsTrue(_eventWasRaised);
            Assert.AreEqual(Rotation3D.NoRotation, _oldRotation);
            Assert.AreEqual(Rotation3D.FromAngles(x, Angle.Zero, Angle.Zero), _newRotation);
            Assert.AreEqual(_rotatableObject.Rotation, _newRotation);
        }

        [TestMethod]
        public void RotateX_RotatesObjectToExpectedRotation_IfCurrentIsNonZeroRotationAroundX()
        {
            var x0 = RandomAngle();
            var x1 = RandomAngle();

            _rotatableObject.RotateX(x0);
            _rotatableObject.RotateX(x1);

            Assert.IsTrue(_eventWasRaised);
            Assert.AreEqual(Rotation3D.FromAngles(x0, Angle.Zero, Angle.Zero), _oldRotation);
            Assert.AreEqual(Rotation3D.FromAngles(x0 + x1, Angle.Zero, Angle.Zero), _newRotation);
            Assert.AreEqual(_rotatableObject.Rotation, _newRotation);
        }

        [TestMethod]
        public void RotateY_RotatesObjectToExpectedRotation_IfCurrentIsNoRotation()
        {
            var y = RandomAngle();

            _rotatableObject.RotateY(y);

            Assert.IsTrue(_eventWasRaised);
            Assert.AreEqual(Rotation3D.NoRotation, _oldRotation);
            Assert.AreEqual(Rotation3D.FromAngles(Angle.Zero, y, Angle.Zero), _newRotation);
            Assert.AreEqual(_rotatableObject.Rotation, _newRotation);
        }

        [TestMethod]
        public void RotateY_RotatesObjectToExpectedRotation_IfCurrentIsNonZeroRotationAroundY()
        {
            var y0 = RandomAngle();
            var y1 = RandomAngle();

            _rotatableObject.RotateY(y0);
            _rotatableObject.RotateY(y1);

            Assert.IsTrue(_eventWasRaised);
            Assert.AreEqual(Rotation3D.FromAngles(Angle.Zero, y0, Angle.Zero), _oldRotation);
            Assert.AreEqual(Rotation3D.FromAngles(Angle.Zero, y0 + y1, Angle.Zero), _newRotation);
            Assert.AreEqual(_rotatableObject.Rotation, _newRotation);
        }

        [TestMethod]
        public void RotateZ_RotatesObjectToExpectedRotation_IfCurrentIsNoRotation()
        {
            var z = RandomAngle();

            _rotatableObject.RotateZ(z);

            Assert.IsTrue(_eventWasRaised);
            Assert.AreEqual(Rotation3D.NoRotation, _oldRotation);
            Assert.AreEqual(Rotation3D.FromAngles(Angle.Zero, Angle.Zero, z), _newRotation);
            Assert.AreEqual(_rotatableObject.Rotation, _newRotation);
        }

        [TestMethod]
        public void RotateZ_RotatesObjectToExpectedRotation_IfCurrentIsNonZeroRotationAroundZ()
        {
            var z0 = RandomAngle();
            var z1 = RandomAngle();

            _rotatableObject.RotateZ(z0);
            _rotatableObject.RotateZ(z1);

            Assert.IsTrue(_eventWasRaised);
            Assert.AreEqual(Rotation3D.FromAngles(Angle.Zero, Angle.Zero, z0), _oldRotation);
            Assert.AreEqual(Rotation3D.FromAngles(Angle.Zero, Angle.Zero, z0 + z1), _newRotation);
            Assert.AreEqual(_rotatableObject.Rotation, _newRotation);
        }

        #endregion

        #region [====== RotateTo ======]

        [TestMethod]
        public void RotateToX_RotatesObjectToExpectedRotation_IfCurrentIsNoRotation()
        {
            var x = RandomAngle();

            _rotatableObject.RotateToX(x);

            Assert.IsTrue(_eventWasRaised);
            Assert.AreEqual(Rotation3D.NoRotation, _oldRotation);
            Assert.AreEqual(Rotation3D.FromAngles(x, Angle.Zero, Angle.Zero), _newRotation);
            Assert.AreEqual(_rotatableObject.Rotation, _newRotation);
        }

        [TestMethod]
        public void RotateToX_RotatesObjectToExpectedRotation_IfCurrentIsNonZeroRotationAroundX()
        {
            var x0 = RandomAngle();
            var x1 = RandomAngle();

            _rotatableObject.RotateToX(x0);
            _rotatableObject.RotateToX(x1);

            Assert.IsTrue(_eventWasRaised);
            Assert.AreEqual(Rotation3D.FromAngles(x0, Angle.Zero, Angle.Zero), _oldRotation);
            Assert.AreEqual(Rotation3D.FromAngles(x1, Angle.Zero, Angle.Zero), _newRotation);
            Assert.AreEqual(_rotatableObject.Rotation, _newRotation);
        }

        [TestMethod]
        public void RotateToY_RotatesObjectToExpectedRotation_IfCurrentIsNoRotation()
        {
            var y = RandomAngle();

            _rotatableObject.RotateToY(y);

            Assert.IsTrue(_eventWasRaised);
            Assert.AreEqual(Rotation3D.NoRotation, _oldRotation);
            Assert.AreEqual(Rotation3D.FromAngles(Angle.Zero, y, Angle.Zero), _newRotation);
            Assert.AreEqual(_rotatableObject.Rotation, _newRotation);
        }

        [TestMethod]
        public void RotateToY_RotatesObjectToExpectedRotation_IfCurrentIsNonZeroRotationAroundY()
        {
            var y0 = RandomAngle();
            var y1 = RandomAngle();

            _rotatableObject.RotateToY(y0);
            _rotatableObject.RotateToY(y1);

            Assert.IsTrue(_eventWasRaised);
            Assert.AreEqual(Rotation3D.FromAngles(Angle.Zero, y0, Angle.Zero), _oldRotation);
            Assert.AreEqual(Rotation3D.FromAngles(Angle.Zero, y1, Angle.Zero), _newRotation);
            Assert.AreEqual(_rotatableObject.Rotation, _newRotation);
        }

        [TestMethod]
        public void RotateToZ_RotatesObjectToExpectedRotation_IfCurrentIsNoRotation()
        {
            var z = RandomAngle();

            _rotatableObject.RotateToZ(z);

            Assert.IsTrue(_eventWasRaised);
            Assert.AreEqual(Rotation3D.NoRotation, _oldRotation);
            Assert.AreEqual(Rotation3D.FromAngles(Angle.Zero, Angle.Zero, z), _newRotation);
            Assert.AreEqual(_rotatableObject.Rotation, _newRotation);
        }

        [TestMethod]
        public void RotateToZ_RotatesObjectToExpectedRotation_IfCurrentIsNonZeroRotationAroundZ()
        {
            var z0 = RandomAngle();
            var z1 = RandomAngle();

            _rotatableObject.RotateToZ(z0);
            _rotatableObject.RotateToZ(z1);

            Assert.IsTrue(_eventWasRaised);
            Assert.AreEqual(Rotation3D.FromAngles(Angle.Zero, Angle.Zero, z0), _oldRotation);
            Assert.AreEqual(Rotation3D.FromAngles(Angle.Zero, Angle.Zero, z1), _newRotation);
            Assert.AreEqual(_rotatableObject.Rotation, _newRotation);
        }

        #endregion

        #region [====== Pitch, Yaw & Roll ======]

        [TestMethod]
        public void Pitch_RotatesObjectToExpectedRotation_IfCurrentIsNoRotation()
        {
            var x = RandomAngle();

            _rotatableObject.Pitch(x);

            Assert.IsTrue(_eventWasRaised);
            Assert.AreEqual(Rotation3D.NoRotation, _oldRotation);
            Assert.AreEqual(Rotation3D.FromAngles(x, Angle.Zero, Angle.Zero), _newRotation);
            Assert.AreEqual(_rotatableObject.Rotation, _newRotation);
        }

        [TestMethod]
        public void Pitch_RotatesObjectToExpectedRotation_IfCurrentIsNonZeroRotationAroundX()
        {
            var x0 = RandomAngle();
            var x1 = RandomAngle();

            _rotatableObject.Pitch(x0);
            _rotatableObject.Pitch(x1);

            Assert.IsTrue(_eventWasRaised);
            Assert.AreEqual(Rotation3D.FromAngles(x0, Angle.Zero, Angle.Zero), _oldRotation);
            Assert.AreEqual(Rotation3D.FromAngles(x0 + x1, Angle.Zero, Angle.Zero), _newRotation);
            Assert.AreEqual(_rotatableObject.Rotation, _newRotation);
        }

        [TestMethod]
        public void Pitch_RotatesObjectToExpectedRotation_IfCurrentIsNonZeroRotationAroundY()
        {
            var pitch = Angle.FromDegrees(180);
            var y = Angle.FromDegrees(90);            

            _rotatableObject.RotateTo(Angle.Zero, y, Angle.Zero);
            _rotatableObject.Pitch(pitch);

            Assert.IsTrue(_eventWasRaised);
            Assert.AreEqual(Rotation3D.FromAngles(Angle.Zero, y, Angle.Zero), _oldRotation);
            Assert.AreEqual(Rotation3D.FromAngles(Angle.Zero, Angle.FromDegrees(270), pitch), _newRotation);
            Assert.AreEqual(_rotatableObject.Rotation, _newRotation);                 
        }

        [TestMethod]
        public void Pitch_RotatesObjectToExpectedRotation_IfCurrentIsNonZeroRotationAroundZ()
        {
            var otherObject = new RotatableObject(this);
            var pitch = Angle.FromDegrees(170);
            var z = Angle.FromDegrees(90);

            // To check the outcome, we perform different but equivalent rotations on two objects.
            _rotatableObject.RotateToZ(z);
            _rotatableObject.Pitch(pitch);

            otherObject.RotateToZ(z);
            otherObject.RotateY(pitch);

            Assert.IsTrue(_eventWasRaised);
            Assert.AreEqual(Rotation3D.FromAngles(Angle.Zero, Angle.Zero, z), _oldRotation);
            Assert.AreEqual(_rotatableObject.Rotation, _newRotation);
            Assert.AreEqual(otherObject.Rotation, _newRotation);            
        }

        [TestMethod]
        public void Yaw_RotatesObjectToExpectedRotation_IfCurrentIsNoRotation()
        {
            var y = RandomAngle();

            _rotatableObject.Yaw(y);

            Assert.IsTrue(_eventWasRaised);
            Assert.AreEqual(Rotation3D.NoRotation, _oldRotation);
            Assert.AreEqual(Rotation3D.FromAngles(Angle.Zero, y, Angle.Zero), _newRotation);
            Assert.AreEqual(_rotatableObject.Rotation, _newRotation);
        }

        [TestMethod]
        public void Yaw_RotatesObjectToExpectedRotation_IfCurrentIsNonZeroRotationAroundY()
        {
            var y0 = RandomAngle();
            var y1 = RandomAngle();

            _rotatableObject.Yaw(y0);
            _rotatableObject.Yaw(y1);

            Assert.IsTrue(_eventWasRaised);
            Assert.AreEqual(Rotation3D.FromAngles(Angle.Zero, y0, Angle.Zero), _oldRotation);
            Assert.AreEqual(Rotation3D.FromAngles(Angle.Zero, y0 + y1, Angle.Zero), _newRotation);
            Assert.AreEqual(_rotatableObject.Rotation, _newRotation);
        }

        [TestMethod]
        public void Yaw_RotatesObjectToExpectedRotation_IfCurrentIsNonZeroRotationAroundX()
        {
            var otherObject = new RotatableObject(this);
            var yaw = Angle.FromDegrees(170);
            var x = Angle.FromDegrees(90);

            // To check the outcome, we perform different but equivalent rotations on two objects.
            _rotatableObject.RotateToX(x);
            _rotatableObject.Yaw(yaw);

            otherObject.RotateToX(x);
            otherObject.RotateZ(yaw);

            Assert.IsTrue(_eventWasRaised);
            Assert.AreEqual(Rotation3D.FromAngles(x, Angle.Zero, Angle.Zero), _oldRotation);
            Assert.AreEqual(_rotatableObject.Rotation, _newRotation);
            Assert.AreEqual(otherObject.Rotation, _newRotation);
        }

        [TestMethod]
        public void Roll_RotatesObjectToExpectedRotation_IfCurrentIsNoRotation()
        {
            var z = RandomAngle();

            _rotatableObject.Roll(z);

            Assert.IsTrue(_eventWasRaised);
            Assert.AreEqual(Rotation3D.NoRotation, _oldRotation);
            Assert.AreEqual(Rotation3D.FromAngles(Angle.Zero, Angle.Zero, z), _newRotation);
            Assert.AreEqual(_rotatableObject.Rotation, _newRotation);
        }

        [TestMethod]
        public void Roll_RotatesObjectToExpectedRotation_IfCurrentIsNonZeroRationAroundZ()
        {
            var z0 = RandomAngle();
            var z1 = RandomAngle();

            _rotatableObject.Roll(z0);
            _rotatableObject.Roll(z1);

            Assert.IsTrue(_eventWasRaised);
            Assert.AreEqual(Rotation3D.FromAngles(Angle.Zero, Angle.Zero, z0), _oldRotation);
            Assert.AreEqual(Rotation3D.FromAngles(Angle.Zero, Angle.Zero, z0 + z1), _newRotation);
            Assert.AreEqual(_rotatableObject.Rotation, _newRotation);
        }

        [TestMethod]
        public void Roll_RotatesObjectToExpectedRotation_IfCurrentIsNonZeroRotationAroundY()
        {
            var otherObject = new RotatableObject(this);
            var roll = Angle.FromDegrees(170);
            var y = Angle.FromDegrees(90);

            // To check the outcome, we perform different but equivalent rotations on two objects.
            _rotatableObject.RotateToY(y);
            _rotatableObject.Roll(roll);

            otherObject.RotateToY(y);
            otherObject.RotateX(roll);

            Assert.IsTrue(_eventWasRaised);
            Assert.AreEqual(Rotation3D.FromAngles(Angle.Zero, y, Angle.Zero), _oldRotation);
            Assert.AreEqual(_rotatableObject.Rotation, _newRotation);
            Assert.AreEqual(otherObject.Rotation, _newRotation);
        }

        #endregion

        private static readonly Random _RandomValueGenerator = new Random();

        private static Angle RandomAngle()
        {
            lock (_RandomValueGenerator)
            {
                return Angle.FromDegrees(_RandomValueGenerator.Next(1, 180));
            }
        }
    }
}
