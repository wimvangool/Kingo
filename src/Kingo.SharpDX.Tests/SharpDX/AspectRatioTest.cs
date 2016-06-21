using System;
using System.Drawing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.SharpDX
{
    [TestClass]
    public sealed class AspectRatioTest
    {
        #region [======= ComparableTestSuite ======]

        private static readonly ComparableReferenceTypeTestSuite<AspectRatio> _ComparableTestSuite = new ComparableReferenceTypeTestSuite<AspectRatio>(new MSTestEngine());
            
        [TestMethod]
        public void ComparableTestSuite_ExecutesSuccesfully()
        {
            _ComparableTestSuite.Execute(new ComparableTestParameters<AspectRatio>()
            {
                Instance = AspectRatio.FromScreenSize(800, 600),
                EqualInstance = AspectRatio.FromScreenSize(1600, 1200),
                LargerInstance = AspectRatio.FromScreenSize(10, 5)
            });
        }

        #endregion

        #region [====== FromScreenSize (By Size) ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void FromScreenSize_BySize_Throws_IfWidthIsZero()
        {
            AspectRatio.FromScreenSize(new Size(0, 600));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void FromScreenSize_BySize_Throws_IfHeightIsZero()
        {
            AspectRatio.FromScreenSize(new Size(800, 0));
        }

        [TestMethod]
        public void FromScreenSize_BySize_ReturnsExpectedAspectRatio_IfWidthAndHeightAreGreaterThanZero()
        {
            Assert.AreEqual(2f, AspectRatio.FromScreenSize(new Size(100, 50)).ToSingle());
        }

        #endregion

        #region [====== FromScreenSize (By Floats) ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void FromScreenSize_ByFloats_Throws_IfWidthIsLessThanZero()
        {
            AspectRatio.FromScreenSize(-1, 600);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void FromScreenSize_ByFloats_Throws_IfWidthIsZero()
        {
            AspectRatio.FromScreenSize(0, 600);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void FromScreenSize_ByFloats_Throws_IfHeightIsLesThanZero()
        {
            AspectRatio.FromScreenSize(800, -1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void FromScreenSize_ByFloats_Throws_IfHeightIsZero()
        {
            AspectRatio.FromScreenSize(800, 0);
        }

        [TestMethod]
        public void FromScreenSize_ByFloats_ReturnsExpectedAspectRatio_IfWidthAndHeightAreGreaterThanZero()
        {
            Assert.AreEqual(0.5f, AspectRatio.FromScreenSize(50, 100).ToSingle());
        }

        #endregion

        #region [====== FromScreenSize (By Length) ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void FromScreenSize_ByLength_Throws_IfWidthIsZero()
        {
            AspectRatio.FromScreenSize(Length.Zero, new Length(600));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void FromScreenSize_ByLength_Throws_IfHeightIsZero()
        {
            AspectRatio.FromScreenSize(new Length(800), Length.Zero);
        }

        [TestMethod]
        public void FromScreenSize_ByLength_ReturnsExpectedAspectRatio_IfWidthAndHeightAreGreaterThanZero()
        {
            Assert.AreEqual(4f, AspectRatio.FromScreenSize(new Length(400), new Length(100)).ToSingle());
        }

        #endregion
    }
}
