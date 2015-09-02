using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.BuildingBlocks.Messaging.Client.DataVirtualization
{
    [TestClass]
    public sealed class IndexSetTest
    {
        private IndexSet _set;

        [TestInitialize]
        public void Setup()
        {
            _set = new IndexSet();
        }

        [TestMethod]
        public void Constructor_CreatesEmptySet()
        {
            AssertSetIsEmpty();
        }

        #region [====== Add ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Add_Throws_IfIndexIsNegative()
        {
            _set.Add(-1);
        }

        [TestMethod]
        public void Add_AddsTheIndexToTheSet_IfIndexIsNotNegative()
        {
            var index = NewIndex();

            _set.Add(index);

            Assert.IsTrue(_set.Contains(index));
            Assert.AreEqual(1, _set.Count);
            Assert.AreEqual(string.Format("[{0}]", index), _set.ToString());
        }

        [TestMethod]
        public void Add_AddsTheIndexToTheSet_IfTheValueIsLeftAdjacentToAnExistingValue()
        {
            var indexA = NewIndex(1);
            var indexB = indexA - 1;

            _set.Add(indexA);
            _set.Add(indexB);

            Assert.IsTrue(_set.Contains(indexA));
            Assert.IsTrue(_set.Contains(indexB));
            Assert.AreEqual(2, _set.Count);
            Assert.AreEqual(string.Format("[{0}, {1}]", indexB, indexA), _set.ToString());
        }

        [TestMethod]
        public void Add_LeavesTheSetUnchanged_IfTheValueIsEqualToAnExistingValue()
        {
            var index = NewIndex();            

            _set.Add(index);
            _set.Add(index);

            Assert.IsTrue(_set.Contains(index));
            Assert.AreEqual(1, _set.Count);
            Assert.AreEqual(string.Format("[{0}]", index), _set.ToString());
        }

        [TestMethod]
        public void Add_AddsTheIndexToTheSet_IfTheValueIsRightAdjacentToAnExistingValue()
        {
            var indexA = NewIndex();
            var indexB = indexA + 1;

            _set.Add(indexA);
            _set.Add(indexB);

            Assert.IsTrue(_set.Contains(indexA));
            Assert.IsTrue(_set.Contains(indexB));
            Assert.AreEqual(2, _set.Count);
            Assert.AreEqual(string.Format("[{0}, {1}]", indexA, indexB), _set.ToString());
        }

        [TestMethod]
        public void Add_AddsTheIndexToTheSet_IfTheValueIsNotAdjacentToAnyExistingValue()
        {
            var indexA = NewIndex();
            var indexB = indexA + 2;

            _set.Add(indexA);
            _set.Add(indexB);

            Assert.IsTrue(_set.Contains(indexA));
            Assert.IsTrue(_set.Contains(indexB));
            Assert.AreEqual(2, _set.Count);
            Assert.AreEqual(string.Format("[{0}][{1}]", indexA, indexB), _set.ToString());
        }

        [TestMethod]
        public void Add_AddsTheIndexToTheSet_IfTheValueFallsRightBetweenTwoExistingRanges()
        {
            var indexA = NewIndex();
            var indexB = indexA + 1;
            var indexC = indexA + 2;

            _set.Add(indexA);           
            _set.Add(indexC);
            _set.Add(indexB);

            Assert.IsTrue(_set.Contains(indexA));
            Assert.IsTrue(_set.Contains(indexB));
            Assert.IsTrue(_set.Contains(indexC));
            Assert.AreEqual(3, _set.Count);
            Assert.AreEqual(string.Format("[{0}, {1}]", indexA, indexC), _set.ToString());
        }

        #endregion

        #region [====== Remove ======]

        [TestMethod]
        public void Remove_LeavesTheSetUnchanged_IfTheValueIsNegative()
        {
            _set.Remove(-1);            

            Assert.AreEqual(0, _set.Count);
        }

        [TestMethod]
        public void Remove_LeavesTheSetUnchanged_IfTheTheValueDoesNotExist()
        {
            _set.Remove(NewIndex());

            Assert.AreEqual(0, _set.Count);
        }

        [TestMethod]
        public void Remove_RemovesTheValue_IfTheValueExists()
        {
            var index = NewIndex();

            _set.Add(index);

            _set.Remove(index);

            AssertSetIsEmpty();
        }

        [TestMethod]
        public void Remove_RemovesTheValue_IfValueIsTheMinValueOfANonTrivialRange()
        {
            var indexA = NewIndex();
            var indexB = indexA + 1;

            _set.Add(indexA);
            _set.Add(indexB);

            _set.Remove(indexA);

            Assert.IsFalse(_set.Contains(indexA));
            Assert.IsTrue(_set.Contains(indexB));
            Assert.AreEqual(string.Format("[{0}]", indexB), _set.ToString());
        }

        [TestMethod]
        public void Remove_RemovesTheValue_IfValueIsTheMaxValueOfANonTrivialRange()
        {
            var indexA = NewIndex();
            var indexB = indexA + 1;

            _set.Add(indexA);
            _set.Add(indexB);

            _set.Remove(indexB);

            Assert.IsTrue(_set.Contains(indexA));
            Assert.IsFalse(_set.Contains(indexB));
            Assert.AreEqual(string.Format("[{0}]", indexA), _set.ToString());
        }

        [TestMethod]
        public void Remove_RemovesTheValue_IfValueFallsRightInTheMiddleOfANonTrivialRange()
        {
            var indexA = NewIndex();
            var indexB = indexA + 1;
            var indexC = indexA + 2;

            _set.Add(indexA);
            _set.Add(indexB);
            _set.Add(indexC);

            _set.Remove(indexB);

            Assert.IsTrue(_set.Contains(indexA));
            Assert.IsFalse(_set.Contains(indexB));
            Assert.IsTrue(_set.Contains(indexC));
            Assert.AreEqual(string.Format("[{0}][{1}]", indexA, indexC), _set.ToString());
        }

        #endregion

        #region [====== Clear ======]

        [TestMethod]
        public void Clear_LeavesTheSetUnchanged_IfTheSetIsAlreadyEmpty()
        {
            _set.Clear();

            AssertSetIsEmpty();
        }

        [TestMethod]
        public void Clear_RemovesAllValuesFromTheSet_IfTheSetIsNotEmpty()
        {
            var indexA = NewIndex();
            var indexB = NewIndex();
            var indexC = NewIndex();

            _set.Add(indexA);
            _set.Add(indexB);
            _set.Add(indexC);

            _set.Clear();

            Assert.IsFalse(_set.Contains(indexA));
            Assert.IsFalse(_set.Contains(indexB));
            Assert.IsFalse(_set.Contains(indexC));
            AssertSetIsEmpty();
        }

        #endregion

        private void AssertSetIsEmpty()
        {
            Assert.IsFalse(_set.Contains(NewIndex()));
            Assert.AreEqual(0, _set.Count);
            Assert.AreEqual(string.Empty, _set.ToString());
        }

        #region [====== Random Value Generation ======]

        private const int _MaxValue = int.MaxValue / 4;
        private static readonly Random _IndexGenerator = new Random();

        private static int NewIndex(int minValue = 0)
        {
            lock (_IndexGenerator)
            {
                return _IndexGenerator.Next(minValue, _MaxValue);
            }
        }

        #endregion
    }
}
