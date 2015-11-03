using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.BuildingBlocks.Constraints
{
    [TestClass]
    public sealed class ConstraintInverterTest : CompositeConstraintTest
    {
        #region [====== Constructor ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_Throws_IfConstraintIsNull()
        {
            new ConstraintInverter<object>(null);
        }

        #endregion        

        #region [====== IsSatisfiedBy ======]

        [TestMethod]
        public void IsSatisfiedBy_ReturnsTrue_IfConstraintIsNotSatisfied()
        {
            var constraint = CreateConstraint(false);

            Assert.IsTrue(constraint.IsSatisfiedBy(null));
        }

        [TestMethod]
        public void IsSatisfiedBy_ReturnsFalse_IfConstraintIsNotSatisfied_And_IsInvertedAgain()
        {
            var constraint = CreateConstraint(false).Invert();

            Assert.IsFalse(constraint.IsSatisfiedBy(null));
        }

        [TestMethod]
        public void IsSatisfiedBy_ReturnsFalse_IfConstraintIsSatisfied()
        {
            var constraint = CreateConstraint(true);

            Assert.IsFalse(constraint.IsSatisfiedBy(null));
        }

        [TestMethod]
        public void IsSatisfiedBy_ReturnsTrue_IfConstraintIsSatisfied_And_IsInvertedAgain()
        {
            var constraint = CreateConstraint(true).Invert();

            Assert.IsTrue(constraint.IsSatisfiedBy(null));
        }

        #endregion

        #region [====== IsNotSatisfiedBy ======]

        [TestMethod]
        public void IsNotSatisfiedBy_ReturnsTrue_IfConstraintIsSatisfied()
        {
            var constraint = CreateConstraint(true);
            IErrorMessage errorMessage;

            Assert.IsTrue(constraint.IsNotSatisfiedBy(null, out errorMessage));
            Assert.IsNotNull(errorMessage);

            errorMessage.Put("parent", "x");

            Assert.AreEqual("x is not satisfied.", errorMessage.ToString());
        }

        [TestMethod]
        public void IsNotSatisfiedBy_ReturnsFalse_IfConstraintIsSatisfied_And_IsInvertedAgain()
        {
            var constraint = CreateConstraint(true).Invert();
            IErrorMessage errorMessage;

            Assert.IsFalse(constraint.IsNotSatisfiedBy(null, out errorMessage));
            Assert.IsNull(errorMessage);
        }

        [TestMethod]
        public void IsNotSatisfiedBy_ReturnsFalse_IfConstraintIsNotSatisfied()
        {
            var constraint = CreateConstraint(false);
            IErrorMessage errorMessage;

            Assert.IsFalse(constraint.IsNotSatisfiedBy(null, out errorMessage));
            Assert.IsNull(errorMessage);
        }

        [TestMethod]
        public void IsNotSatisfiedBy_ReturnsTrue_IfConstraintIsNotSatisfied_And_IsInvertedAgain()
        {
            var constraint = CreateConstraint(false).Invert();
            IErrorMessage errorMessage;

            Assert.IsTrue(constraint.IsNotSatisfiedBy(null, out errorMessage));
            Assert.IsNotNull(errorMessage);

            errorMessage.Put("child", "x");

            Assert.AreEqual("x is not satisfied.", errorMessage.ToString());
        }

        #endregion

        private static IConstraintWithErrorMessage<object> CreateConstraint(bool value)
        {
            return new ConstraintInverter<object>(NewConstraint(value).WithErrorMessage("{child} is not satisfied."))
                .WithErrorMessage("{parent} is not satisfied.");
        }
    }
}
