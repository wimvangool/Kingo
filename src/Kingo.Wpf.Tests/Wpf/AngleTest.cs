using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Wpf
{
    [TestClass]
    public sealed class AngleTest
    {
        private const double _Zero = 0;

        private const double _HalfPI = 0.5 * Math.PI;
        private const double _PI = Math.PI;
        private const double _TwoPI = 2 * Math.PI;
        private const double _ThreePI = 3 * Math.PI;

        #region [====== ComparableTestSuite ======]

        private static readonly ComparableTestSuite<Angle> _ComparableTestSuite = new ComparableValueTypeTestSuite<Angle>(new MSTestEngine());

        [TestMethod]
        public void ComparableTestSuite_ExecutesSuccesfully()
        {
            _ComparableTestSuite.Execute(new ComparableTestParameters<Angle>()
            {
                Instance = Angle.FromDegrees(121),
                EqualInstance = Angle.FromDegrees(121),
                LargerInstance = Angle.FromDegrees(122)
            });
        }

        #endregion

        #region [====== FromRadians ======]

        [TestMethod]
        public void FromRadians_CreatesExpectedAngle_IfRadiansIsZero()
        {
            var angle = Angle.FromRadians(_Zero);

            Assert.AreEqual(_Zero, angle.ToRadians());
            Assert.AreEqual(_Zero, angle.ToDegrees());
            Assert.AreEqual("0 rad. (0˚)", angle.ToString());
        }

        [TestMethod]
        public void FromRadians_CreatesExpectedAngle_IfRadiansIsHalfPI()
        {
            var angle = Angle.FromRadians(_HalfPI);

            Assert.AreEqual(_HalfPI, angle.ToRadians());
            Assert.AreEqual(90, angle.ToDegrees());
            Assert.AreEqual("0.5π rad. (90˚)", angle.ToString());
        }

        [TestMethod]
        public void FromRadians_CreatesExpectedAngle_IfRadiansIsMinusHalfPI()
        {            
            var angle = Angle.FromRadians(-_HalfPI);

            Assert.AreEqual(- _HalfPI, angle.ToRadians());
            Assert.AreEqual(-90, angle.ToDegrees());
            Assert.AreEqual("-0.5π rad. (-90˚)", angle.ToString());
        }

        [TestMethod]
        public void FromRadians_CreatesExpectedAngle_IfRadiansIsPI()
        {            
            var angle = Angle.FromRadians(_PI);

            Assert.AreEqual(_PI, angle.ToRadians());
            Assert.AreEqual(180, angle.ToDegrees());
            Assert.AreEqual("π rad. (180˚)", angle.ToString());
        }

        [TestMethod]
        public void FromRadians_CreatesExpectedAngle_IfRadiansIsMinusPI()
        {
            var angle = Angle.FromRadians(-_PI);

            Assert.AreEqual(-_PI, angle.ToRadians());
            Assert.AreEqual(-180, angle.ToDegrees());
            Assert.AreEqual("-π rad. (-180˚)", angle.ToString());
        }

        [TestMethod]
        public void FromRadians_CreatesExpectedAngle_IfRadiansIsTwoPI()
        {
            var angle = Angle.FromRadians(_TwoPI);

            Assert.AreEqual(_TwoPI, angle.ToRadians());
            Assert.AreEqual(360, angle.ToDegrees());
            Assert.AreEqual("2π rad. (360˚)", angle.ToString());
        }

        [TestMethod]
        public void FromRadians_CreatesExpectedAngle_IfRadiansIsMinusTwoPI()
        {
            var angle = Angle.FromRadians(-_TwoPI);

            Assert.AreEqual(-_TwoPI, angle.ToRadians());
            Assert.AreEqual(-360, angle.ToDegrees());
            Assert.AreEqual("-2π rad. (-360˚)", angle.ToString());
        }

        [TestMethod]
        public void FromRadians_CreatesExpectedAngle_IfRadiansIsThreePI()
        {
            var angle = Angle.FromRadians(_ThreePI);

            Assert.AreEqual(_ThreePI, angle.ToRadians());
            Assert.AreEqual(540, angle.ToDegrees());
            Assert.AreEqual("3π rad. (540˚)", angle.ToString());
        }

        [TestMethod]
        public void FromRadians_CreatesExpectedAngle_IfRadiansIsMinusThreePI()
        {
            var angle = Angle.FromRadians(-_ThreePI);

            Assert.AreEqual(-_ThreePI, angle.ToRadians());
            Assert.AreEqual(-540, angle.ToDegrees());
            Assert.AreEqual("-3π rad. (-540˚)", angle.ToString());
        }

        #endregion

        #region [====== FromDegrees ======]

        [TestMethod]
        public void FromDegrees_CreatesExpectedAngle_IfDegreesIsZero()
        {
            var angle = Angle.FromDegrees(_Zero);

            Assert.AreEqual(_Zero, angle.ToRadians());
            Assert.AreEqual(_Zero, angle.ToDegrees());
            Assert.AreEqual("0 rad. (0˚)", angle.ToString());
        }

        [TestMethod]
        public void FromDegrees_CreatesExpectedAngle_IfDegreesIsNinety()
        {
            const double degrees = 90;

            var angle = Angle.FromDegrees(degrees);

            Assert.AreEqual(_HalfPI, angle.ToRadians());
            Assert.AreEqual(degrees, angle.ToDegrees());
            Assert.AreEqual("0.5π rad. (90˚)", angle.ToString());
        }

        [TestMethod]
        public void FromDegrees_CreatesExpectedAngle_IfDegreesIsMinusNinety()
        {
            const double degrees = -90;

            var angle = Angle.FromDegrees(degrees);

            Assert.AreEqual(-_HalfPI, angle.ToRadians());
            Assert.AreEqual(degrees, angle.ToDegrees());
            Assert.AreEqual("-0.5π rad. (-90˚)", angle.ToString());
        }

        [TestMethod]
        public void FromDegrees_CreatesExpectedAngle_IfDegreesIsOneHundredEighty()
        {
            const double degrees = 180;

            var angle = Angle.FromDegrees(degrees);

            Assert.AreEqual(_PI, angle.ToRadians());
            Assert.AreEqual(degrees, angle.ToDegrees());
            Assert.AreEqual("π rad. (180˚)", angle.ToString());
        }

        [TestMethod]
        public void FromDegrees_CreatesExpectedAngle_IfDegreesIsMinusOneHundredEighty()
        {
            const double degrees = -180;

            var angle = Angle.FromDegrees(degrees);

            Assert.AreEqual(-_PI, angle.ToRadians());
            Assert.AreEqual(degrees, angle.ToDegrees());
            Assert.AreEqual("-π rad. (-180˚)", angle.ToString());
        }

        [TestMethod]
        public void FromDegrees_CreatesExpectedAngle_IfDegreesIsThreeHundredSixty()
        {
            const double degrees = 360;

            var angle = Angle.FromDegrees(degrees);

            Assert.AreEqual(_TwoPI, angle.ToRadians());
            Assert.AreEqual(degrees, angle.ToDegrees());
            Assert.AreEqual("2π rad. (360˚)", angle.ToString());
        }

        [TestMethod]
        public void FromDegrees_CreatesExpectedAngle_IfDegreesIsMinusThreeHundredSixty()
        {
            const double degrees = -360;

            var angle = Angle.FromDegrees(degrees);

            Assert.AreEqual(-_TwoPI, angle.ToRadians());
            Assert.AreEqual(degrees, angle.ToDegrees());
            Assert.AreEqual("-2π rad. (-360˚)", angle.ToString());
        }

        [TestMethod]
        public void FromDegrees_CreatesExpectedAngle_IfDegreesIsFiveHundredFourty()
        {
            const double degrees = 540;

            var angle = Angle.FromDegrees(degrees);

            Assert.AreEqual(_ThreePI, angle.ToRadians());
            Assert.AreEqual(degrees, angle.ToDegrees());
            Assert.AreEqual("3π rad. (540˚)", angle.ToString());
        }

        [TestMethod]
        public void FromDegrees_CreatesExpectedAngle_IfDegreesIsMinusFiveHundredFourty()
        {
            const double degrees = -540;

            var angle = Angle.FromDegrees(degrees);

            Assert.AreEqual(-_ThreePI, angle.ToRadians());
            Assert.AreEqual(degrees, angle.ToDegrees());
            Assert.AreEqual("-3π rad. (-540˚)", angle.ToString());
        }

        #endregion

        #region [====== Normalize ======]

        [TestMethod]
        public void Normalize_ReturnsExpectedAngle_IfAngleIsZeroDegrees()
        {
            var x = Angle.FromDegrees(_Zero);
            var y = x.Normalize();

            Assert.AreEqual(x, y);
        }

        [TestMethod]
        public void Normalize_ReturnsExpectedAngle_IfAngleIsNinetyDegrees()
        {
            var x = Angle.FromDegrees(90);
            var y = x.Normalize();

            Assert.AreEqual(x, y);
        }

        [TestMethod]
        public void Normalize_ReturnsExpectedAngle_IfAngleIsMinusNinetyDegrees()
        {
            var x = Angle.FromDegrees(-90);
            var y = x.Normalize();

            Assert.AreEqual(x, y);
        }

        [TestMethod]
        public void Normalize_ReturnsExpectedAngle_IfAngleIsThreeHundredSixtyDegrees()
        {
            var x = Angle.FromDegrees(360);
            var y = x.Normalize();

            Assert.AreEqual(Angle.Zero, y);
        }

        [TestMethod]
        public void Normalize_ReturnsExpectedAngle_IfAngleIsMinusThreeHundredSixtyDegrees()
        {
            var x = Angle.FromDegrees(-360);
            var y = x.Normalize();

            Assert.AreEqual(Angle.Zero, y);
        }

        [TestMethod]
        public void Normalize_ReturnsExpectedAngle_IfAngleIsFiveHundredFourtyDegrees()
        {
            var x = Angle.FromDegrees(540);
            var y = x.Normalize();

            Assert.AreEqual(Angle.FromDegrees(180), y);
        }

        [TestMethod]
        public void Normalize_ReturnsExpectedAngle_IfAngleIsMinusFiveHundredFourtyDegrees()
        {
            var x = Angle.FromDegrees(-540);
            var y = x.Normalize();

            Assert.AreEqual(Angle.FromDegrees(-180), y);
        }

        #endregion

        #region [====== Add & Subtract ======]

        [TestMethod]
        public void Add_ReturnsExpectedAngle_IfLeftAndRightAreZero()
        {
            var x = Angle.Zero;
            var y = Angle.Zero;

            Assert.AreEqual(Angle.Zero, x + y);
        }

        [TestMethod]
        public void Add_ReturnsExpectedAngle_IfLeftIs45_And_RightIs90()
        {
            var x = Angle.FromDegrees(45);
            var y = Angle.FromDegrees(90);

            Assert.AreEqual(Angle.FromDegrees(135), x + y);
        }

        [TestMethod]
        public void Add_ReturnsExpectedAngle_IfLeftIsMinus45_AndRightIs90()
        {
            var x = Angle.FromDegrees(-45);
            var y = Angle.FromDegrees(90);

            Assert.AreEqual(Angle.FromDegrees(45), x + y);
        }

        [TestMethod]
        public void Subtract_ReturnsExpectedAngle_IfLeftAndRightAreZero()
        {
            var x = Angle.Zero;
            var y = Angle.Zero;

            Assert.AreEqual(Angle.Zero, x - y);
        }

        [TestMethod]
        public void Subtract_ReturnsExpectedAngle_IfLeftIs45_And_RightIs90()
        {
            var x = Angle.FromDegrees(45);
            var y = Angle.FromDegrees(90);

            Assert.AreEqual(Angle.FromDegrees(-45), x - y);
        }

        [TestMethod]
        public void Subtract_ReturnsExpectedAngle_IfLeftIsMinus45_AndRightIs90()
        {
            var x = Angle.FromDegrees(-45);
            var y = Angle.FromDegrees(90);

            Assert.AreEqual(Angle.FromDegrees(-135), x - y);
        }

        #endregion

        #region [====== Framework Functions ======]

        public static void AssertAreEqual(Angle left, Angle right)
        {
            AssertAreEqual(left.ToDegrees(), right.ToDegrees());
        }

        public static void AssertAreEqual(double left, double right)
        {
            Assert.AreEqual(Round(left), Round(right));
        }

        private static double Round(double value)
        {
            return Math.Round(value, 1);
        }

        private static readonly Random _Random = new Random();

        public static int RandomDegrees()
        {
            lock (_Random)
            {
                return _Random.Next(0, 180);
            }
        }

        public static double RandomRadians()
        {
            lock (_Random)
            {
                return (double) (_Random.NextDouble() * Math.PI);
            }
        }

        #endregion
    }
}
