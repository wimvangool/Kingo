using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging.Validation
{
    [TestClass]
    public sealed class ConstraintValidationAttributeTest
    {
        [TestMethod]        
        public void IsValid_ReturnsSuccess_IsRequiredIsFalse_And_IfValueIsNull()
        {
            AssertSuccess(null, false);
        }

        [TestMethod]
        public void IsValid_ReturnsSuccess_IfRequiredIsFalse_And_ValueIsNotNull_AndConstraintReturnsSuccess()
        {
            AssertSuccess(new object(), false);
        }

        [TestMethod]
        public void IsValid_ReturnsError_IfRequiredIsFalse_And_ValueIsNotNull_AndConstraintReturnsError()
        {
            var errorMessage = Guid.NewGuid().ToString("N");
            var constraint = Constraint.IsNeverValid<object>(errorMessage);

            AssertError(new object(), false, errorMessage, constraint);
        }

        [TestMethod]
        public void IsValid_ReturnsError_IsRequiredIsTrue_And_IfValueIsNull()
        {
            AssertError(null, true, "No value for required parameter '' specified.");
        }

        [TestMethod]
        public void IsValid_ReturnsSuccess_IfRequiredIsTrue_And_ValueIsNotNull_AndConstraintReturnsSuccess()
        {
            AssertSuccess(new object(), true);
        }

        [TestMethod]
        public void IsValid_ReturnsError_IfRequiredIsTrue_And_ValueIsNotNull_AndConstraintReturnsError()
        {
            var errorMessage = Guid.NewGuid().ToString("N");
            var constraint = Constraint.IsNeverValid<object>(errorMessage);

            AssertError(new object(), true, errorMessage, constraint);
        }

        private static void AssertSuccess(object value, bool isRequired, IConstraint constraint = null) =>
            Assert.AreSame(ValidationResult.Success, Validate(value, isRequired, constraint));

        private static void AssertError(object value, bool isRequired, string errorMessage, IConstraint constraint = null)
        {
            var result = Validate(value, isRequired, constraint);

            Assert.IsNotNull(result);
            Assert.AreEqual(errorMessage, result.ErrorMessage);
        }

        private static ValidationResult Validate(object value, bool isRequired, IConstraint constraint = null) =>
            CreateAttribute(isRequired, constraint).GetValidationResult(value, new ValidationContext(value ?? new object()));

        private static ConstraintValidationAttribute CreateAttribute(bool isRequired, IConstraint constraint = null) =>
            new ConstraintValidationAttributeStub(constraint ?? Constraint.IsAlwaysValid<object>(), isRequired);
    }
}
