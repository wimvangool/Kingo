using Kingo.BuildingBlocks.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.BuildingBlocks.Constraints
{
    public sealed partial class StringConstraintsTest
    {
        #region [====== IsNotNullOrEmpty ======]

        [TestMethod]
        public void ValidateIsNotNullOrEmpty_ReturnsNoErrors_IfStringIsNotNullOrEmpty()
        {
            var message = NewValidatedMessage();
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotNullOrEmpty(RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsNotNullOrEmpty_ReturnsExpectedError_IfStringIsNull()
        {
            var message = new ValidatedMessage<string>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotNullOrEmpty(RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsNotNullOrEmpty_ReturnsExpectedError_IfStringIsEmpty()
        {
            var message = new ValidatedMessage<string>(string.Empty);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotNullOrEmpty(RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsNotNullOrEmpty_ReturnsDefaultError_IfStringIsEmpty_And_NoErrorMessageIsSpecified()
        {
            var message = new ValidatedMessage<string>(string.Empty);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotNullOrEmpty();

            validator.Validate(message).AssertOneError("Member () is not allowed to be null or empty.");
        }

        #endregion

        #region [====== IsNullOrEmpty ======]

        [TestMethod]
        public void ValidateIsNullOrEmpty_ReturnsNoErrors_IfStringIsNull()
        {
            var message = new ValidatedMessage<string>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNullOrEmpty();

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsNullOrEmpty_ReturnsNoErrors_IfStringIsEmpty()
        {
            var message = new ValidatedMessage<string>(string.Empty);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNullOrEmpty();

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsNullOrEmpty_ReturnsExpectedError_IfStringHasNonEmptyValue()
        {
            var message = NewValidatedMessage();
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNullOrEmpty(RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsNullOrEmpty_ReturnsDefaultError_IfStringHasNonEmptyValue_And_NoErrorMessageIsSpecified()
        {
            var message = NewValidatedMessage();
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNullOrEmpty();

            validator.Validate(message).AssertOneError(string.Format("Member ({0}) must be either null or empty.", message.Member));
        }

        #endregion
    }
}
