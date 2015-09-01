using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.BuildingBlocks.ComponentModel.Constraints
{
    [TestClass]
    public sealed class NullableConstraintTest : ConstraintTest
    {        
        #region [====== NotNull ======]

        [TestMethod]
        public void ValidateIsNotNull_ReturnsExpectedError_IfMemberIsNull()
        {
            var message = new ValidatedMessage<int?>(null);
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).HasValue(RandomErrorMessage);

            validator.Validate().AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsNotNull_ReturnsDefaultError_IfMemberIsNull_And_NoErrorMessageIsSpecified()
        {
            var message = new ValidatedMessage<int?>(null);
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).HasValue();

            validator.Validate().AssertOneError("Member (<null>) must have a value.");
        }

        [TestMethod]
        public void ValidateIsNotNull_ReturnsNoErrors_IfMemberIsNotNull()
        {
            var member = Clock.Current.UtcDateAndTime().Second;
            var message = new ValidatedMessage<int?>(member);
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).HasValue(RandomErrorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsNotNull_ReturnsExpectedValue_IfMemberIsNotNull()
        {
            var member = Clock.Current.UtcDateAndTime().Second;
            var message = new ValidatedMessage<int?>(member);
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member)
                .HasValue(RandomErrorMessage)
                .IsEqualTo(member, RandomErrorMessage);

            validator.Validate().AssertNoErrors();
        }

        #endregion
    }
}
