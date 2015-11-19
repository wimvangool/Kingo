using Kingo.BuildingBlocks.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.BuildingBlocks.Constraints
{
    [TestClass]
    public sealed class BooleanConstraintsTest : ConstraintTestBase
    {
        #region [====== IsTrue ======]

        [TestMethod]
        public void ValidateIsTrue_ReturnsExpectedError_IfMemberIsFalse()
        {
            var message = new ValidatedMessage<bool>(false);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsTrue(RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsTrue_ReturnsDefaultError_IfMemberIsFalse_And_NoErrorMessageIsSpecified()
        {
            var message = new ValidatedMessage<bool>(false);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsTrue();

            validator.Validate(message).AssertMemberError("Member (false) must be true.");
        }

        [TestMethod]
        public void ValidateIsTrue_ReturnsNoErrors_IfMemberIsTrue()
        {
            var message = new ValidatedMessage<bool>(true);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsTrue(RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        #endregion

        #region [====== IsFalse ======]

        [TestMethod]
        public void ValidateIsFalse_ReturnsExpectedError_IfMemberIsTrue()
        {
            var message = new ValidatedMessage<bool>(true);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsFalse(RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsFalse_ReturnsDefaultError_IfMemberIsTrue_And_NoErrorMessageIsSpecified()
        {
            var message = new ValidatedMessage<bool>(true);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsFalse();

            validator.Validate(message).AssertMemberError("Member (true) must be false.");
        }

        [TestMethod]
        public void ValidateIsFalse_ReturnsNoErrors_IfMemberIsFalse()
        {
            var message = new ValidatedMessage<bool>(false);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsFalse(RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        #endregion
    }
}
