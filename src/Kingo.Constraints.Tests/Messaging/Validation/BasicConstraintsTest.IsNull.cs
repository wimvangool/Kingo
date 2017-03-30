using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging.Validation
{
    public sealed partial class BasicConstraintsTest
    {
        #region [====== IsNotNull ======]

        [TestMethod]
        public void ValidateIsNotNull_ReturnsNoErrors_IfObjectIsNotNull()
        {
            var message = new ValidatedMessage<object>(new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotNull(RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsNotNull_ReturnsExpectedError_IfObjectIsNull()
        {
            var message = new ValidatedMessage<object>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotNull(RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsNotNull_ReturnsDefaultError_IfObjectIsNull_And_NoErrorMessageIsSpecified()
        {
            var message = new ValidatedMessage<object>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotNull();

            validator.Validate(message).AssertMemberError("Member (<null>) must refer to an instance of an object.");
        }

        #endregion

        #region [====== IsNull ======]

        [TestMethod]
        public void ValidateIsNull_ReturnsNoErrors_IfObjectIsNull()
        {
            var message = new ValidatedMessage<object>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNull(RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsNull_ReturnsExpectedError_IfObjectIsNotNull()
        {
            var message = new ValidatedMessage<object>(new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNull(RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsNull_ReturnsDefaultError_IfObjectIsNotNull_And_NoErrorMessageIsSpecified()
        {
            var message = new ValidatedMessage<object>(new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNull();

            validator.Validate(message).AssertMemberError("Member (System.Object) must be null.");
        }

        #endregion
    }
}
