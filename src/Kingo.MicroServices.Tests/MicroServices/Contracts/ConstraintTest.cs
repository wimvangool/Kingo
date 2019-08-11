using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices.Contracts
{
    [TestClass]
    public sealed class ConstraintTest
    {
        #region [====== Basic Validation ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void IsValid_Throws_IfValueIsNull()
        {
            Constraint.IsAlwaysValid<object>().IsValid(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void IsValid_Throws_IfContextIsNull()
        {
            Constraint.IsAlwaysValid<object>().IsValid(new object(), null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void IsValid_Throws_IfValueIsNotOfExpectedType()
        {
            Constraint.IsAlwaysValid<string>().IsValid(new object());
        }

        [TestMethod]
        public void IsValid_ReturnsSuccess_IfValueIsValid()
        {
            AssertSuccess(Constraint.IsAlwaysValid<object>().IsValid(new object()));
        }

        [TestMethod]
        public void IsValid_ReturnsError_IfValueIsNotValid()
        {
            AssertError(Constraint.IsNeverValid<object>().IsValid(new object()), "The specified value (System.Object) is not valid.");           
        }

        [TestMethod]
        public void IsNotValid_ReturnsFalse_IfValueIsValid()
        {
            Assert.IsFalse(Constraint.IsAlwaysValid<object>().IsNotValid(new object(), out var result));
            Assert.AreSame(ValidationResult.Success, result);
        }

        [TestMethod]
        public void IsNotValid_ReturnsTrue_IfValueIsNotValid()
        {
            Assert.IsTrue(Constraint.IsNeverValid<object>().IsNotValid(new object(), out var result));
            AssertError(result, "The specified value (System.Object) is not valid.");            
        }

        #endregion

        #region [====== Logical Combinations (And) ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void And_Throws_IfConstraintIsNull()
        {
            Constraint.IsAlwaysValid<object>().And(null);
        }

        [TestMethod]
        public void AndConstraint_ReturnsSuccess_IfAllConstraintsReturnSuccess()
        {
            var constraintA = Constraint.IsAlwaysValid<object>();
            var constraintB = Constraint.IsAlwaysValid<object>();
            var constraintC = Constraint.IsAlwaysValid<object>();
            var constraint = constraintA.And(constraintB).And(constraintC);

            AssertSuccess(constraint.IsValid(new object()));
        }

        [TestMethod]
        public void AndConstraint_ReturnsError_IfFirstConstraintReturnsError()
        {
            var constraintA = Constraint.IsNeverValid<object>("A");
            var constraintB = Constraint.IsAlwaysValid<object>();
            var constraintC = Constraint.IsAlwaysValid<object>();
            var constraint = constraintA.And(constraintB).And(constraintC);

            AssertError(constraint.IsValid(new object()), "A");
        }

        [TestMethod]
        public void AndConstraint_ReturnsError_IfSecondConstraintReturnsError()
        {
            var constraintA = Constraint.IsAlwaysValid<object>();
            var constraintB = Constraint.IsNeverValid<object>("B");
            var constraintC = Constraint.IsAlwaysValid<object>();
            var constraint = constraintA.And(constraintB).And(constraintC);

            AssertError(constraint.IsValid(new object()), "B");
        }

        [TestMethod]
        public void AndConstraint_ReturnsError_IfThirdConstraintReturnsError()
        {
            var constraintA = Constraint.IsAlwaysValid<object>();
            var constraintB = Constraint.IsAlwaysValid<object>();
            var constraintC = Constraint.IsNeverValid<object>("C");
            var constraint = constraintA.And(constraintB).And(constraintC);

            AssertError(constraint.IsValid(new object()), "C");
        }

        #endregion

        #region [====== Logical Combinations (Or) ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Or_Throws_IfConstraintIsNull()
        {
            Constraint.IsAlwaysValid<object>().Or(null);
        }

        [TestMethod]
        public void Or_ReturnsSuccess_IfFirstConstraintReturnsSuccess()
        {
            var constraintA = Constraint.IsAlwaysValid<object>();
            var constraintB = Constraint.IsNeverValid<object>("B");
            var constraintC = Constraint.IsNeverValid<object>("C");
            var constraint = constraintA.Or(constraintB).Or(constraintC);

            AssertSuccess(constraint.IsValid(new object()));
        }

        [TestMethod]
        public void Or_ReturnsSuccess_IfSecondConstraintReturnsSuccess()
        {
            var constraintA = Constraint.IsNeverValid<object>("A");
            var constraintB = Constraint.IsAlwaysValid<object>();
            var constraintC = Constraint.IsNeverValid<object>("C");
            var constraint = constraintA.Or(constraintB).Or(constraintC);

            AssertSuccess(constraint.IsValid(new object()));
        }

        [TestMethod]
        public void Or_ReturnsSuccess_IfThirdConstraintReturnsSuccess()
        {
            var constraintA = Constraint.IsNeverValid<object>("A");
            var constraintB = Constraint.IsNeverValid<object>("B");
            var constraintC = Constraint.IsAlwaysValid<object>();
            var constraint = constraintA.Or(constraintB).Or(constraintC);

            AssertSuccess(constraint.IsValid(new object()));
        }

        [TestMethod]
        public void Or_ReturnsError_IfNoConstraintReturnsSuccess()
        {
            var constraintA = Constraint.IsNeverValid<object>("A");
            var constraintB = Constraint.IsNeverValid<object>("B");
            var constraintC = Constraint.IsNeverValid<object>("C");
            var constraint = constraintA.Or(constraintB).Or(constraintC);

            AssertError(constraint.IsValid(new object()), "The specified value (System.Object) should satisfy any of the following constraints: [A | B | C].");
        }

        [TestMethod]
        public void Or_ReturnsCustomError_IfNoConstraintReturnsSuccess_And_CustomErrorIsUsed()
        {
            var constraintA = Constraint.IsNeverValid<object>("A");
            var constraintB = Constraint.IsNeverValid<object>("B");
            var constraintC = Constraint.IsNeverValid<object>("C");
            var constraint = constraintA
                .Or(constraintB)
                .Or(constraintC)
                .CombineErrors((errorMessages, value) => string.Join(" ", errorMessages));

            AssertError(constraint.IsValid(new object()), "A B C");
        }

        #endregion

        private static void AssertSuccess(ValidationResult result) =>
            Assert.AreSame(ValidationResult.Success, result);

        private static void AssertError(ValidationResult result, string errorMessage)
        {
            Assert.IsNotNull(result);
            Assert.AreEqual(errorMessage, result.ErrorMessage);
        }
    }
}
