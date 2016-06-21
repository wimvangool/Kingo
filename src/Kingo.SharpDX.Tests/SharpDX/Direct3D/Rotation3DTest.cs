using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpDX;

namespace Kingo.SharpDX.Direct3D
{
    [TestClass]
    public sealed class Rotation3DTest
    {
        #region [====== EquatableTestSuite ======]

        private static readonly EquatableValueTypeTestSuite<Rotation3D> _EquatableTestSuite = new EquatableValueTypeTestSuite<Rotation3D>(new MSTestEngine());
        
        [TestMethod]
        public void EquatableTestSuite_ExecutesSuccesfully()
        {
            _EquatableTestSuite.Execute(new EquatableTestParameters<Rotation3D>()
            {
                Instance = Rotation3D.NoRotation,
                EqualInstance = Rotation3D.NoRotation,
                UnequalInstance = Rotation3D.FromDegrees(10, 20, 30)
            });
        }

        #endregion

        #region [====== FromAngleAroundAxis ======]

        [TestMethod]
        public void FromAngleAroundAxis_ReturnsExpectedRotation_IfAngleIsZero()
        {
            var rotation = Rotation3D.FromAngleAroundAxis(Vector3.ForwardLH, Angle.Zero);

            AssertAreEqual(Angle.Zero, rotation.AroundX);
            AssertAreEqual(Angle.Zero, rotation.AroundY);
            AssertAreEqual(Angle.Zero, rotation.AroundZ);
        }

        [TestMethod]
        public void FromAngleAroundAxis_ReturnsExpectedRotation_IfAxisIsUnitX()
        {
            var rotation = Rotation3D.FromAngleAroundAxis(Vector3.UnitX, Angle.FromDegrees(40));

            AssertAreEqual(Angle.FromDegrees(40), rotation.AroundX);
            AssertAreEqual(Angle.Zero, rotation.AroundY);
            AssertAreEqual(Angle.Zero, rotation.AroundZ);
        }

        [TestMethod]
        public void FromAngleAroundAxis_ReturnsExpectedRotation_IfAxisIsUnitY()
        {
            var rotation = Rotation3D.FromAngleAroundAxis(Vector3.UnitY, Angle.FromDegrees(60));

            AssertAreEqual(Angle.Zero, rotation.AroundX);
            AssertAreEqual(Angle.FromDegrees(60), rotation.AroundY);
            AssertAreEqual(Angle.Zero, rotation.AroundZ);
        }

        [TestMethod]
        public void FromAngleAroundAxis_ReturnsExpectedRotation_IfAxisIsUnitZ()
        {
            var rotation = Rotation3D.FromAngleAroundAxis(Vector3.UnitZ, Angle.FromDegrees(80));

            AssertAreEqual(Angle.Zero, rotation.AroundX);
            AssertAreEqual(Angle.Zero, rotation.AroundY);
            AssertAreEqual(Angle.FromDegrees(80), rotation.AroundZ);
        }

        #endregion

        #region [====== FromAngles ======]  

        [TestMethod]
        public void FromAngles_ReturnsExpectedRotation_IfAllAnglesAreZero()
        {
            var rotation = Rotation3D.FromAngles(Angle.Zero, Angle.Zero, Angle.Zero);

            AssertAreEqual(Angle.Zero, rotation.AroundX);
            AssertAreEqual(Angle.Zero, rotation.AroundY);
            AssertAreEqual(Angle.Zero, rotation.AroundZ);
        }

        [TestMethod]
        public void FromAngles_ReturnsExpectedRotation_IfOnlyXAngleIsSpecified()
        {
            var x = Angle.FromDegrees(180);
            var y = Angle.Zero;
            var z = Angle.Zero;
            var rotation = Rotation3D.FromAngles(x, y, z);

            AssertAreEqual(x, rotation.AroundX.Absolute());
            AssertAreEqual(y, rotation.AroundY);
            AssertAreEqual(z, rotation.AroundZ);
        }

