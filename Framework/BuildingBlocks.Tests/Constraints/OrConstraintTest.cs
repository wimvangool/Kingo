using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.BuildingBlocks.Constraints
{
    [TestClass]
    public sealed class OrConstraintTest : CompositeConstraintTest
    {
        #region [====== Constructor ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_Throws_IfConstraintIsNull()
        {
            new OrConstraint<string>(new NullConstraint<string>(), null);
        }

        #endregion

        #region [====== IsSatisfiedBy - 2 Constraints ======]

        [TestMethod]
        public void IsSatisfiedBy2_ReturnsTrue_IfLeftAndRightConstraintAreSatisfied()
        {
            var constraint = CreateOrConstraint(true, true);

            Assert.IsTrue(constraint.IsSatisfiedBy(null));
        }

        [TestMethod]
        public void IsSatisfiedBy2_ReturnsTrue_IfLeftConstraintIsSatisfied()
        {
            var constraint = CreateOrConstraint(true, false);

            Assert.IsTrue(constraint.IsSatisfiedBy(null));
        }

        [TestMethod]
        public void IsSatisfiedBy2_ReturnsTrue_IfRightConstraintIsSatisfied()
        {
            var constraint = CreateOrConstraint(false, true);

            Assert.IsTrue(constraint.IsSatisfiedBy(null));
        }

        [TestMethod]
        public void IsSatisfiedBy2_ReturnsFalse_IfLeftAndRightConstraintAreNotSatisfied()
        {
            var constraint = CreateOrConstraint(false, false);

            Assert.IsFalse(constraint.IsSatisfiedBy(null));
        }

        #endregion

        #region [====== IsSatisfiedBy - 2 Constraints (Inverted) ======]

        [TestMethod]
        public void IsSatisfiedBy2_ReturnsFalse_IfLeftAndRightConstraintAreSatisfied_And_IsInverted()
        {
            var constraint = CreateOrConstraint(true, true).Invert();

            Assert.IsFalse(constraint.IsSatisfiedBy(null));
        }

        [TestMethod]
        public void IsSatisfiedBy2_ReturnsFalse_IfLeftConstraintIsSatisfied_And_IsInverted()
        {
            var constraint = CreateOrConstraint(true, false).Invert();

            Assert.IsFalse(constraint.IsSatisfiedBy(null));
        }

        [TestMethod]
        public void IsSatisfiedBy2_ReturnsFalse_IfRightConstraintIsSatisfied_And_IsInverted()
        {
            var constraint = CreateOrConstraint(false, true).Invert();

            Assert.IsFalse(constraint.IsSatisfiedBy(null));
        }

        [TestMethod]
        public void IsSatisfiedBy2_ReturnsTrue_IfLeftAndRightConstraintAreNotSatisfied_And_IsInverted()
        {
            var constraint = CreateOrConstraint(false, false).Invert();

            Assert.IsTrue(constraint.IsSatisfiedBy(null));
        }

        #endregion

        #region [====== IsSatisfiedBy - 3 Constraints ======]

        [TestMethod]
        public void IsSatisfiedBy3_ReturnsTrue_IfLeftAndMiddleAndRightConstraintAreSatisfied()
        {
            var constraint = CreateOrConstraint(true, true, true);

            Assert.IsTrue(constraint.IsSatisfiedBy(null));
        }

        [TestMethod]
        public void IsSatisfiedBy3_ReturnsTrue_IfLeftConstraintIsSatisfied()
        {
            var constraint = CreateOrConstraint(true, false, false);

            Assert.IsTrue(constraint.IsSatisfiedBy(null));
        }

        [TestMethod]
        public void IsSatisfiedBy3_ReturnsTrue_IfMiddleConstraintIsSatisfied()
        {
            var constraint = CreateOrConstraint(false, true, false);

            Assert.IsTrue(constraint.IsSatisfiedBy(null));
        }

        [TestMethod]
        public void IsSatisfiedBy3_ReturnsTrue_IfRightConstraintIsSatisfied()
        {
            var constraint = CreateOrConstraint(false, false, true);

            Assert.IsTrue(constraint.IsSatisfiedBy(null));
        }

        [TestMethod]
        public void IsSatisfiedBy3_ReturnsFalse_IfLeftAndMiddleAndRightConstraintAreNotSatisfied()
        {
            var constraint = CreateOrConstraint(false, false, false);

            Assert.IsFalse(constraint.IsSatisfiedBy(null));
        }

        #endregion

        #region [====== IsNotSatisfiedBy - 2 Constraints ======]

        [TestMethod]
        public void IsNotSatisfiedBy_ReturnsFalse_IfLeftAndRightConstraintAreSatisfied()
        {
            var constraint = CreateOrConstraint(true, true);
            IErrorMessage errorMessage;

            Assert.IsFalse(constraint.IsNotSatisfiedBy(null, out errorMessage));
            Assert.IsNull(errorMessage);
        }

        [TestMethod]
        public void IsNotSatisfiedBy_ReturnsFalse_IfLeftConstraintIsSatisfied()
        {
            var constraint = CreateOrConstraint(true, false);
            IErrorMessage errorMessage;

            Assert.IsFalse(constraint.IsNotSatisfiedBy(null, out errorMessage));
            Assert.IsNull(errorMessage);
        }

        [TestMethod]
        public void IsNotSatisfiedBy_ReturnsFalse_IfRightConstraintIsSatisfied()
        {
            var constraint = CreateOrConstraint(false, true);
            IErrorMessage errorMessage;

            Assert.IsFalse(constraint.IsNotSatisfiedBy(null, out errorMessage));
            Assert.IsNull(errorMessage);
        }

        [TestMethod]
        public void IsNotSatisfiedBy_ReturnsTrue_IfLeftAndRightConstraintAreNotSatisfied()
        {
            var constraint = CreateOrConstraint(false, false);
            IErrorMessage errorMessage;

            Assert.IsTrue(constraint.IsNotSatisfiedBy(null, out errorMessage));
            Assert.IsNotNull(errorMessage);
            Assert.AreEqual("Value (<null>) is not valid.", errorMessage.ToString());
        }

        [TestMethod]
        public void IsNotSatisfiedBy_ReturnsExpectedErrorMessage_IfLeftAndRightConstraintAreNotSatisfied_And_ErrorMessageHasBeenSpecified()
        {
            var constraint = CreateOrConstraint(false, false)
                .WithErrorMessage("Both {left} and {right} are not satisfied.");

            IErrorMessage errorMessage;

            Assert.IsTrue(constraint.IsNotSatisfiedBy(null, out errorMessage));
            Assert.IsNotNull(errorMessage);

            errorMessage.Add("left", "x");
            errorMessage.Add("right", "y");

            Assert.AreEqual("Both x and y are not satisfied.", errorMessage.ToString());
        }

        #endregion

        #region [====== IsNotSatisfiedBy - 2 Constraints (Inverted) ======]

        [TestMethod]
        public void IsNotSatisfiedBy_ReturnsTrue_IfLeftAndRightConstraintAreSatisfied_And_IsInverted()
        {
            var constraint = CreateOrConstraint(true, true).Invert(ParentErrorMessage);
            IErrorMessage errorMessage;

            Assert.IsTrue(constraint.IsNotSatisfiedBy(null, out errorMessage));
            Assert.IsNotNull(errorMessage);

            errorMessage.Add("parent", "x");

            Assert.AreEqual("x is not satisfied.", errorMessage.ToString());
        }

        [TestMethod]
        public void IsNotSatisfiedBy_ReturnsTrue_IfLeftConstraintIsSatisfied_And_IsInverted()
        {
            var constraint = CreateOrConstraint(true, false).Invert(ParentErrorMessage);
            IErrorMessage errorMessage;

            Assert.IsTrue(constraint.IsNotSatisfiedBy(null, out errorMessage));
            Assert.IsNotNull(errorMessage);

            errorMessage.Add("parent", "x");

            Assert.AreEqual("x is not satisfied.", errorMessage.ToString());
        }

        [TestMethod]
        public void IsNotSatisfiedBy_ReturnsTrue_IfRightConstraintIsSatisfied_And_IsInverted()
        {
            var constraint = CreateOrConstraint(false, true).Invert(ParentErrorMessage);
            IErrorMessage errorMessage;

            Assert.IsTrue(constraint.IsNotSatisfiedBy(null, out errorMessage));
            Assert.IsNotNull(errorMessage);

            errorMessage.Add("parent", "x");

            Assert.AreEqual("x is not satisfied.", errorMessage.ToString());
        }

        [TestMethod]
        public void IsNotSatisfiedBy_ReturnsFalse_IfLeftAndRightConstraintAreNotSatisfied_And_IsInverted()
        {
            var constraint = CreateOrConstraint(false, false).Invert(ParentErrorMessage);
            IErrorMessage errorMessage;

            Assert.IsFalse(constraint.IsNotSatisfiedBy(null, out errorMessage));
            Assert.IsNull(errorMessage);            
        }        

        #endregion

        #region [====== IsNotSatisfiedBy - 3 Constraints ======]

        [TestMethod]
        public void IsNotSatisfiedBy3_ReturnsFalse_IfLeftAndMiddleAndRightConstraintAreSatisfied()
        {
            var constraint = CreateOrConstraint(true, true, true);
            IErrorMessage errorMessage;

            Assert.IsFalse(constraint.IsNotSatisfiedBy(null, out errorMessage));
            Assert.IsNull(errorMessage);
        }

        [TestMethod]
        public void IsNotSatisfiedBy3_ReturnsFalse_IfLeftConstraintIsSatisfied()
        {
            var constraint = CreateOrConstraint(true, false, false);
            IErrorMessage errorMessage;

            Assert.IsFalse(constraint.IsNotSatisfiedBy(null, out errorMessage));
            Assert.IsNull(errorMessage);
        }

        [TestMethod]
        public void IsNotSatisfiedBy3_ReturnsFalse_IfMiddleConstraintIsSatisfied()
        {
            var constraint = CreateOrConstraint(false, true, false);
            IErrorMessage errorMessage;

            Assert.IsFalse(constraint.IsNotSatisfiedBy(null, out errorMessage));
            Assert.IsNull(errorMessage);
        }

        [TestMethod]
        public void IsNotSatisfiedBy3_ReturnsFalse_IfRightConstraintIsSatisfied()
        {
            var constraint = CreateOrConstraint(false, false, true);
            IErrorMessage errorMessage;

            Assert.IsFalse(constraint.IsNotSatisfiedBy(null, out errorMessage));
            Assert.IsNull(errorMessage);
        }

        [TestMethod]
        public void IsNotSatisfiedBy3_ReturnsTrue_IfLeftAndMiddleAndRightConstraintAreNotSatisfied()
        {
            var constraint = CreateOrConstraint(false, false, false);
            IErrorMessage errorMessage;

            Assert.IsTrue(constraint.IsNotSatisfiedBy(null, out errorMessage));
            Assert.IsNotNull(errorMessage);
            Assert.AreEqual("Value (<null>) is not valid.", errorMessage.ToString());
        }

        [TestMethod]
        public void IsNotSatisfiedBy3_ReturnsExpectedErrorMessage_IfLeftAndMiddleAndRightConstraintAreNotSatisfied_And_ErrorMessageHasBeenSpecified()
        {
            var constraint = CreateOrConstraint(false, false, false)
                .WithErrorMessage("Both {left}, {middle} and {right} are not satisfied.");

            IErrorMessage errorMessage;

            Assert.IsTrue(constraint.IsNotSatisfiedBy(null, out errorMessage));
            Assert.IsNotNull(errorMessage);

            errorMessage.Add("left", "x");
            errorMessage.Add("middle", "y");
            errorMessage.Add("right", "z");

            Assert.AreEqual("Both x, y and z are not satisfied.", errorMessage.ToString());
        }

        #endregion                

        private static IConstraintWithErrorMessage<object> CreateOrConstraint(bool left, bool right)
        {
            return new OrConstraint<object>(NewConstraint(left), NewConstraint(right));
        }

        private static IConstraintWithErrorMessage<object> CreateOrConstraint(bool left, bool middle, bool right)
        {
            return new OrConstraint<object>(NewConstraint(left), NewConstraint(middle)).Or(NewConstraint(right));
        }
    }
}
