using System;
using Kingo.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Constraints
{
    [TestClass]
    public sealed class AndConstraintTestOne : AndConstraintTest
    {
        #region [====== Constructor ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_Throws_IfConstraintIsNull()
        {
            new AndConstraint<string>(new NullConstraint<string>(), null);
        }

        #endregion

        #region [====== IsSatisfiedBy - 2 Constraints ======]

        [TestMethod]
        public void IsSatisfiedBy2_ReturnsTrue_IfLeftAndRightConstraintAreSatisfied()
        {
            var constraint = CreateAndConstraint(true, true);

            Assert.IsTrue(constraint.IsSatisfiedBy(null));
        }

        [TestMethod]
        public void IsSatisfiedBy2_ReturnsFalse_IfLeftConstraintIsNotSatisfied()
        {
            var constraint = CreateAndConstraint(false, true);

            Assert.IsFalse(constraint.IsSatisfiedBy(null));
        }

        [TestMethod]
        public void IsSatisfiedBy2_ReturnsFalse_IfRightConstraintIsNotSatisfied()
        {
            var constraint = CreateAndConstraint(true, false);

            Assert.IsFalse(constraint.IsSatisfiedBy(null));
        }

        [TestMethod]
        public void IsSatisfiedBy2_ReturnsFalse_IfLeftAndRightConstraintAreNotSatisfied()
        {
            var constraint = CreateAndConstraint(false, false);

            Assert.IsFalse(constraint.IsSatisfiedBy(null));
        }

        #endregion

        #region [====== IsSatisfiedBy - 2 Constraints (Inverted) ======]

        [TestMethod]
        public void IsSatisfiedBy2_ReturnsFalse_IfLeftAndRightConstraintAreSatisfied_And_IsInverted()
        {
            var constraint = CreateAndConstraint(true, true).Invert();

            Assert.IsFalse(constraint.IsSatisfiedBy(null));
        }

        [TestMethod]
        public void IsSatisfiedBy2_ReturnsTrue_IfLeftConstraintIsNotSatisfied_And_IsInverted()
        {
            var constraint = CreateAndConstraint(false, true).Invert();

            Assert.IsTrue(constraint.IsSatisfiedBy(null));
        }

        [TestMethod]
        public void IsSatisfiedBy2_ReturnsTrue_IfRightConstraintIsNotSatisfied_And_IsInverted()
        {
            var constraint = CreateAndConstraint(true, false).Invert();

            Assert.IsTrue(constraint.IsSatisfiedBy(null));
        }

        [TestMethod]
        public void IsSatisfiedBy2_ReturnsTrue_IfLeftAndRightConstraintAreNotSatisfied_And_IsInverted()
        {
            var constraint = CreateAndConstraint(false, false).Invert();

            Assert.IsTrue(constraint.IsSatisfiedBy(null));
        }

        #endregion

        #region [====== IsSatisfiedBy - 3 Constraints ======]

        [TestMethod]
        public void IsSatisfiedBy3_ReturnsTrue_IfLeftAndMiddleAndRightConstraintAreSatisfied()
        {
            var constraint = CreateAndConstraint(true, true, true);

            Assert.IsTrue(constraint.IsSatisfiedBy(null));
        }

        [TestMethod]
        public void IsSatisfiedBy3_ReturnsFalse_IfLeftConstraintIsNotSatisfied()
        {
            var constraint = CreateAndConstraint(false, true, true);

            Assert.IsFalse(constraint.IsSatisfiedBy(null));
        }

        [TestMethod]
        public void IsSatisfiedBy3_ReturnsFalse_IfMiddleConstraintIsNotSatisfied()
        {
            var constraint = CreateAndConstraint(true, false, true);

            Assert.IsFalse(constraint.IsSatisfiedBy(null));
        }

        [TestMethod]
        public void IsSatisfiedBy3_ReturnsFalse_IfRightConstraintIsNotSatisfied()
        {
            var constraint = CreateAndConstraint(true, true, false);

            Assert.IsFalse(constraint.IsSatisfiedBy(null));
        }

        [TestMethod]
        public void IsSatisfiedBy3_ReturnsFalse_IfLeftAndMiddleAndRightConstraintAreNotSatisfied()
        {
            var constraint = CreateAndConstraint(false, false, false);

            Assert.IsFalse(constraint.IsSatisfiedBy(null));
        }

        #endregion

        #region [====== IsNotSatisfiedBy - 2 Constraints ======]

        [TestMethod]
        public void IsNotSatisfiedBy_ReturnsFalse_IfLeftAndRightConstraintAreSatisfied()
        {
            var constraint = CreateAndConstraint(true, true);
            IErrorMessageBuilder errorMessage;

            Assert.IsFalse(constraint.IsNotSatisfiedBy(null, out errorMessage));
            Assert.IsNull(errorMessage);
        }

        [TestMethod]
        public void IsNotSatisfiedBy_ReturnsTrue_IfLeftConstraintIsNotSatisfied()
        {
            var constraint = CreateAndConstraint(false, true);
            IErrorMessageBuilder errorMessage;

            Assert.IsTrue(constraint.IsNotSatisfiedBy(null, out errorMessage));
            Assert.IsNotNull(errorMessage);

            errorMessage.Put("left", "x");

            Assert.AreEqual("x is not satisfied.", errorMessage.ToString());
        }

        [TestMethod]
        public void IsNotSatisfiedBy_ReturnsTrue_IfRightConstraintIsNotSatisfied()
        {
            var constraint = CreateAndConstraint(true, false);
            IErrorMessageBuilder errorMessage;

            Assert.IsTrue(constraint.IsNotSatisfiedBy(null, out errorMessage));
            Assert.IsNotNull(errorMessage);

            errorMessage.Put("right", "y");

            Assert.AreEqual("y is not satisfied.", errorMessage.ToString());
        }

        [TestMethod]
        public void IsNotSatisfiedBy_ReturnsTrue_IfLeftAndRightConstraintAreNotSatisfied()
        {
            var constraint = CreateAndConstraint(false, false);
            IErrorMessageBuilder errorMessage;

            Assert.IsTrue(constraint.IsNotSatisfiedBy(null, out errorMessage));
            Assert.IsNotNull(errorMessage);

            errorMessage.Put("left", "x");

            Assert.AreEqual("x is not satisfied.", errorMessage.ToString());
        }

        #endregion

        #region [====== IsNotSatisfiedBy - 2 Constraints (Inverted) ======]        

        [TestMethod]
        public void IsNotSatisfiedBy_ReturnsTrue_IfLeftAndRightConstraintAreSatisfied_And_IsInverted()
        {
            var constraint = CreateAndConstraint(true, true).Invert(ParentErrorMessage);
            IErrorMessageBuilder errorMessage;

            Assert.IsTrue(constraint.IsNotSatisfiedBy(null, out errorMessage));
            Assert.IsNotNull(errorMessage);

            errorMessage.Put("parent", "x");

            Assert.AreEqual("x is not satisfied.", errorMessage.ToString());
        }

        [TestMethod]
        public void IsNotSatisfiedBy_ReturnsFalse_IfLeftConstraintIsNotSatisfied_And_IsInverted()
        {
            var constraint = CreateAndConstraint(false, true).Invert();
            IErrorMessageBuilder errorMessage;

            Assert.IsFalse(constraint.IsNotSatisfiedBy(null, out errorMessage));
            Assert.IsNull(errorMessage);            
        }

        [TestMethod]
        public void IsNotSatisfiedBy_ReturnsFalse_IfRightConstraintIsNotSatisfied()
        {
            var constraint = CreateAndConstraint(true, false).Invert();
            IErrorMessageBuilder errorMessage;

            Assert.IsFalse(constraint.IsNotSatisfiedBy(null, out errorMessage));
            Assert.IsNull(errorMessage);  
        }

        [TestMethod]
        public void IsNotSatisfiedBy_ReturnsFalse_IfLeftAndRightConstraintAreNotSatisfied()
        {
            var constraint = CreateAndConstraint(false, false).Invert();
            IErrorMessageBuilder errorMessage;

            Assert.IsFalse(constraint.IsNotSatisfiedBy(null, out errorMessage));
            Assert.IsNull(errorMessage);  
        }

        #endregion

        #region [====== IsNotSatisfiedBy - 3 Constraints ======]

        [TestMethod]
        public void IsNotSatisfiedBy3_ReturnsFalse_IfLeftAndMiddleAndRightConstraintAreSatisfied()
        {
            var constraint = CreateAndConstraint(true, true, true);
            IErrorMessageBuilder errorMessage;

            Assert.IsFalse(constraint.IsNotSatisfiedBy(null, out errorMessage));
            Assert.IsNull(errorMessage);
        }

        [TestMethod]
        public void IsNotSatisfiedBy3_ReturnsTrue_IfLeftConstraintIsNotSatisfied()
        {
            var constraint = CreateAndConstraint(false, true, true);
            IErrorMessageBuilder errorMessage;

            Assert.IsTrue(constraint.IsNotSatisfiedBy(null, out errorMessage));
            Assert.IsNotNull(errorMessage);

            errorMessage.Put("left", "x");

            Assert.AreEqual("x is not satisfied.", errorMessage.ToString());
        }

        [TestMethod]
        public void IsNotSatisfiedBy3_ReturnsTrue_IfMiddleConstraintIsNotSatisfied()
        {
            var constraint = CreateAndConstraint(true, false, true);
            IErrorMessageBuilder errorMessage;

            Assert.IsTrue(constraint.IsNotSatisfiedBy(null, out errorMessage));
            Assert.IsNotNull(errorMessage);

            errorMessage.Put("middle", "y");

            Assert.AreEqual("y is not satisfied.", errorMessage.ToString());
        }

        [TestMethod]
        public void IsNotSatisfiedBy3_ReturnsTrue_IfRightConstraintIsNotSatisfied()
        {
            var constraint = CreateAndConstraint(true, true, false);
            IErrorMessageBuilder errorMessage;

            Assert.IsTrue(constraint.IsNotSatisfiedBy(null, out errorMessage));
            Assert.IsNotNull(errorMessage);

            errorMessage.Put("right", "z");

            Assert.AreEqual("z is not satisfied.", errorMessage.ToString());
        }

        [TestMethod]
        public void IsNotSatisfiedBy3_ReturnsTrue_IfLeftAndMiddleAndRightConstraintAreNotSatisfied()
        {
            var constraint = CreateAndConstraint(false, false, false);
            IErrorMessageBuilder errorMessage;

            Assert.IsTrue(constraint.IsNotSatisfiedBy(null, out errorMessage));
            Assert.IsNotNull(errorMessage);

            errorMessage.Put("left", "x");

            Assert.AreEqual("x is not satisfied.", errorMessage.ToString());
        }

        #endregion        

        private static IConstraint<object> CreateAndConstraint(bool left, bool right)
        {
            return new AndConstraint<object>(
                NewConstraint(left).WithErrorMessage(ErrorMessageLeft),
                NewConstraint(right).WithErrorMessage(ErrorMessageRight));
        }

        private static IConstraint<object> CreateAndConstraint(bool left, bool middle, bool right)
        {
            return new AndConstraint<object>(
                NewConstraint(left).WithErrorMessage(ErrorMessageLeft),
                NewConstraint(middle).WithErrorMessage(ErrorMessageMiddle)).And(
                NewConstraint(right).WithErrorMessage(ErrorMessageRight));
        }
    }
}
