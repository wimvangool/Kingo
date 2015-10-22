using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.BuildingBlocks.Constraints
{
    [TestClass]
    public sealed class DelegateConstraintTest
    {
        #region [====== Constructor ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_Throws_IfConstraintIsNull()
        {
            new DelegateConstraint<object>(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_Throws_IfErrorMessageIsNotInCorrectFormat()
        {
            new DelegateConstraint<object>(IsValid, @"{xyz");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_Throws_IfNameIsNotAValidIdentifier()
        {
            new DelegateConstraint<object>(IsValid, @"xyz", "!@#$");
        }

        #endregion

        #region [====== Name & ErrorMessage ======]

        [TestMethod]
        public void ErrorMessage_ReturnsExpectedErrorMessage_IfNoErrorMessageWasExplicitlySpecified()
        {
            Assert.AreEqual(Constraint.DefaultErrorMessage, new DelegateConstraint<object>(IsValid).ErrorMessage);
        }

        [TestMethod]
        public void ErrorMessage_ReturnsExpectedErrorMessage_IfErrorMessageWasExplicitlySpecified()
        {
            var errorMessage = StringTemplate.Parse("Custom {error.Message}.");
            var constraint = new DelegateConstraint<object>(IsValid, errorMessage, null);
            
            Assert.AreEqual(errorMessage, constraint.ErrorMessage);
        }

        [TestMethod]
        public void ErrorMessage_ReturnsExpectedErrorMessage_IfErrorMessageWasExplicitlySpecified_ThroughMethod()
        {
            var errorMessage = StringTemplate.Parse("Custom {error.Message}.");
            var constraint = new DelegateConstraint<object>(IsValid).WithErrorMessage(errorMessage);

            Assert.AreEqual(errorMessage, constraint.ErrorMessage);
        }

        [TestMethod]
        public void Name_ReturnsExpectedName_IfNoNameWasExplicitlySpecified()
        {
            Assert.AreEqual(Constraint.DefaultName, new DelegateConstraint<object>(IsValid).Name);
        }

        [TestMethod]
        public void Name_ReturnsExpectedName_IfNameWasExplicitlySpecified()
        {
            var name = Identifier.Parse("other");
            var constraint = new DelegateConstraint<object>(IsValid, null, name);

            Assert.AreEqual(name, constraint.Name);
        }

        [TestMethod]
        public void Name_ReturnsExpectedName_IfNameWasExplicitlySpecified_ThroughMethod()
        {
            var name = Identifier.Parse("other");
            var constraint = new DelegateConstraint<object>(IsValid).WithName(name);

            Assert.AreEqual(name, constraint.Name);
        }

        #endregion

        #region [====== IsSatisfiedBy ======]

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

        #endregion

        #region [====== IsNotSatisfiedBy ======]

        [TestMethod]
        public void IsNotSatisfiedBy_ReturnsTrue_IfDelegateReturnsFalse()
        {
            var constraint = new DelegateConstraint<object>(value => false);
            IErrorMessage errorMessage;

            Assert.IsTrue(constraint.IsNotSatisfiedBy(null, out errorMessage));
            Assert.IsNotNull(errorMessage);
            Assert.AreEqual(Constraint.DefaultErrorMessage.ToString(), errorMessage.ToString());            
        }

        [TestMethod]
        public void IsNotSatisfiedBy_ReturnsFalse_IfDelegateReturnsTrue()
        {
            var constraint = new DelegateConstraint<object>(value => true);
            IErrorMessage errorMessage;

            Assert.IsFalse(constraint.IsNotSatisfiedBy(null, out errorMessage));
            Assert.IsNull(errorMessage);
        }

        [TestMethod]
        public void IsNotSatisfiedBy_ReturnsTrue_IfDelegateReturnsTrue_And_ConstraintIsInverted()
        {
            var constraint = new DelegateConstraint<object>(value => true).Invert();
            IErrorMessage errorMessage;

            Assert.IsTrue(constraint.IsNotSatisfiedBy(null, out errorMessage));
            Assert.IsNotNull(errorMessage);
            Assert.AreEqual(Constraint.DefaultErrorMessage.ToString(), errorMessage.ToString()); 
        }

        [TestMethod]
        public void IsNotSatisfiedBy_ReturnsFalse_IfDelegateReturnsFalse_And_ConstraintIsInverted()
        {
            var constraint = new DelegateConstraint<object>(value => false).Invert();
            IErrorMessage errorMessage;

            Assert.IsFalse(constraint.IsNotSatisfiedBy(null, out errorMessage));
            Assert.IsNull(errorMessage);
        }

        #endregion

        private static bool IsValid(object value)
        {
            throw new NotSupportedException("This is a dummy implementation.");
        }
    }
}