        [TestMethod]
        public void FromAngles_ReturnsExpectedRotation_IfOnlyYAngleIsSpecified()
        {
            var x = Angle.Zero;
            var y = Angle.FromDegrees(90);
            var z = Angle.Zero;
            var rotation = Rotation3D.FromAngles(x, y, z);

            AssertAreEqual(x, rotation.AroundX);
            AssertAreEqual(y, rotation.AroundY);
            AssertAreEqual(z, rotation.AroundZ);
        }

        [TestMethod]
        public void FromAngles_ReturnsExpectedRotation_IfOnlyZAngleIsSpecified()
        {
            var x = Angle.Zero;
            var y = Angle.Zero;
            var z = Angle.FromDegrees(30);
            var rotation = Rotation3D.FromAngles(x, y, z);

            AssertAreEqual(x, rotation.AroundX);
            AssertAreEqual(y, rotation.AroundY);
            AssertAreEqual(z, rotation.AroundZ);
        }

        [TestMethod]
        public void FromAngles_ReturnsExpectedRotation_IfBoth_X_And_Y_And_Z_AnglesAreSpecified()
        {
            var x = Angle.FromDegrees(90);
            var y = Angle.FromDegrees(180);
            var z = Angle.FromDegrees(180);
            var rotation = Rotation3D.FromAngles(x, y, z);

            AssertAreEqual(Angle.FromDegrees(-90), rotation.AroundX);
            AssertAreEqual(Angle.Zero, rotation.AroundY);
            AssertAreEqual(Angle.Zero, rotation.AroundZ);
        }

        #endregion

        #region [====== FromRadians ======]

        [TestMethod]
        public void FromRadians_ByVector_ReturnsExpectedRotationInstance()
        {
            var x = RandomRadians();            
            var rotation = Rotation3D.FromRadians(new Vector3(x, 0, 0));

            AssertAreEqual(Angle.FromRadians(x), rotation.AroundX);
            AssertAreEqual(Angle.Zero, rotation.AroundY);
            AssertAreEqual(Angle.Zero, rotation.AroundZ);
        }

        [TestMethod]
        public void FromRadians_ByFloats_ReturnsExpectedRotationInstance()
        {
            var x = RandomRadians();
            var rotation = Rotation3D.FromRadians(x, 0, 0);

            AssertAreEqual(Angle.FromRadians(x), rotation.AroundX);
            AssertAreEqual(Angle.Zero, rotation.AroundY);
            AssertAreEqual(Angle.Zero, rotation.AroundZ);
        }

        #endregion

        #region [====== FromDegrees ======]

        [TestMethod]
        public void FromDegrees_ByVector_ReturnsExpectedRotationInstance()
        {
            var x = RandomDegrees();            
            var rotation = Rotation3D.FromDegrees(new Vector3(x, 0, 0));

            AssertAreEqual(Angle.FromDegrees(x), rotation.AroundX);
            AssertAreEqual(Angle.Zero, rotation.AroundY);
            AssertAreEqual(Angle.Zero, rotation.AroundZ);
        }

        [TestMethod]
        public void FromDegrees_ByFloats_ReturnsExpectedRotationInstance()
        {
            var x = RandomDegrees();
            var rotation = Rotation3D.FromDegrees(x, 0, 0);

            AssertAreEqual(Angle.FromDegrees(x), rotation.AroundX);
            AssertAreEqual(Angle.Zero, rotation.AroundY);
            AssertAreEqual(Angle.Zero, rotation.AroundZ);
        }

        #endregion

        #region [====== RotationMatrix ======]

        [TestMethod]
        public void RotationMatrix_ReturnsIdentityMatrix_IfRotationIsZero()
        {
            var rotation = Rotation3D.NoRotation;
            var matrix = rotation.RotationMatrix;

            Assert.AreEqual(Matrix.Identity, matrix);
        }

        [TestMethod]
        public void RotationMatrix_ReturnsIdentityMatrix_IfRotationIsFullCircle()
        {
            var rotation = Rotation3D.FromDegrees(360, 360, 360);
            var matrix = rotation.RotationMatrix;

            Assert.AreEqual(Matrix.Identity, matrix);
        }

