using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.SharpDX
{
    [TestClass]
    public sealed class ScaleFactorTest
    {
        #region [====== ComparableTestSuite ======]

        private static readonly ComparableTestSuite<ScaleFactor> _ComparableTestSuite = new ComparableValueTypeTestSuite<ScaleFactor>(new MSTestEngine());

        [TestMethod]
        public void ComparableTestSuite_ExecutesSuccesfully()
        {
            _ComparableTestSuite.Execute(new ComparableTestParameters<ScaleFactor>()
            {
                Instance = new ScaleFactor(0.5f),
                EqualInstance = new ScaleFactor(0.5f),
                LargerInstance = new ScaleFactor(3.4f)
            });
        }

        #endregion

        #region [====== Constructor ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Constructor_Throws_IfScaleFactorIsLessThanZero()
        {
            new ScaleFactor(-0.5f);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Constructor_Throws_IfScaleFactorIsZero()
        {
            new ScaleFactor(0);
        }

        [TestMethod]
        public void Constructor_ReturnsExpectedScaleFactor_IfScaleFactorIsGreaterThanZero()
        {
            var scaleFactorValue = RandomScaleFactor();
            var scaleFactor = new ScaleFactor(scaleFactorValue);

            Assert.AreEqual(scaleFactorValue, scaleFactor.ToSingle());
        }

        #endregion

        #region [====== ToString ======]

        [TestMethod]
        public void ToString_ReturnsExpectedStringRepresentation()
        {
            Assert.AreEqual("1", ScaleFactor.DefaultScale.ToString());
        }

        #endregion

        #region [====== Multiply ======]

        [TestMethod]
        public void Multiply_ReturnsExpectedScaleFactor_IfLeftAndRightAreDefaultScaleFactor()
        {
            Assert.AreEqual(ScaleFactor.DefaultScale, ScaleFactor.DefaultScale * ScaleFactor.DefaultScale);
        }

        [TestMethod]
        public void Multiple_ReturnsExpectedScaleFactor_IfLeftAndRightAreNotDefaultScaleFactor()
        {
            var leftValue = RandomScaleFactor();
            var left = new ScaleFactor(leftValue);

            var rightValue = RandomScaleFactor();
            var right = new ScaleFactor(rightValue);

            var resultingValue = leftValue * rightValue;
            var result = left * right;

            Assert.AreEqual(resultingValue, result.ToSingle());
        }

        #endregion

        private static readonly Random _Random = new Random();

        internal static float RandomScaleFactor()
        {
            lock (_Random)
            {
                return _Random.Next(1, 100);
            }
        }
    }
}
