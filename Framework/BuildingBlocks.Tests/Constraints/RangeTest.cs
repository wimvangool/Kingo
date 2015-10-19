using System;
using Kingo.BuildingBlocks.Clocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.BuildingBlocks.Constraints
{
    [TestClass]
    public sealed class RangeTest
    {
        #region [====== Constructor ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Constructor_Throws_IfRangeIsNotValid()
        {
            var left = RandomValue();
            var right = left - 1;

            new Range<int>(left, right);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Constructor_Throws_IfLeftAndRightAreEqual_And_OptionsIsLeftExclusive()
        {
            var value = RandomValue();

            new Range<int>(value, value, RangeOptions.LeftExclusive);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Constructor_Throws_IfLeftAndRightAreEqual_And_OptionsIsRightExclusive()
        {
            var value = RandomValue();

            new Range<int>(value, value, RangeOptions.RightExclusive);
        }

        [TestMethod]
        public void Constructor_AcceptsValues_IfLeftAndRightAreEqual_And_OptionsIsNone()
        {
            var value = RandomValue();

            new Range<int>(value, value);
        }

        #endregion

        #region [====== Boundary Options ======]

        [TestMethod]
        public void BoundaryOptions_AreSetCorrectly_IfOptionsIsNone()
        {
            var range = new Range<int>();

            Assert.IsTrue(range.IsLeftInclusive);
            Assert.IsFalse(range.IsLeftExclusive);
            Assert.IsTrue(range.IsRightInclusive);
            Assert.IsFalse(range.IsRightExclusive);
        }        

        [TestMethod]
        public void BoundaryOptions_AreSetCorrectly_IfOptionIsLeftExclusive()
        {
            var left = RandomValue();
            var right = left + 1;
            var range = new Range<int>(left, right, RangeOptions.LeftExclusive);

            Assert.IsFalse(range.IsLeftInclusive);
            Assert.IsTrue(range.IsLeftExclusive);
            Assert.IsTrue(range.IsRightInclusive);
            Assert.IsFalse(range.IsRightExclusive);
        }

        [TestMethod]
        public void BoundaryOptions_AreSetCorrectly_IfOptionIsRightExclusive()
        {
            var left = RandomValue();
            var right = left + 1;
            var range = new Range<int>(left, right, RangeOptions.RightExclusive);

            Assert.IsTrue(range.IsLeftInclusive);
            Assert.IsFalse(range.IsLeftExclusive);
            Assert.IsFalse(range.IsRightInclusive);
            Assert.IsTrue(range.IsRightExclusive);
        }

        [TestMethod]
        public void BoundaryOptions_AreSetCorrectly_IfOptionIsAllExclusive()
        {
            var left = RandomValue();
            var right = left + 1;
            var range = new Range<int>(left, right, RangeOptions.AllExclusive);

            Assert.IsFalse(range.IsLeftInclusive);
            Assert.IsTrue(range.IsLeftExclusive);
            Assert.IsFalse(range.IsRightInclusive);
            Assert.IsTrue(range.IsRightExclusive);
        }

        #endregion

        #region [====== Contains ======]

        [TestMethod]
        public void Contains_ReturnsFalse_IsValueIsSmallerThanLeftBoundary()
        {
            var left = RandomValue();
            var right = left + 10;
            var range = new Range<int>(left, right);

            Assert.IsFalse(range.Contains(left - 1));
        }

        [TestMethod]
        public void Contains_ReturnsTrue_IsValueIsEqualToLeftInclusiveBoundary()
        {
            var left = RandomValue();
            var right = left + 10;
            var range = new Range<int>(left, right);

            Assert.IsTrue(range.Contains(left));
        }

        [TestMethod]
        public void Contains_ReturnsFalse_IsValueIsEqualToLeftExclusiveBoundary()
        {
            var left = RandomValue();
            var right = left + 10;
            var range = new Range<int>(left, right, RangeOptions.LeftExclusive);

            Assert.IsFalse(range.Contains(left));
        }

        [TestMethod]
        public void Contains_ReturnsFalse_IsValueIsGreaterThanRightBoundary()
        {
            var left = RandomValue();
            var right = left + 10;
            var range = new Range<int>(left, right);

            Assert.IsFalse(range.Contains(right + 1));
        }

        [TestMethod]
        public void Contains_ReturnsTrue_IsValueIsEqualToRightInclusiveBoundary()
        {
            var left = RandomValue();
            var right = left + 10;
            var range = new Range<int>(left, right);

            Assert.IsTrue(range.Contains(right));
        }

        [TestMethod]
        public void Contains_ReturnsFalse_IsValueIsEqualToRightExclusiveBoundary()
        {
            var left = RandomValue();
            var right = left + 10;
            var range = new Range<int>(left, right, RangeOptions.RightExclusive);

            Assert.IsFalse(range.Contains(right));
        }

        [TestMethod]
        public void Contains_ReturnsTrue_IfValueIsBetweenLeftAndRightBoundaries()
        {
            var left = RandomValue();
            var right = left + 10;
            var range = new Range<int>(left, right);

            Assert.IsTrue(range.Contains(left + 5));
        }

        #endregion

        #region [====== ToString ======]

        [TestMethod]
        public void ToString_ReturnsExpectedValue_IfOptionsIsNone()
        {
            var left = RandomValue();
            var right = left + 10;
            var range = new Range<int>(left, right);

            Assert.AreEqual(string.Format("[{0}, {1}]", left, right), range.ToString());
        }

        [TestMethod]
        public void ToString_ReturnsExpectedValue_IfOptionsIsLeftExclusive()
        {
            var left = RandomValue();
            var right = left + 10;
            var range = new Range<int>(left, right, RangeOptions.LeftExclusive);

            Assert.AreEqual(string.Format("<{0}, {1}]", left, right), range.ToString());
        }

        [TestMethod]
        public void ToString_ReturnsExpectedValue_IfOptionsIsRightExclusive()
        {
            var left = RandomValue();
            var right = left + 10;
            var range = new Range<int>(left, right, RangeOptions.RightExclusive);

            Assert.AreEqual(string.Format("[{0}, {1}>", left, right), range.ToString());
        }

        [TestMethod]
        public void ToString_ReturnsExpectedValue_IfOptionsIsAllExclusive()
        {
            var left = RandomValue();
            var right = left + 10;
            var range = new Range<int>(left, right, RangeOptions.AllExclusive);

            Assert.AreEqual(string.Format("<{0}, {1}>", left, right), range.ToString());
        }

        #endregion

        private static int RandomValue()
        {
            return Clock.Current.UtcDateAndTime().Millisecond;
        }
    }
}