        [TestMethod]
        public void ToRationMatrix_ReturnsExpectedMatrix_IfRotationXIs90Degrees()
        {
            var rotation = Rotation3D.FromDegrees(90, 0, 0);
            var matrix = rotation.RotationMatrix;

            AssertAreEqual(1, matrix.M11);
            AssertAreEqual(0, matrix.M12);
            AssertAreEqual(0, matrix.M13);
            AssertAreEqual(0, matrix.M14);

            AssertAreEqual(0, matrix.M21);
            AssertAreEqual(0, matrix.M22);     //  cos(90)
            AssertAreEqual(1, matrix.M23);     //  sin(90)
            AssertAreEqual(0, matrix.M24);

            AssertAreEqual(0, matrix.M31);
            AssertAreEqual(-1, matrix.M32);    // -sin(90)
            AssertAreEqual(0, matrix.M33);     //  cos(90)
            AssertAreEqual(0, matrix.M34);

            AssertAreEqual(0, matrix.M41);
            AssertAreEqual(0, matrix.M42);
            AssertAreEqual(0, matrix.M43);
            AssertAreEqual(1, matrix.M44);
        }

        [TestMethod]
        public void ToRationMatrix_ReturnsExpectedMatrix_IfRotationXIs45Degrees()
        {
            var rotation = Rotation3D.FromDegrees(45, 0, 0);
            var matrix = rotation.RotationMatrix;

            AssertAreEqual(1, matrix.M11);
            AssertAreEqual(0, matrix.M12);
            AssertAreEqual(0, matrix.M13);
            AssertAreEqual(0, matrix.M14);

            AssertAreEqual(0, matrix.M21);
            AssertAreEqual(0.7071f, matrix.M22);     //  cos(45)
            AssertAreEqual(0.7071f, matrix.M23);     //  sin(45)
            AssertAreEqual(0, matrix.M24);

            AssertAreEqual(0, matrix.M31);
            AssertAreEqual(-0.7071f, matrix.M32);    // -sin(45)
            AssertAreEqual(0.7071f, matrix.M33);     //  cos(45)
            AssertAreEqual(0, matrix.M34);

            AssertAreEqual(0, matrix.M41);
            AssertAreEqual(0, matrix.M42);
            AssertAreEqual(0, matrix.M43);
            AssertAreEqual(1, matrix.M44);
        }

        [TestMethod]
        public void ToRationMatrix_ReturnsExpectedMatrix_IfRotationYIs90Degrees()
        {
            var rotation = Rotation3D.FromDegrees(0, 90, 0);
            var matrix = rotation.RotationMatrix;

            AssertAreEqual(0, matrix.M11);     //  cos(90)
            AssertAreEqual(0, matrix.M12);
            AssertAreEqual(-1, matrix.M13);    // -sin(90)
            AssertAreEqual(0, matrix.M14);

            AssertAreEqual(0, matrix.M21);
            AssertAreEqual(1, matrix.M22);
            AssertAreEqual(0, matrix.M23);
            AssertAreEqual(0, matrix.M24);

            AssertAreEqual(1, matrix.M31);     //  sin(90)
            AssertAreEqual(0, matrix.M32);
            AssertAreEqual(0, matrix.M33);     //  cos(90)
            AssertAreEqual(0, matrix.M34);

            AssertAreEqual(0, matrix.M41);
            AssertAreEqual(0, matrix.M42);
            AssertAreEqual(0, matrix.M43);
            AssertAreEqual(1, matrix.M44);
        }

