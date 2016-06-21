using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo
{
    [TestClass]
    public sealed class ComparableTest
    {
        #region [====== CompareValues ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CompareValues_Throws_IfRightIsNull()
        {
            Comparable.CompareValues(0, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CompareValues_Throws_IfRightIsNotOfCorrectType()
        {
            Comparable.CompareValues(0, string.Empty);
        }

        [TestMethod]
        public void CompareValues_ReturnsNegativeValue_IfLeftIsLessThanRight()
        {
            Assert.IsTrue(Comparable.CompareValues(0, 1) < 0);
        }

        [TestMethod]
        public void CompareValues_ReturnsZero_IfLeftIsEqualToRight()
        {
            Assert.AreEqual(0, Comparable.CompareValues(0, 0));
        }

        [TestMethod]
        public void CompareValues_ReturnsPositiveValue_IfLeftIsGreaterThanRight()
        {
            Assert.IsTrue(0 < Comparable.CompareValues(1, 0));
        }

        #endregion

        #region [====== CompareReferences ======]

        [TestMethod]        
        public void CompareReferences_ReturnsPositiveValue_IfRightIsNull()
        {
            Assert.IsTrue(0 < Comparable.CompareReferences(string.Empty, null));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CompareReferences_Throws_IfRightIsNotOfCorrectType()
        {
            Comparable.CompareReferences(string.Empty, 0);
        }

        [TestMethod]
        public void CompareReferences_ReturnsNegativeValue_IfLeftIsLessThanRight()
        {
            Assert.IsTrue(Comparable.CompareReferences(string.Empty, " ") < 0);
        }

        [TestMethod]
        public void CompareReferences_ReturnsZero_IfLeftIsEqualToRight()
        {
            Assert.AreEqual(0, Comparable.CompareReferences(string.Empty, string.Empty));
        }

        [TestMethod]
        public void CompareReferences_ReturnsPositiveValue_IfLeftIsGreaterThanRight()
        {
            Assert.IsTrue(0 < Comparable.CompareReferences(" ", string.Empty));
        }

        #endregion
    }
}
