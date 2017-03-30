using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging.Validation
{
    public sealed partial class StringConstraintsTest
    {
        #region [====== IsNotEqualTo ======]

        [TestMethod]
        public void ValidateIsNotEqualTo_ReturnsExpectedError_IfMemberIsEqualToValue_Ordinal()
        {
            var message = new ValidatedMessage<string>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotEqualTo("Some value", StringComparison.Ordinal, RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsNotEqualTo_ReturnsDefaultError_IfMemberIsEqualToValue_Ordinal_And_NoErrorMessageIsSpecified()
        {
            var message = new ValidatedMessage<string>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotEqualTo("Some value", StringComparison.Ordinal);

            validator.Validate(message).AssertMemberError("Member (Some value) must not be equal to 'Some value'.");
        }

        [TestMethod]
        public void ValidateIsNotEqualTo_ReturnsNoErrors_IfMemberIsNotEqualToValue_Ordinal()
        {
            var message = new ValidatedMessage<string>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotEqualTo("Some VALUE", StringComparison.Ordinal, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsNotEqualTo_ReturnsExpectedError_IfMemberIsEqualToValue_OrdinalIgnoreCase()
        {
            var message = new ValidatedMessage<string>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotEqualTo("Some value", StringComparison.OrdinalIgnoreCase, RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsNotEqualTo_ReturnsExpectedError_IfMemberIsEqualToValueExceptForCasing_OrdinalIgnoreCase()
        {
            var message = new ValidatedMessage<string>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotEqualTo("Some VALUE", StringComparison.OrdinalIgnoreCase, RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsNotEqualTo_ReturnsNoErrors_IfMemberIsNotEqualToValue_OrdinalIgnoreCase()
        {
            var message = new ValidatedMessage<string>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotEqualTo("Some other value", StringComparison.OrdinalIgnoreCase, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        #endregion

        #region [====== IsEqualTo ======]

        [TestMethod]
        public void ValidateIsEqualTo_ReturnsNoErrors_IfMemberIsEqualToValue_Ordinal()
        {
            var message = new ValidatedMessage<string>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsEqualTo("Some value", StringComparison.Ordinal, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsEqualTo_ReturnsExpectedError_IfMemberIsNotEqualToValue_Ordinal()
        {
            var message = new ValidatedMessage<string>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsEqualTo("Some VALUE", StringComparison.Ordinal, RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsEqualTo_ReturnsDefaultError_IfMemberIsNotEqualToValue_Ordinal_And_NoErrorMessageIsSpecified()
        {
            var message = new ValidatedMessage<string>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsEqualTo("Some VALUE", StringComparison.Ordinal);

            validator.Validate(message).AssertMemberError("Member (Some value) must be equal to 'Some VALUE'.");
        }

        [TestMethod]
        public void ValidateIsEqualTo_ReturnsNoErrors_IfMemberIsEqualToValue_OrdinalIgnoreCase()
        {
            var message = new ValidatedMessage<string>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsEqualTo("Some value", StringComparison.OrdinalIgnoreCase, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsEqualTo_ReturnsNoErrors_IfMemberIsEqualToValueExceptForCasing_OrdinalIgnoreCase()
        {
            var message = new ValidatedMessage<string>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsEqualTo("Some VALUE", StringComparison.OrdinalIgnoreCase, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsEqualTo_ReturnsExpectedError_IfMemberIsNotEqualToValue_OrdinalIgnoreCase()
        {
            var message = new ValidatedMessage<string>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsEqualTo("Some other value", StringComparison.OrdinalIgnoreCase, RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        #endregion
    }
}
