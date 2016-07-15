using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpDX;

namespace Kingo.SharpDX.Direct3D
{
    [TestClass]
    public sealed class RotatableObjectTest
    {
        private RotatableObject _rotatableObject;
        private bool _eventWasRaised;
        private RotationTransformation3D _oldRotation;
        private RotationTransformation3D _newRotation;

        [TestInitialize]
        public void Setup()
        {
            _rotatableObject = new RotatableObject(this);
            _rotatableObject.RotationChanged += (s, e) =>
            {
                _eventWasRaised = true;
                _oldRotation = e.OldValue;
                _newRotation = e.NewValue;
            };
        }

        #region [====== Rotate ======]

        [TestMethod]
        public void Rotate_RotatesObjectToExpectedRotation_IfCurrentIsNoRotation()
        {
            var x = RandomAngle();           

            _rotatableObject.Rotate(x, Angle.Zero, Angle.Zero);

            Assert.IsTrue(_eventWasRaised);
            Assert.AreEqual(RotationTransformation3D.NoRotation, _oldRotation);
            Assert.AreEqual(RotationTransformation3D.FromAngles(x, Angle.Zero, Angle.Zero), _newRotation);
            Assert.AreEqual(_rotatableObject.Rotation, _newRotation);
        }

        [TestMethod]
        public void Rotate_RotatesObjectToExpectedRotation_IfCurrentIsNonZeroRotationAroundX()
        {
            var x0 = RandomAngle();
            var x1 = RandomAngle();

            _rotatableObject.Rotate(x0, Angle.Zero, Angle.Zero);
            _rotatableObject.Rotate(x1, Angle.Zero, Angle.Zero);

            Assert.IsTrue(_eventWasRaised);
            Assert.AreEqual(RotationTransformation3D.FromAngles(x0, Angle.Zero, Angle.Zero), _oldRotation);
            Assert.AreEqual(RotationTransformation3D.FromAngles(x0 + x1, Angle.Zero, Angle.Zero), _newRotation);
            Assert.AreEqual(_rotatableObject.Rotation, _newRotation);
        }        

        #endregion

        #region [====== RotateTo ======]

        [TestMethod]
        public void RotateTo_RotatesObjectToExpectedRotation_IfCurrentIsNoRotation()
        {
            var x = RandomAngle();

            _rotatableObject.RotateTo(x, Angle.Zero, Angle.Zero);

            Assert.IsTrue(_eventWasRaised);
            Assert.AreEqual(RotationTransformation3D.NoRotation, _oldRotation);
            Assert.AreEqual(RotationTransformation3D.FromAngles(x, Angle.Zero, Angle.Zero), _newRotation);
            Assert.AreEqual(_rotatableObject.Rotation, _newRotation);
        }

        [TestMethod]
        public void RotateTo_RotatesObjectToExpectedRotation_IfCurrentIsNonZeroRotationAroundX()
        {
            var x0 = RandomAngle();
            var x1 = RandomAngle();

            _rotatableObject.RotateTo(x0, Angle.Zero, Angle.Zero);
            _rotatableObject.RotateTo(x1, Angle.Zero, Angle.Zero);

            Assert.IsTrue(_eventWasRaised);
            Assert.AreEqual(RotationTransformation3D.FromAngles(x0, Angle.Zero, Angle.Zero), _oldRotation);
            Assert.AreEqual(RotationTransformation3D.FromAngles(x1, Angle.Zero, Angle.Zero), _newRotation);
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
            Assert.AreEqual(RotationTransformation3D.NoRotation, _oldRotation);
            Assert.AreEqual(RotationTransformation3D.FromAngles(x, Angle.Zero, Angle.Zero), _newRotation);
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
            Assert.AreEqual(RotationTransformation3D.FromAngles(x0, Angle.Zero, Angle.Zero), _oldRotation);
            Assert.AreEqual(RotationTransformation3D.FromAngles(x0 + x1, Angle.Zero, Angle.Zero), _newRotation);
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
            Assert.AreEqual(RotationTransformation3D.FromAngles(Angle.Zero, y, Angle.Zero), _oldRotation);
            Assert.AreEqual(RotationTransformation3D.FromAngles(Angle.Zero, Angle.FromDegrees(270), pitch), _newRotation);
            Assert.AreEqual(_rotatableObject.Rotation, _newRotation);                 
        }

        [TestMethod]
        public void Pitch_RotatesObjectToExpectedRotation_IfCurrentIsNonZeroRotationAroundZ()
        {
            var otherObject = new RotatableObject(this);
            var pitch = Angle.FromDegrees(170);
            var z = Angle.FromDegrees(90);

            // To check the outcome, we perform different but equivalent rotations on two objects.
            _rotatableObject.RotateTo(Angle.Zero, Angle.Zero, z);
            _rotatableObject.Pitch(pitch);

            otherObject.RotateTo(Angle.Zero, Angle.Zero, z);
            otherObject.Rotate(Angle.Zero, pitch, Angle.Zero);

            Assert.IsTrue(_eventWasRaised);
            Assert.AreEqual(RotationTransformation3D.FromAngles(Angle.Zero, Angle.Zero, z), _oldRotation);
            Assert.AreEqual(_rotatableObject.Rotation, _newRotation);
            Assert.AreEqual(otherObject.Rotation, _newRotation);            
        }