        [TestMethod]
        public void ToRationMatrix_ReturnsExpectedMatrix_IfRotationYIs45Degrees()
        {
            var rotation = Rotation3D.FromDegrees(0, 45, 0);
            var matrix = rotation.RotationMatrix;

            AssertAreEqual(0.7071f, matrix.M11);     //  cos(45)
            AssertAreEqual(0, matrix.M12);
            AssertAreEqual(-0.7071f, matrix.M13);    // -sin(45)
            AssertAreEqual(0, matrix.M14);

            AssertAreEqual(0, matrix.M21);
            AssertAreEqual(1, matrix.M22);
            AssertAreEqual(0, matrix.M23);
            AssertAreEqual(0, matrix.M24);

            AssertAreEqual(0.7071f, matrix.M31);     //  sin(45)
            AssertAreEqual(0, matrix.M32);
            AssertAreEqual(0.7071f, matrix.M33);     //  cos(45)
            AssertAreEqual(0, matrix.M34);

            AssertAreEqual(0, matrix.M41);
            AssertAreEqual(0, matrix.M42);
            AssertAreEqual(0, matrix.M43);
            AssertAreEqual(1, matrix.M44);
        }

        [TestMethod]
        public void ToRationMatrix_ReturnsExpectedMatrix_IfRotationZIs90Degrees()
        {
            var rotation = Rotation3D.FromDegrees(0, 0, 90);
            var matrix = rotation.RotationMatrix;

            AssertAreEqual(0, matrix.M11);     //  cos(90)
            AssertAreEqual(1, matrix.M12);     //  sin(90)
            AssertAreEqual(0, matrix.M13);
            AssertAreEqual(0, matrix.M14);

            AssertAreEqual(-1, matrix.M21);    // -sin(90)
            AssertAreEqual(0, matrix.M22);     //  cos(90)
            AssertAreEqual(0, matrix.M23);
            AssertAreEqual(0, matrix.M24);

            AssertAreEqual(0, matrix.M31);
            AssertAreEqual(0, matrix.M32);
            AssertAreEqual(1, matrix.M33);
            AssertAreEqual(0, matrix.M34);

            AssertAreEqual(0, matrix.M41);
            AssertAreEqual(0, matrix.M42);
            AssertAreEqual(0, matrix.M43);
            AssertAreEqual(1, matrix.M44);
        }

        [TestMethod]
        public void ToRationMatrix_ReturnsExpectedMatrix_IfRotationZIs45Degrees()
        {
            var rotation = Rotation3D.FromDegrees(0, 0, 45);
            var matrix = rotation.RotationMatrix;

            AssertAreEqual(0.7071f, matrix.M11); //  cos(45)
            AssertAreEqual(0.7071f, matrix.M12); //  sin(45)
            AssertAreEqual(0, matrix.M13);
            AssertAreEqual(0, matrix.M14);

            AssertAreEqual(-0.7071f, matrix.M21); // -sin(45)
            AssertAreEqual(0.7071f, matrix.M22);  //  cos(45)
            AssertAreEqual(0, matrix.M23);
            AssertAreEqual(0, matrix.M24);

            AssertAreEqual(0, matrix.M31);
            AssertAreEqual(0, matrix.M32);
            AssertAreEqual(1, matrix.M33);
            AssertAreEqual(0, matrix.M34);

            AssertAreEqual(0, matrix.M41);
            AssertAreEqual(0, matrix.M42);
            AssertAreEqual(0, matrix.M43);
            AssertAreEqual(1, matrix.M44);
        }

        #endregion

        #region [====== Multiply ======]

        [TestMethod]
        public void Multiply_ReturnsExpectedRotation_IfBothRotationsAreZeroAngles()
        {
            var left = Rotation3D.FromAngleAroundAxis(Vector3.UnitX, Angle.Zero);
            var right = Rotation3D.FromAngleAroundAxis(Vector3.UnitX, Angle.Zero);
            var rotation = left * right;

            AssertAreEqual(Angle.Zero, rotation.AroundX);
            AssertAreEqual(Angle.Zero, rotation.AroundY);
            AssertAreEqual(Angle.Zero, rotation.AroundZ);
        }

