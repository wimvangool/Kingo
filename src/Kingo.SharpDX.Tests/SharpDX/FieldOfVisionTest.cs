using System;
using Kingo.SharpDX.Direct3D;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.SharpDX
{
    [TestClass]
    public sealed class FieldOfVisionTest
    {
        #region [====== ComparableTestSuite ======]

        private static readonly ComparableTestSuite<FieldOfVision> _ComparableTestSuite = new ComparableReferenceTypeTestSuite<FieldOfVision>(new MSTestEngine());

        [TestMethod]
        public void ComparableTestSuite_ExecutesSuccesfully()
        {
            _ComparableTestSuite.Execute(new ComparableTestParameters<FieldOfVision>()
            {
                Instance = FieldOfVision.FromDegrees(56),
                EqualInstance = FieldOfVision.FromDegrees(56),
                LargerInstance = FieldOfVision.FromDegrees(57)
            });
        }

        #endregion

        #region [====== FromRadians ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void FromRadians_Throws_IfRadiansIsLessThanZero()
        {
            FieldOfVision.FromRadians(-0.001f);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void FromRadians_Throws_IfRadiansIsZero()
        {
            FieldOfVision.FromRadians(0);
        }        

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void FromRadians_Throws_IfRadiansIsPI()
        {
            FieldOfVision.FromRadians((float) Math.PI);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void FromRadians_Throws_IfRadiansIsGreaterThanPI()
        {
            FieldOfVision.FromRadians((float) (1.001 * Math.PI));
        }

        [TestMethod]
        public void FromRadians_ReturnsExpectedAngle_IfRadiansIsBetweenZeroAndPI()
        {
            const float radians = (float) (0.25 * Math.PI);

            var fov = FieldOfVision.FromRadians(radians);

            Assert.AreEqual(radians, fov.ToRadians());
            Assert.AreEqual(45, fov.ToDegrees());
        }

        #endregion

        #region [====== FromDegrees ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void FromDegrees_Throws_IfDegreesIsLessThanZero()
        {
            FieldOfVision.FromDegrees(-0.001f);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void FromDegrees_Throws_IfDegreesIsZero()
        {
            FieldOfVision.FromDegrees(0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void FromDegrees_Throws_IfDegreesIs180()
        {
            FieldOfVision.FromDegrees(180);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void FromDegrees_Throws_IfDegreesIsGreaterThan180()
        {
            FieldOfVision.FromDegrees(180.001f);
        }

        [TestMethod]
        public void FromDegrees_ReturnsExpectedAngle_IfRadiansIsBetweenZeroAnd180()
        {
            const float degrees = 135;

            var fov = FieldOfVision.FromDegrees(degrees);

            Assert.AreEqual(Math.Round(0.75 * Math.PI, 4), Math.Round(fov.ToRadians(), 4));
            Assert.AreEqual(degrees, fov.ToDegrees());
        }

        #endregion
    }
}
