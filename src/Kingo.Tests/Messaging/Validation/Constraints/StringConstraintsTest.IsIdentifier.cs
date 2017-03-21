using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging.Validation.Constraints
{
    public sealed partial class StringConstraintsTest
    {
        #region [====== IsIdentifier ======]

        [TestMethod]
        public void ValidateIsIdentifier_ReturnsExpectedError_IfValueIsNull()
        {
            var message = new ValidatedMessage<string>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsIdentifier(RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsIdentifier_ReturnsExpectedError_IfValueIsEmpty()
        {
            var message = new ValidatedMessage<string>(string.Empty);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsIdentifier(RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsIdentifier_ReturnsExpectedError_IfValueIsWhiteSpace()
        {
            var message = new ValidatedMessage<string>("    ");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsIdentifier(RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsIdentifier_ReturnsExpectedError_IfValueStartsWithDigit()
        {
            var message = new ValidatedMessage<string>("8abc");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsIdentifier(RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsIdentifier_ReturnsExpectedError_IfValueContainsIllegalCharacters()
        {
            var message = new ValidatedMessage<string>("abcde!@#$");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsIdentifier(RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsIdentifier_ReturnsExpectedError_IfValueContainsIllegalCharacters_And_NoErrorMessageIsSpecified()
        {
            var message = new ValidatedMessage<string>("abcde!@#$");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsIdentifier();

            validator.Validate(message).AssertMemberError("Member (abcde!@#$) could not be converted to an identifier.");
        }

        [TestMethod]
        public void ValidateIsIdentifier_ReturnsNoErrors_IfValueStartsWithUnderscore()
        {
            var message = new ValidatedMessage<string>("_abcd1234");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsIdentifier().IsNotNull();

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsIdentifier_ReturnsNoErrors_IfValueStartsWithLetter()
        {
            var message = new ValidatedMessage<string>("abcd1234");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsIdentifier().IsNotNull();

            validator.Validate(message).AssertNoErrors();
        }

        #endregion
    }
}
