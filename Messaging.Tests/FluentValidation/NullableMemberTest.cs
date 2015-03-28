using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel.FluentValidation
{
    [TestClass]
    public sealed class NullableMemberTest
    {
        private string _errorMessage;

        [TestInitialize]
        public void Setup()
        {
            _errorMessage = Guid.NewGuid().ToString("N");
        }

        #region [====== NotNull ======]

        [TestMethod]
        public void ValidateIsNotNull_ReturnsExpectedError_IfMemberIsNull()
        {
            var message = new ValidatedMessage<int?>(null);
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).HasValue(_errorMessage);

            validator.Validate().AssertOneError(_errorMessage);
        }

        [TestMethod]
        public void ValidateIsNotNull_ReturnsNoErrors_IfMemberIsNotNull()
        {
            var member = Clock.Current.UtcDateAndTime().Second;
            var message = new ValidatedMessage<int?>(member);
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).HasValue(_errorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsNotNull_ReturnsExpectedValue_IfMemberIsNotNull()
        {
            var member = Clock.Current.UtcDateAndTime().Second;
            var message = new ValidatedMessage<int?>(member);
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member)
                .HasValue(_errorMessage)
                .IsEqualTo(member, _errorMessage);

            validator.Validate().AssertNoErrors();
        }

        #endregion
    }
}
