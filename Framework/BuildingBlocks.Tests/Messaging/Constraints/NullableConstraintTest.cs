using Kingo.BuildingBlocks.Clocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.BuildingBlocks.Messaging.Constraints
{
    [TestClass]
    public sealed class NullableConstraintTest : ConstraintTest
    {        
        #region [====== NotNull ======]

        [TestMethod]
        public void ValidateIsNotNull_ReturnsExpectedError_IfMemberIsNull()
        {
            var message = new ValidatedMessage<int?>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).HasValue(RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsNotNull_ReturnsDefaultError_IfMemberIsNull_And_NoErrorMessageIsSpecified()
        {
            var message = new ValidatedMessage<int?>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).HasValue();

            validator.Validate(message).AssertOneError("Member (<null>) must have a value.");
        }

        [TestMethod]
        public void ValidateIsNotNull_ReturnsNoErrors_IfMemberIsNotNull()
        {
            var member = Clock.Current.UtcDateAndTime().Second;
            var message = new ValidatedMessage<int?>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).HasValue(RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsNotNull_ReturnsExpectedValue_IfMemberIsNotNull()
        {
            var member = Clock.Current.UtcDateAndTime().Second;
            var message = new ValidatedMessage<int?>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .HasValue(RandomErrorMessage)
                .IsEqualTo(member, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        #endregion
    }
}