        [TestMethod]
        public void Multiply_ReturnsExpectedRotation_IfLeftAngleIsZero_And_RightAngleIsNotZero_AroundSameAxis()
        {
            var left = Rotation3D.FromAngleAroundAxis(Vector3.UnitX, Angle.Zero);
            var right = Rotation3D.FromAngleAroundAxis(Vector3.UnitX, Angle.FromDegrees(100));
            var rotation = left * right;

            AssertAreEqual(Angle.FromDegrees(100), rotation.AroundX);
            AssertAreEqual(Angle.Zero, rotation.AroundY);
            AssertAreEqual(Angle.Zero, rotation.AroundZ);
        }

        [TestMethod]
        public void Multiply_ReturnsExpectedRotation_IfLeftAngleNotIsZero_And_RightAngleIsZero_AroundSameAxis()
        {
            var left = Rotation3D.FromAngleAroundAxis(Vector3.UnitX, Angle.FromDegrees(100));
            var right = Rotation3D.FromAngleAroundAxis(Vector3.UnitX, Angle.Zero);
            var rotation = left * right;

            AssertAreEqual(Angle.FromDegrees(100), rotation.AroundX);
            AssertAreEqual(Angle.Zero, rotation.AroundY);
            AssertAreEqual(Angle.Zero, rotation.AroundZ);
        }

        [TestMethod]
        public void Multiply_ReturnsExpectedRotation_IfLeftAngleNotIsZero_And_RightAngleIsNotZero_AroundSameAxis()
        {
            var left = Rotation3D.FromAngleAroundAxis(Vector3.UnitX, Angle.FromDegrees(50));
            var right = Rotation3D.FromAngleAroundAxis(Vector3.UnitX, Angle.FromDegrees(40));
            var rotation = left * right;

            AssertAreEqual(Angle.FromDegrees(90), rotation.AroundX);
            AssertAreEqual(Angle.Zero, rotation.AroundY);
            AssertAreEqual(Angle.Zero, rotation.AroundZ);
        }

        [TestMethod]
        public void Multiply_ReturnsExpectedRotation_IfLeftAngleIsPositive_And_RightAngleIsNegative_AroundSameAxis()
        {
            var left = Rotation3D.FromAngleAroundAxis(Vector3.UnitX, Angle.FromDegrees(-50));
            var right = Rotation3D.FromAngleAroundAxis(Vector3.UnitX, Angle.FromDegrees(40));
            var rotation = left * right;

            AssertAreEqual(Angle.FromDegrees(-10), rotation.AroundX);
            AssertAreEqual(Angle.Zero, rotation.AroundY);
            AssertAreEqual(Angle.Zero, rotation.AroundZ);
        }

        [TestMethod]
        public void Multiply_ReturnsExpectedRotation_IfMultipleRotationsArePerformed_AroundDifferentAxis()
        {
            var rotation0 = Rotation3D.FromAngleAroundAxis(Vector3.UnitX, Angle.FromDegrees(90));
            var rotation1 = Rotation3D.FromAngleAroundAxis(Vector3.UnitY, Angle.FromDegrees(180));
            var rotation2 = Rotation3D.FromAngleAroundAxis(Vector3.UnitZ, Angle.FromDegrees(180));            
            var rotation = rotation0 * rotation1 * rotation2;

            AssertAreEqual(Angle.FromDegrees(-90), rotation.AroundX);
            AssertAreEqual(Angle.Zero, rotation.AroundY);
            AssertAreEqual(Angle.Zero, rotation.AroundZ);
        }

        #endregion

        private static void AssertAreEqual(Angle left, Angle right)
        {
            AssertAreEqual(left.ToDegrees(), right.ToDegrees());
        }

        private static void AssertAreEqual(float left, float right)
        {
            Assert.AreEqual(Round(left), Round(right));
        }

        private static double Round(float value)
        {
            return Math.Round(value, 1);
        }

        private static readonly Random _Random = new Random();

        private static int RandomDegrees()
        {
            lock (_Random)
            {
                return _Random.Next(0, 180);
            }
        }

        private static float RandomRadians()
        {
            lock (_Random)
            {
                return (float) (_Random.NextDouble() * Math.PI);
            }
        }
    }
}
