using System;
using Kingo.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Constraints
{
    [TestClass]
    public sealed class GuidConstraintsTest : ConstraintTestBase
    {        
        #region [====== IsNotEmpty ======]

        [TestMethod]
        public void ValidateIsNotEmpty_ReturnsExpectedError_IfGuidIsEmpty()
        {
            var message = new ValidatedMessage<Guid>(Guid.Empty);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotEmpty(RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsNotEmpty_ReturnsDefaultError_IfGuidIsEmpty_And_NoErrorMessageIsSpecified()
        {
            var message = new ValidatedMessage<Guid>(Guid.Empty);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotEmpty();

            validator.Validate(message).AssertMemberError("Member (00000000-0000-0000-0000-000000000000) must not be empty.");
        }

        [TestMethod]
        public void ValidateIsNotEmpty_ReturnsNoErrors_IfGuidIsNotEmpty()
        {
            var message = new ValidatedMessage<Guid>(Guid.NewGuid());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotEmpty(RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        #endregion

        #region [====== IsEmpty ======]

        [TestMethod]
        public void ValidateIsEmpty_ReturnsNoErrors_IfGuidIsEmpty()
        {
            var message = new ValidatedMessage<Guid>(Guid.Empty);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsEmpty(RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsEmpty_ReturnsExpectedError_IfGuidIsNotEmpty()
        {
            var message = new ValidatedMessage<Guid>(Guid.NewGuid());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsEmpty(RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsEmpty_ReturnsDefaultError_IfGuidIsNotEmpty_And_NoErrorMessageIsSpecified()
        {
            var message = new ValidatedMessage<Guid>(Guid.NewGuid());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsEmpty();

            validator.Validate(message).AssertMemberError(string.Format("Member ({0}) must be empty.", message.Member));
        }

        #endregion
    }
}
