using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Syztem.ComponentModel.FluentValidation
{
    [TestClass]
    public sealed class VerifyAnyTest
    {
        private const string _MemberName = "Number";
        private FluentValidator _validator;
        private string _errorMessage;

        [TestInitialize]
        public void Setup()
        {
            _validator = new FluentValidator();
            _errorMessage = Guid.NewGuid().ToString("N");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void VerifyAny_Throws_IfConstraintsIsNull()
        {
            _validator.VerifyThat(8, _MemberName).Any(_errorMessage, null);            
        }

        [TestMethod]
        public void VerifyAny_ReturnsNoErrors_IfNoConstraintsAreSpecified()
        {
            _validator.VerifyThat(8, _MemberName).Any(_errorMessage);
            _validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void VerifyAny_ReturnsNoErrors_IfOneConstraintIsSpecified_And_OneVerificationPasses()
        {
            _validator.VerifyThat(8, _MemberName).Any(_errorMessage, IsPositiveNumber);
            _validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void VerifyAny_ReturnsExpectedErrors_IfOneConstraintIsSpecified_And_NoVerificationPasses()
        {
            _validator.VerifyThat(8, _MemberName).Any(_errorMessage, IsNegativeNumber);
            _validator.Validate().AssertOneError(_errorMessage, _MemberName);
        }

        [TestMethod]
        public void VerifyAny_ReturnsNoErrors_IfTwoConstraintsAreSpecified_And_OneVerificationPasses()
        {
            _validator.VerifyThat(8, _MemberName).Any(_errorMessage, IsPositiveNumber, IsNegativeNumber);
            _validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void VerifyAny_ReturnsExpectedErrors_IfTwoConstraintsAreSpecified_And_NoVerificationPasses()
        {
            _validator.VerifyThat(0, _MemberName).Any(_errorMessage, IsPositiveNumber, IsNegativeNumber);
            _validator.Validate().AssertOneError(_errorMessage, _MemberName);
        }

        private void IsPositiveNumber(IMemberConstraintSet validator, int value)
        {
            validator.VerifyThat(() => value).IsGreaterThan(0);
        }

        private void IsNegativeNumber(IMemberConstraintSet validator, int value)
        {
            validator.VerifyThat(() => value).IsSmallerThan(0);
        }
    }
}