        [TestMethod]
        public void Yaw_RotatesObjectToExpectedRotation_IfCurrentIsNoRotation()
        {
            var y = RandomAngle();

            _rotatableObject.Yaw(y);

            Assert.IsTrue(_eventWasRaised);
            Assert.AreEqual(RotationTransformation3D.NoRotation, _oldRotation);
            Assert.AreEqual(RotationTransformation3D.FromAngles(Angle.Zero, y, Angle.Zero), _newRotation);
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
            Assert.AreEqual(RotationTransformation3D.FromAngles(Angle.Zero, y0, Angle.Zero), _oldRotation);
            Assert.AreEqual(RotationTransformation3D.FromAngles(Angle.Zero, y0 + y1, Angle.Zero), _newRotation);
            Assert.AreEqual(_rotatableObject.Rotation, _newRotation);
        }

        [TestMethod]
        public void Yaw_RotatesObjectToExpectedRotation_IfCurrentIsNonZeroRotationAroundX()
        {
            var otherObject = new RotatableObject(this);
            var yaw = Angle.FromDegrees(170);
            var x = Angle.FromDegrees(90);

            // To check the outcome, we perform different but equivalent rotations on two objects.
            _rotatableObject.RotateTo(x, Angle.Zero, Angle.Zero);
            _rotatableObject.Yaw(yaw);

            otherObject.RotateTo(x, Angle.Zero, Angle.Zero);
            otherObject.Rotate(Angle.Zero, Angle.Zero, yaw);

            Assert.IsTrue(_eventWasRaised);
            Assert.AreEqual(RotationTransformation3D.FromAngles(x, Angle.Zero, Angle.Zero), _oldRotation);
            Assert.AreEqual(_rotatableObject.Rotation, _newRotation);
            Assert.AreEqual(otherObject.Rotation, _newRotation);
        }

        [TestMethod]
        public void Roll_RotatesObjectToExpectedRotation_IfCurrentIsNoRotation()
        {
            var z = RandomAngle();

            _rotatableObject.Roll(z);

            Assert.IsTrue(_eventWasRaised);
            Assert.AreEqual(RotationTransformation3D.NoRotation, _oldRotation);
            Assert.AreEqual(RotationTransformation3D.FromAngles(Angle.Zero, Angle.Zero, z), _newRotation);
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
            Assert.AreEqual(RotationTransformation3D.FromAngles(Angle.Zero, Angle.Zero, z0), _oldRotation);
            Assert.AreEqual(RotationTransformation3D.FromAngles(Angle.Zero, Angle.Zero, z0 + z1), _newRotation);
            Assert.AreEqual(_rotatableObject.Rotation, _newRotation);
        }

        [TestMethod]
        public void Roll_RotatesObjectToExpectedRotation_IfCurrentIsNonZeroRotationAroundY()
        {
            var otherObject = new RotatableObject(this);
            var roll = Angle.FromDegrees(170);
            var y = Angle.FromDegrees(90);

            // To check the outcome, we perform different but equivalent rotations on two objects.
            _rotatableObject.RotateTo(Angle.Zero, y, Angle.Zero);
            _rotatableObject.Roll(roll);

            otherObject.RotateTo(Angle.Zero, y, Angle.Zero);
            otherObject.Rotate(roll, Angle.Zero, Angle.Zero);

            Assert.IsTrue(_eventWasRaised);
            Assert.AreEqual(RotationTransformation3D.FromAngles(Angle.Zero, y, Angle.Zero), _oldRotation);
            Assert.AreEqual(_rotatableObject.Rotation, _newRotation);
            Assert.AreEqual(otherObject.Rotation, _newRotation);
        }

        [TestMethod]
        public void PitchYawRoll_RotatesObjectToExpectedRotation_IfCurrentIsNoRotation()
        {
            var pitch = Angle.FromDegrees(20);
            var yaw = Angle.FromDegrees(30);
            var roll = Angle.FromDegrees(40);

            _rotatableObject.PitchYawRoll(pitch, yaw, roll);

            Assert.IsTrue(_eventWasRaised);            
            Assert.AreEqual(_rotatableObject.Rotation, _newRotation);
            Assert.AreEqual(RotationTransformation3D.FromAngles(pitch, yaw, roll), _newRotation);
        }

        [TestMethod]
        public void PitchYawRoll_RotatesObjectToExpectedRotation_IfCurrentIsComplexRotation()
        {
            var length = (float) (Math.Sqrt(2) / 2);

            var pitch = Angle.FromDegrees(90);
            var yaw = Angle.FromDegrees(45);
            var roll = Angle.Zero;

            _rotatableObject.Pitch(Angle.FromDegrees(180));
            _rotatableObject.PitchYawRoll(pitch, yaw, roll);

            Assert.IsTrue(_eventWasRaised);
            Assert.AreEqual(_rotatableObject.Rotation, _newRotation);
            
            AssertAreEqual(length, length, 0, _newRotation.Forward);
            AssertAreEqual(length, -length, 0, _newRotation.Right);
            AssertAreEqual(0, 0, -1, _newRotation.Up);
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

        private static void AssertAreEqual(float x, float y, float z, Vector3 vector)
        {
            AngleTest.AssertAreEqual(x, vector.X);
            AngleTest.AssertAreEqual(y, vector.Y);
            AngleTest.AssertAreEqual(z, vector.Z);
        }
    }
}
