using Kingo.BuildingBlocks.Clocks;
using Kingo.BuildingBlocks.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.BuildingBlocks.Constraints
{
    [TestClass]
    public sealed class NullableConstraintsTest : ConstraintTestBase
    {        
        #region [====== NotNull ======]

        [TestMethod]
        public void ValidateIsNotNull_ReturnsExpectedError_IfMemberIsNull()
        {
            var message = new ValidatedMessage<int?>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotNull(RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsNotNull_ReturnsDefaultError_IfMemberIsNull_And_NoErrorMessageIsSpecified()
        {
            var message = new ValidatedMessage<int?>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotNull();

            validator.Validate(message).AssertMemberError("Member (<null>) must have a value.");
        }

        [TestMethod]
        public void ValidateIsNotNull_ReturnsNoErrors_IfMemberIsNotNull()
        {
            var member = Clock.Current.UtcDateAndTime().Second;
            var message = new ValidatedMessage<int?>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotNull(RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsNotNull_ReturnsExpectedValue_IfMemberIsNotNull()
        {
            var member = Clock.Current.UtcDateAndTime().Second;
            var message = new ValidatedMessage<int?>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsNotNull(RandomErrorMessage)
                .IsEqualTo(member, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        #endregion
    }
}
