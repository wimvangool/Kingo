using System;
using Kingo.BuildingBlocks.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.BuildingBlocks.Constraints
{
    public sealed partial class StringConstraintsTest
    {
        #region [====== Length ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Length_Throws_IfArrayIsNull()
        {
            var message = new ValidatedMessage<string>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).Length();

            validator.Validate(message);
        }

        [TestMethod]
        public void LengthIsEqualTo_ReturnsExpectedErrorMessage_IfLengthIsNotEqualToSpecifiedValue()
        {
            var message = new ValidatedMessage<string>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).Length().IsEqualTo(3);

            validator.Validate(message).AssertOneError("Member.Length (10) must be equal to '3'.");
        }

        [TestMethod]
        public void LengthIsEqualTo_ReturnsNoError_IfLengthIsEqualToSpecifiedValue()
        {
            var message = new ValidatedMessage<string>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).Length().IsEqualTo(10);

            validator.Validate(message).AssertNoErrors();
        }

        #endregion
    }
}
