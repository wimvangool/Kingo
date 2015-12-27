using System;
using Kingo.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Constraints
{
    [TestClass]
    public sealed class AndConstraintTestTwo : AndConstraintTest
    {
        #region [====== Constructor ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_Throws_IfConstraintIsNull()
        {
            new AndConstraint<object, object, object>(new NullConstraint<object>().MapInputToOutput(), null);
        }

        #endregion

        #region [====== IsSatisfiedBy ======]

        [TestMethod]
        public void IsSatisfiedBy_ReturnsTrue_IfLeftAndRightConstraintAreSatisfied()
        {            
            var constraint = CreateConstraint(true, true);
            var valueIn = new object();
            object valueOut;

            Assert.IsTrue(constraint.IsSatisfiedBy(valueIn, out valueOut));
            Assert.AreSame(valueIn, valueOut);
        }

        [TestMethod]
        public void IsSatisfiedBy_ReturnsFalse_IfLeftConstraintIsNotSatisfied()
        {
            var constraint = CreateConstraint(false, true);
            var valueIn = new object();
            object valueOut;

            Assert.IsFalse(constraint.IsSatisfiedBy(valueIn, out valueOut));
            Assert.IsNull(valueOut);
        }

        [TestMethod]
        public void IsSatisfiedBy_ReturnsFalse_IfRightConstraintIsNotSatisfied()
        {
            var constraint = CreateConstraint(true, false);
            var valueIn = new object();
            object valueOut;

            Assert.IsFalse(constraint.IsSatisfiedBy(valueIn, out valueOut));
            Assert.IsNull(valueOut);
        }

        [TestMethod]
        public void IsSatisfiedBy_ReturnsFalse_IfBothLeftAndRightConstraintAreNotSatisfied()
        {
            var constraint = CreateConstraint(false, false);
            var valueIn = new object();
            object valueOut;

            Assert.IsFalse(constraint.IsSatisfiedBy(valueIn, out valueOut));
            Assert.IsNull(valueOut);
        }

        #endregion

        #region [====== IsSatisfiedBy (Inverted) ======]

        [TestMethod]
        public void IsSatisfiedBy_ReturnsFalse_IfLeftAndRightConstraintAreSatisfied_And_IsInverted()
        {
            var constraint = CreateConstraint(true, true).Invert().MapInputToOutput();
            var valueIn = new object();
            object valueOut;

            Assert.IsFalse(constraint.IsSatisfiedBy(valueIn, out valueOut));
            Assert.IsNull(valueOut);
        }

        [TestMethod]
        public void IsSatisfiedBy_ReturnsTrue_IfLeftConstraintIsNotSatisfied_And_Inverted()
        {
            var constraint = CreateConstraint(false, true).Invert().MapInputToOutput();
            var valueIn = new object();
            object valueOut;

            Assert.IsTrue(constraint.IsSatisfiedBy(valueIn, out valueOut));
            Assert.AreSame(valueIn, valueOut);
        }

        [TestMethod]
        public void IsSatisfiedBy_ReturnsTrue_IfRightConstraintIsNotSatisfied_And_IsInverted()
        {
            var constraint = CreateConstraint(true, false).Invert().MapInputToOutput();
            var valueIn = new object();
            object valueOut;

            Assert.IsTrue(constraint.IsSatisfiedBy(valueIn, out valueOut));
            Assert.AreSame(valueIn, valueOut);
        }

        [TestMethod]
        public void IsSatisfiedBy_ReturnsTrue_IfBothLeftAndRightConstraintAreNotSatisfied_And_IsInverted()
        {
            var constraint = CreateConstraint(false, false).Invert().MapInputToOutput();
            var valueIn = new object();
            object valueOut;

            Assert.IsTrue(constraint.IsSatisfiedBy(valueIn, out valueOut));
            Assert.AreSame(valueIn, valueOut);
        }

        #endregion

        #region [====== IsNotSatisfiedBy ======]

        [TestMethod]
        public void IsNotSatisfiedBy_ReturnsFalse_IfLeftAndRightConstraintAreSatisfied()
        {
            var constraint = CreateConstraint(true, true);
            var valueIn = new object();
            object valueOut;

            IErrorMessageBuilder errorMessage;            

            Assert.IsFalse(constraint.IsNotSatisfiedBy(valueIn, out errorMessage, out valueOut));
            Assert.AreSame(valueIn, valueOut);
            Assert.IsNull(errorMessage);
        }

        [TestMethod]
        public void IsNotSatisfiedBy_ReturnsTrue_IfLeftConstraintIsNotSatisfied()
        {
            var constraint = CreateConstraint(false, true);
            var valueIn = new object();
            object valueOut;

            IErrorMessageBuilder errorMessage;

            Assert.IsTrue(constraint.IsNotSatisfiedBy(valueIn, out errorMessage, out valueOut));
            Assert.IsNull(valueOut);
            Assert.IsNotNull(errorMessage);

            errorMessage.Put("left", "x");

            Assert.AreEqual("x is not satisfied.", errorMessage.ToString());
        }

        [TestMethod]
        public void IsNotSatisfiedBy_ReturnsTrue_IfRightConstraintIsNotSatisfied()
        {
            var constraint = CreateConstraint(true, false);
            var valueIn = new object();
            object valueOut;

            IErrorMessageBuilder errorMessage;

            Assert.IsTrue(constraint.IsNotSatisfiedBy(valueIn, out errorMessage, out valueOut));
            Assert.IsNull(valueOut);
            Assert.IsNotNull(errorMessage);

            errorMessage.Put("right", "y");

            Assert.AreEqual("y is not satisfied.", errorMessage.ToString());
        }

        [TestMethod]
        public void IsNotSatisfiedBy_ReturnsTrue_IfBothLeftAndRightConstraintAreNotSatisfied()
        {
            var constraint = CreateConstraint(false, false);
            var valueIn = new object();
            object valueOut;

            IErrorMessageBuilder errorMessage;

            Assert.IsTrue(constraint.IsNotSatisfiedBy(valueIn, out errorMessage, out valueOut));
            Assert.IsNull(valueOut);
            Assert.IsNotNull(errorMessage);

            errorMessage.Put("left", "x");

            Assert.AreEqual("x is not satisfied.", errorMessage.ToString());
        }

        #endregion

        #region [====== IsNotSatisfiedBy (Inverted) ======]

        [TestMethod]
        public void IsNotSatisfiedBy_ReturnsTrue_IfLeftAndRightConstraintAreSatisfied_And_IsInverted()
        {
            var constraint = CreateConstraint(true, true).Invert(ParentErrorMessage).MapInputToOutput();
            var valueIn = new object();
            object valueOut;

            IErrorMessageBuilder errorMessage;

            Assert.IsTrue(constraint.IsNotSatisfiedBy(valueIn, out errorMessage, out valueOut));
            Assert.IsNull(valueOut);
            Assert.IsNotNull(errorMessage);

            errorMessage.Put("parent", "x");

            Assert.AreEqual("x is not satisfied.", errorMessage.ToString());
        }

        [TestMethod]
        public void IsNotSatisfiedBy_ReturnsFalse_IfLeftConstraintIsNotSatisfied_And_IsInverted()
        {
            var constraint = CreateConstraint(false, true).Invert().MapInputToOutput();
            var valueIn = new object();
            object valueOut;

            IErrorMessageBuilder errorMessage;

            Assert.IsFalse(constraint.IsNotSatisfiedBy(valueIn, out errorMessage, out valueOut));
            Assert.AreSame(valueIn, valueOut);
            Assert.IsNull(errorMessage);            
        }

        [TestMethod]
        public void IsNotSatisfiedBy_ReturnsFalse_IfRightConstraintIsNotSatisfied_And_IsInverted()
        {
            var constraint = CreateConstraint(true, false).Invert().MapInputToOutput();
            var valueIn = new object();
            object valueOut;

            IErrorMessageBuilder errorMessage;

            Assert.IsFalse(constraint.IsNotSatisfiedBy(valueIn, out errorMessage, out valueOut));
            Assert.AreSame(valueIn, valueOut);
            Assert.IsNull(errorMessage);  
        }

        [TestMethod]
        public void IsNotSatisfiedBy_ReturnsFalse_IfBothLeftAndRightConstraintAreNotSatisfied_And_IsInverted()
        {
            var constraint = CreateConstraint(false, false).Invert().MapInputToOutput();
            var valueIn = new object();
            object valueOut;

            IErrorMessageBuilder errorMessage;

            Assert.IsFalse(constraint.IsNotSatisfiedBy(valueIn, out errorMessage, out valueOut));
            Assert.AreSame(valueIn, valueOut);
            Assert.IsNull(errorMessage); 
        }

        #endregion

        private static IFilter<object, object> CreateConstraint(bool left, bool right)
        {
            var leftConstraint = NewConstraint(left).WithErrorMessage(ErrorMessageLeft).MapInputToOutput();
            var rightConstraint = NewConstraint(right).WithErrorMessage(ErrorMessageRight).MapInputToOutput();

            return new AndConstraint<object, object, object>(leftConstraint, rightConstraint);
        }
    }
}
