using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging.Validation.Constraints
{
    public sealed partial class StringConstraintsTest
    {
        #region [====== IsNotNullOrWhiteSpace ======]

        [TestMethod]
        public void ValidateIsNotNullOrWhiteSpace_ReturnsNoErrors_IfStringIsNotNullOrWhiteSpace()
        {            
            var message = NewValidatedMessage();
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotNullOrWhiteSpace(RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();           
        }

        [TestMethod]
        public void ValidateIsNotNullOrWhiteSpace_ReturnsExpectedError_IfStringIsNull()
        {            
            var message = new ValidatedMessage<string>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotNullOrWhiteSpace(RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);            
        }

        [TestMethod]
        public void ValidateIsNotNullOrWhiteSpace_ReturnsExpectedError_IfStringIsEmpty()
        {            
            var message = new ValidatedMessage<string>(string.Empty);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotNullOrWhiteSpace(RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);            
        }

        [TestMethod]
        public void ValidateIsNotNullOrWhiteSpace_ReturnsExpectedError_IfStringIsWhiteSpace()
        {            
            var message = new ValidatedMessage<string>("     ");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotNullOrWhiteSpace(RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);            
        }

        [TestMethod]
        public void ValidateIsNotNullOrWhiteSpace_ReturnsDefaultError_IfStringIsWhiteSpace_And_NoErrorMessageIsSpecified()
        {
            var message = new ValidatedMessage<string>("     ");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotNullOrWhiteSpace();

            validator.Validate(message).AssertMemberError("Member (     ) is not allowed to be null or contain only white space.");
        }

        #endregion

        #region [====== IsNullOrWhiteSpace ======]

        [TestMethod]
        public void ValidateIsNullOrWhiteSpace_ReturnsNoErrors_IfStringIsNull()
        {
            var message = new ValidatedMessage<string>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNullOrWhiteSpace();

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsNullOrWhiteSpace_ReturnsNoErrors_IfStringIsEmpty()
        {
            var message = new ValidatedMessage<string>(string.Empty);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNullOrWhiteSpace();

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsNullOrWhiteSpace_ReturnsNoErrors_IfStringIsWhiteSpace()
        {
            var message = new ValidatedMessage<string>("     ");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNullOrWhiteSpace();

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsNullOrWhiteSpace_ReturnsExpectedError_IfStringHasNonWhiteSpaceValue()
        {
            var message = NewValidatedMessage();
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNullOrWhiteSpace(RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsNullOrWhiteSpace_ReturnsDefaultError_IfStringHasNonWhiteSpaceValue_And_NoErrorMessageIsSpecified()
        {
            var message = NewValidatedMessage();
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNullOrWhiteSpace();

            validator.Validate(message).AssertMemberError(string.Format("Member ({0}) must be either null or contain only white space.", message.Member));
        }

        #endregion
    }
}
