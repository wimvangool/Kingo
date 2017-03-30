using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging.Validation
{
    public sealed partial class StringConstraintsTest
    {
        #region [====== DoesNotEndWith ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ValidateDoesNotEndWith_Throws_IfPostfixIsNull()
        {
            var message = new ValidatedMessage<string>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).DoesNotEndWith(null);

            validator.Validate(message);
        }

        [TestMethod]
        public void ValidateDoesNotEndWith_ReturnsExpectedError_IfValueEndsWithSpecifiedPostfix()
        {
            var message = new ValidatedMessage<string>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).DoesNotEndWith("value", RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateDoesNotEndWith_ReturnsDefaultError_IfValueEndsWithSpecifiedPostfix_And_NoErrorMessageIsSpecified()
        {
            var message = new ValidatedMessage<string>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).DoesNotEndWith("value");

            validator.Validate(message).AssertMemberError("Member (Some value) must not end with 'value'.");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ValidateDoesNotEndWith_Throws_IfMemberIsNull()
        {
            var message = new ValidatedMessage<string>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).DoesNotEndWith("value");

            validator.Validate(message);
        }

        [TestMethod]
        public void ValidateDoesNotEndWith_ReturnsNoErrors_IfValueDoesNotEndWithSpecifiedPostfix()
        {
            var message = new ValidatedMessage<string>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).DoesNotEndWith("VALUE");

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateDoesNotEndWith_ReturnsExpectedErrors_IfValueEndsWithSpecifiedPostfix_OrdinalIgnoreCase()
        {
            var message = new ValidatedMessage<string>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).DoesNotEndWith("VALUE", StringComparison.OrdinalIgnoreCase, RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        #endregion

        #region [====== EndsWith ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ValidateEndsWith_Throws_IfPostfixIsNull()
        {
            var message = new ValidatedMessage<string>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).EndsWith(null);

            validator.Validate(message);
        }

        [TestMethod]
        public void ValidateEndsWith_ReturnsNoErrors_IfValueEndsWithSpecifiedPostfix()
        {
            var message = new ValidatedMessage<string>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).EndsWith("value", RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ValidateEndsWith_Throws_IfMemberIsNull()
        {
            var message = new ValidatedMessage<string>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).EndsWith("value");

            validator.Validate(message);
        }

        [TestMethod]
        public void ValidateEndsWith_ReturnsExpectedError_IfValueDoesNotEndWithSpecifiedPostfix()
        {
            var message = new ValidatedMessage<string>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).EndsWith("VALUE", RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateEndsWith_ReturnsExpectedError_IfValueDoesNotEndWithSpecifiedPostfix_And_NoErrorMessageIsSpecified()
        {
            var message = new ValidatedMessage<string>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).EndsWith("VALUE");

            validator.Validate(message).AssertMemberError("Member (Some value) must end with 'VALUE'.");
        }

        [TestMethod]
        public void ValidateEndsWith_ReturnsNoErrors_IfValueEndsWithSpecifiedPostfix_OrdinalIgnoreCase()
        {
            var message = new ValidatedMessage<string>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).EndsWith("VALUE", StringComparison.OrdinalIgnoreCase, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateEndsWith_ReturnsNoErrors_IfValueDoesNotEndWithSpecifiedPostfix_OrdinalIgnoreCase()
        {
            var message = new ValidatedMessage<string>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).EndsWith("Some", StringComparison.OrdinalIgnoreCase, RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        #endregion
    }    
}
