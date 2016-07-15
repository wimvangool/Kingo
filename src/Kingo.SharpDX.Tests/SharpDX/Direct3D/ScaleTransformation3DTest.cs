using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.SharpDX.Direct3D
{
    [TestClass]
    public sealed class ScaleTransformation3DTest
    {
        #region [====== Constructor (By Single Factor) ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Constructor_BySingleFloatFactor_Throws_IfScaleFactorIsLessThanZero()
        {
            new ScaleTransformation3D(-1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Constructor_BySingleFloatFactor_Throws_IfScaleFactorIsZero()
        {
            new ScaleTransformation3D(0);
        }

        [TestMethod]        
        public void Constructor_ReturnsExpectedTransformation_IfScaleFactorIsGreaterThanZero()
        {
            var scaleFactorValue = ScaleFactorTest.RandomScaleFactor();
            var scaleFactor = new ScaleTransformation3D(scaleFactorValue);

            Assert.AreEqual(scaleFactorValue, scaleFactor.X.ToSingle());
            Assert.AreEqual(scaleFactorValue, scaleFactor.Y.ToSingle());
            Assert.AreEqual(scaleFactorValue, scaleFactor.Z.ToSingle());
        }

        #endregion
    }
}
