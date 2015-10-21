using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.BuildingBlocks.Constraints
{
    [TestClass]
    public sealed class DelegateConstraintTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_Throws_IfConstraintIsNull()
        {
            new DelegateConstraint<object>(null);
        }

        [TestMethod]
        public void IsSatisfiedBy_ReturnsTrue_IfDelegateReturnsTrue()
        {
            var constraint = new DelegateConstraint<object>(value => true);

            Assert.IsTrue(constraint.IsSatisfiedBy(null));
            Assert.IsTrue(constraint.IsSatisfiedBy(new object()));
        }

        [TestMethod]
        public void IsSatisfiedBy_ReturnsFalse_IfDelegateReturnsFalse()
        {
            var constraint = new DelegateConstraint<object>(value => false);

            Assert.IsFalse(constraint.IsSatisfiedBy(null));
            Assert.IsFalse(constraint.IsSatisfiedBy(new object()));
        }

        [TestMethod]
        public void IsSatisfiedBy_ReturnsTrue_IfDelegateReturnsFalse_And_ConstraintIsInverted()
        {
            var constraint = new DelegateConstraint<object>(value => false).Invert();

            Assert.IsTrue(constraint.IsSatisfiedBy(null));
            Assert.IsTrue(constraint.IsSatisfiedBy(new object()));
        }

        [TestMethod]
        public void IsSatisfiedBy_ReturnsFalse_IfDelegateReturnsTrue_And_ConstraintIsInverted()
        {
            var constraint = new DelegateConstraint<object>(value => true).Invert();

            Assert.IsFalse(constraint.IsSatisfiedBy(null));
            Assert.IsFalse(constraint.IsSatisfiedBy(new object()));
        }
    }
}
