using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel.Messaging.Client.DataVirtualization
{
    [TestClass]
    public class IndexRangeTest
    {
        #region [====== Constructors ======]

        [TestMethod]
        public void DefaultConstructor_CreatesRangeWithIndexZero()
        {
            var range = new IndexRange();

            Assert.AreEqual(0, range.MinValue);
            Assert.AreEqual(0, range.MaxValue);
            Assert.AreEqual(1, range.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Constructor_Throws_IfIndexIsNegative()
        {
            new IndexRange(-1, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Constructor_Throws_IfCountIsZero()
        {
            new IndexRange(0, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Constructor_Throws_IfIndexPlusCountIsGreaterThanIntMaxValue()
        {
            new IndexRange(int.MaxValue, 2);
        }

        #endregion

        #region [====== Properties ======]

        [TestMethod]
        public void MinValue_IsEqualToSpecifiedIndex()
        {
            var index = NewIndex();
            var range = new IndexRange(index);

            Assert.AreEqual(index, range.MinValue);
        }

        [TestMethod]
        public void MaxValue_IsEqualToIndexPlusCountMinusOne()
        {
            var index = NewIndex();
            var count = NewCount();
            var range = new IndexRange(index, count);

            Assert.AreEqual(index + count - 1, range.MaxValue);
        }

        [TestMethod]
        public void Count_IsEqualToCount()
        {
            var index = NewIndex();
            var count = NewCount();
            var range = new IndexRange(index, count);

            Assert.AreEqual(count, range.Count);
        }

        #endregion

        #region [====== Contains ======]

        [TestMethod]
        public void Contains_ReturnsTrue_IfValueIsEqualToMinValue()
        {
            var index = NewIndex();
            var count = NewCount();
            var range = new IndexRange(index, count);

            Assert.IsTrue(range.Contains(range.MinValue));
        }

        [TestMethod]
        public void Contains_ReturnsTrue_IfValueIsEqualToMaxValue()
        {
            var index = NewIndex();
            var count = NewCount();
            var range = new IndexRange(index, count);

            Assert.IsTrue(range.Contains(range.MaxValue));
        }

        [TestMethod]
        public void Contains_ReturnsTrue_IfValueIsBetweenMinValueAndMaxValue()
        {
            var index = NewIndex();
            var count = NewCount(4);
            var range = new IndexRange(index, count);
            var value = range.MinValue + (range.Count / 2);

            Assert.IsTrue(range.Contains(value));
        }

        [TestMethod]
        public void Contains_ReturnsFalse_IfValueIsSmallerThanMinValue()
        {
            var index = NewIndex();
            var count = NewCount();
            var range = new IndexRange(index, count);

            Assert.IsFalse(range.Contains(range.MinValue - 1));
        }

        [TestMethod]
        public void Contains_ReturnsFalse_IfValueIsGreaterThanMaxValue()
        {
            var index = NewIndex();
            var count = NewCount();
            var range = new IndexRange(index, count);

            Assert.IsFalse(range.Contains(range.MaxValue + 1));
        }

        #endregion

        #region [====== IsLeftAdjacentTo ======]

        [TestMethod]
        public void IsLeftAdjacentTo_ReturnsFalse_IfValueIsTooSmall()
        {
            var index = NewIndex();
            var count = NewCount();
            var range = new IndexRange(index, count);

            Assert.IsFalse(range.IsLeftAdjacentTo(range.MinValue - 2));
        }

        [TestMethod]
        public void IsLeftAdjacentTo_ReturnsFalse_IfValueIsEqualToMinValue()
        {
            var index = NewIndex();
            var count = NewCount();
            var range = new IndexRange(index, count);

            Assert.IsFalse(range.IsLeftAdjacentTo(range.MinValue));
        }

        [TestMethod]
        public void IsLeftAdjacentTo_ReturnsTrue_IfValueIsEqualToMinValueMinusOne()
        {
            var index = NewIndex();
            var count = NewCount();
            var range = new IndexRange(index, count);

            Assert.IsTrue(range.IsLeftAdjacentTo(range.MinValue - 1));
        }

        #endregion

        #region [====== IsRightAdjacentTo(int) ======]

        [TestMethod]
        public void IsRightAdjacentTo_ReturnsFalse_IfValueIsTooLarge()
        {
            var index = NewIndex();
            var count = NewCount();
            var range = new IndexRange(index, count);

            Assert.IsFalse(range.IsRightAdjacentTo(range.MaxValue + 2));
        }

        [TestMethod]
        public void IsRightAdjacentTo_ReturnsFalse_IfValueIsEqualToMaxValue()
        {
            var index = NewIndex();
            var count = NewCount();
            var range = new IndexRange(index, count);

            Assert.IsFalse(range.IsRightAdjacentTo(range.MaxValue));
        }

        [TestMethod]
        public void IsRightAdjacentTo_ReturnsTrue_IfValueIsEqualToMaxValuePlusOne()
        {
            var index = NewIndex();
            var count = NewCount();
            var range = new IndexRange(index, count);

            Assert.IsTrue(range.IsRightAdjacentTo(range.MaxValue + 1));
        }

        #endregion

        #region [====== IsRightAdjacentTo(IndexRange) ======]

        [TestMethod]
        public void IsRightAdjacentTo_ReturnsFalse_IfMinValueOfOtherIsTooLarge()
        {
            var index = NewIndex();
            var count = NewCount();
            var rangeA = new IndexRange(index, count);
            var rangeB = new IndexRange(rangeA.MaxValue + 2);

            Assert.IsFalse(rangeA.IsRightAdjacentTo(rangeB));
        }

        [TestMethod]
        public void IsRightAdjacentTo_ReturnsFalse_IfMinValueOfOtherIsEqualToMaxValue()
        {
            var index = NewIndex();
            var count = NewCount();
            var rangeA = new IndexRange(index, count);
            var rangeB = new IndexRange(rangeA.MaxValue);

            Assert.IsFalse(rangeA.IsRightAdjacentTo(rangeB));
        }

        [TestMethod]
        public void IsRightAdjacentTo_ReturnsTrue_IfMinValueOfOtherIsEqualToMaxValuePlusOne()
        {
            var index = NewIndex();
            var count = NewCount();
            var rangeA = new IndexRange(index, count);
            var rangeB = new IndexRange(rangeA.MaxValue + 1);

            Assert.IsTrue(rangeA.IsRightAdjacentTo(rangeB));
        }

        #endregion

        #region [====== AddToLeft ======]

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void AddToLeft_Throws_IfMinValueIsZero()
        {
            var range = new IndexRange(0);

            range.AddToLeft();
        }

        [TestMethod]
        public void AddToLeft_AddsOneIndexToTheLeftSide_IfMinValueIsLargerThanZero()
        {
            var index = NewIndex();
            var count = NewCount();
            var rangeA = new IndexRange(index, count);
            var rangeB = rangeA.AddToLeft();

            Assert.AreEqual(index - 1, rangeB.MinValue);
            Assert.AreEqual(rangeA.MaxValue, rangeB.MaxValue);
            Assert.AreEqual(count + 1, rangeB.Count);
        }

        #endregion

        #region [====== AddToRight ======]

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void AddToRight_Throws_IfMaxValueIsEqualToIntMaxValue()
        {
            var range = new IndexRange(int.MaxValue);

            range.AddToRight();
        }

        [TestMethod]
        public void AddToRight_AddsOneIndexToTheRightSide_IfMaxValueIsLessThanIntMaxValue()
        {
            var index = NewIndex();
            var count = NewCount();
            var rangeA = new IndexRange(index, count);
            var rangeB = rangeA.AddToRight();

            Assert.AreEqual(index, rangeB.MinValue);
            Assert.AreEqual(rangeA.MaxValue + 1, rangeB.MaxValue);
            Assert.AreEqual(count + 1, rangeB.Count);
        }

        #endregion

        #region [====== AddToRight(int) ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void AddToRight_Throws_IfCountIsNegative()
        {
            var index = NewIndex();
            var count = NewCount();
            var range = new IndexRange(index, count);

            range.AddToRight(-1);
        }

        [TestMethod]
        public void AddToRight_ReturnsEqualRange_IfCountIsZero()
        {
            var index = NewIndex();
            var count = NewCount();
            var rangeA = new IndexRange(index, count);
            var rangeB = rangeA.AddToRight(0);

            Assert.AreEqual(rangeA, rangeB);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void AddToRight_Throws_IfMaxValuePlusCountIsGreaterThanToIntMaxValue()
        {
            var range = new IndexRange(int.MaxValue - 3);

            range.AddToRight(4);
        }

        [TestMethod]
        public void AddToRight_AddsSpecifiedNumberOfIndicesToTheRightSide_IfMaxValuePlusCountIsLessThanOrEqualToIntMaxValue()
        {
            var index = NewIndex();
            var count = NewCount();
            var rangeA = new IndexRange(index, count);
            var rangeB = rangeA.AddToRight(12);

            Assert.AreEqual(index, rangeB.MinValue);
            Assert.AreEqual(rangeA.MaxValue + 12, rangeB.MaxValue);
            Assert.AreEqual(count + 12, rangeB.Count);
        }

        #endregion

        #region [====== RemoveFromLeft ======]

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void RemoveFromLeft_Throws_IfCountIsZero()
        {
            var range = new IndexRange(NewIndex());

            range.RemoveFromLeft();
        }

        [TestMethod]
        public void RemoveFromLeft_RemovesOneIndexFromTheLeft()
        {
            var index = NewIndex();
            var count = NewCount();
            var rangeA = new IndexRange(index, count);
            var rangeB = rangeA.RemoveFromLeft();

            Assert.AreEqual(rangeA.MinValue + 1, rangeB.MinValue);
            Assert.AreEqual(rangeA.MaxValue, rangeB.MaxValue);
            Assert.AreEqual(rangeA.Count - 1, rangeB.Count);
        }

        #endregion

        #region [====== RemoveFromRight ======]

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void RemoveFromRight_Throws_IfCountIsZero()
        {
            var range = new IndexRange(NewIndex());

            range.RemoveFromRight();
        }

        [TestMethod]
        public void RemoveFromRight_RemovesOneIndexFromTheLeft()
        {
            var index = NewIndex();
            var count = NewCount();
            var rangeA = new IndexRange(index, count);
            var rangeB = rangeA.RemoveFromRight();

            Assert.AreEqual(rangeA.MaxValue - 1, rangeB.MaxValue);
            Assert.AreEqual(rangeA.MinValue, rangeB.MinValue);
            Assert.AreEqual(rangeA.Count - 1, rangeB.Count);
        }

        #endregion

        #region [====== Random Number Generation ======]

        private const int _MaxValue = int.MaxValue / 4;
        private static readonly Random _IndexGenerator = new Random();

        private static int NewIndex(int minValue = 0)
        {
            lock (_IndexGenerator)
            {
                return _IndexGenerator.Next(minValue, _MaxValue);
            }
        }        

        private static int NewCount(int minValue = 1)
        {
            lock (_IndexGenerator)
            {
                return _IndexGenerator.Next(minValue, _MaxValue);
            }
        }

        #endregion
    }
}
