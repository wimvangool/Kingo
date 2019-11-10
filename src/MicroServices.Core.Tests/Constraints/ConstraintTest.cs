using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Constraints
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
        [ExpectedException(typeof(ArgumentException))]
        public void IsValid_Throws_IfValueIsNotOfExpectedType()
        {
            try
            {
                Constraint.IsAlwaysValid<string>().IsValid(new object());
            }
            catch (ArgumentException exception)
            {
                Assert.IsTrue(exception.Message.Contains("The specified value of type 'Object' is not supported by constraint of type 'DelegateConstraint<String>'; expected value of type 'String'."));
                throw;
            }
        }

        [TestMethod]
        public void IsValid_ReturnsTrue_IfValueIsValid()
        {
            Assert.IsTrue(Constraint.IsAlwaysValid<object>().IsValid(new object()));
        }

        [TestMethod]
        public void IsValid_ReturnsFalse_IfValueIsNotValid()
        {
            Assert.IsFalse(Constraint.IsNeverValid<object>().IsValid(new object()));           
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
        public void AndConstraint_ReturnsTrue_IfAllConstraintsReturnTrue()
        {
            var constraintA = Constraint.IsAlwaysValid<object>();
            var constraintB = Constraint.IsAlwaysValid<object>();
            var constraintC = Constraint.IsAlwaysValid<object>();
            var constraint = constraintA.And(constraintB).And(constraintC);

            Assert.IsTrue(constraint.IsValid(new object()));
        }

        [TestMethod]
        public void AndConstraint_ReturnsFalse_IfFirstConstraintReturnsFalse()
        {
            var constraintA = Constraint.IsNeverValid<object>();
            var constraintB = Constraint.IsAlwaysValid<object>();
            var constraintC = Constraint.IsAlwaysValid<object>();
            var constraint = constraintA.And(constraintB).And(constraintC);

            Assert.IsFalse(constraint.IsValid(new object()));
        }

        [TestMethod]
        public void AndConstraint_ReturnsFalse_IfSecondConstraintReturnsFalse()
        {
            var constraintA = Constraint.IsAlwaysValid<object>();
            var constraintB = Constraint.IsNeverValid<object>();
            var constraintC = Constraint.IsAlwaysValid<object>();
            var constraint = constraintA.And(constraintB).And(constraintC);

            Assert.IsFalse(constraint.IsValid(new object()));
        }

        [TestMethod]
        public void AndConstraint_ReturnsFalse_IfThirdConstraintReturnsFalse()
        {
            var constraintA = Constraint.IsAlwaysValid<object>();
            var constraintB = Constraint.IsAlwaysValid<object>();
            var constraintC = Constraint.IsNeverValid<object>();
            var constraint = constraintA.And(constraintB).And(constraintC);

            Assert.IsFalse(constraint.IsValid(new object()));
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
        public void Or_ReturnsTrue_IfFirstConstraintReturnsTrue()
        {
            var constraintA = Constraint.IsAlwaysValid<object>();
            var constraintB = Constraint.IsNeverValid<object>();
            var constraintC = Constraint.IsNeverValid<object>();
            var constraint = constraintA.Or(constraintB).Or(constraintC);

            Assert.IsTrue(constraint.IsValid(new object()));
        }

        [TestMethod]
        public void Or_ReturnsTrue_IfSecondConstraintReturnsTrue()
        {
            var constraintA = Constraint.IsNeverValid<object>();
            var constraintB = Constraint.IsAlwaysValid<object>();
            var constraintC = Constraint.IsNeverValid<object>();
            var constraint = constraintA.Or(constraintB).Or(constraintC);

            Assert.IsTrue(constraint.IsValid(new object()));
        }

        [TestMethod]
        public void Or_ReturnsTrue_IfThirdConstraintReturnsTrue()
        {
            var constraintA = Constraint.IsNeverValid<object>();
            var constraintB = Constraint.IsNeverValid<object>();
            var constraintC = Constraint.IsAlwaysValid<object>();
            var constraint = constraintA.Or(constraintB).Or(constraintC);

            Assert.IsTrue(constraint.IsValid(new object()));
        }

        [TestMethod]
        public void Or_ReturnsFalse_IfAllConstraintsReturnsFalse()
        {
            var constraintA = Constraint.IsNeverValid<object>();
            var constraintB = Constraint.IsNeverValid<object>();
            var constraintC = Constraint.IsNeverValid<object>();
            var constraint = constraintA.Or(constraintB).Or(constraintC);

            Assert.IsFalse(constraint.IsValid(new object()));
        }

        #endregion
    }
}
