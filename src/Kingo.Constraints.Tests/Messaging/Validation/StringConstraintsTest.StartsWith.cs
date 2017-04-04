﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging.Validation
{
    public sealed partial class StringConstraintsTest
    {
        #region [====== DoesNotStartWith ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ValidateDoesNotStartWith_Throws_IfPrefixIsNull()
        {
            var message = new ValidatedMessage<string>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).DoesNotStartWith(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ValidateDoesNotStartWith_Throws_IfMemberIsNull()
        {
            var message = new ValidatedMessage<string>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).DoesNotStartWith("SOME");

            validator.Validate(message);
        }

        [TestMethod]
        public void ValidateDoesNotStartWith_ReturnsExpectedError_IfValueStartsWithSpecifiedPrefix_OrdinalIgnoreCase()
        {
            var message = new ValidatedMessage<string>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).DoesNotStartWith("SOME", StringComparison.OrdinalIgnoreCase, RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateDoesNotStartWith_ReturnsDefaultError_IfValueStartsWithSpecifiedPrefix_OrdinalIgnoreCase_And_NoErrorMessageIsSpecified()
        {
            var message = new ValidatedMessage<string>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).DoesNotStartWith("SOME", StringComparison.OrdinalIgnoreCase);

            validator.Validate(message).AssertMemberError("Member (Some value) must not start with 'SOME'.");
        }

        [TestMethod]
        public void ValidateDoesNotStartWith_ReturnsNoErrors_IfValueDoesNotStartWithSpecifiedPrefix_OrdinalIgnoreCase()
        {
            var message = new ValidatedMessage<string>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).DoesNotStartWith("value", StringComparison.OrdinalIgnoreCase, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        #endregion

        #region [====== StartsWith ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ValidateStartsWith_Throws_IfPrefixIsNull()
        {
            var message = new ValidatedMessage<string>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).StartsWith(null);
        }

        [TestMethod]
        public void ValidateStartsWith_ReturnsNoErrors_IfValueStartsWithSpecifiedPrefix()
        {
            var message = new ValidatedMessage<string>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).StartsWith("Some", RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ValidateStartsWith_Throws_IfValueIsNull()
        {
            var message = new ValidatedMessage<string>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).StartsWith("SOME");

            validator.Validate(message);
        }

        [TestMethod]
        public void ValidateStartsWith_ReturnsExpectedError_IfValueDoesNotStartWithSpecifiedPrefix()
        {
            var message = new ValidatedMessage<string>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).StartsWith("SOME", RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateStartsWith_ReturnsExpectedError_IfValueDoesNotStartWithSpecifiedPrefix_And_NoErrorMessageIsSpecified()
        {
            var message = new ValidatedMessage<string>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).StartsWith("SOME");

            validator.Validate(message).AssertMemberError("Member (Some value) must start with 'SOME'.");
        }

        [TestMethod]
        public void ValidateStartsWith_ReturnsNoErrors_IfValueStartsWithSpecifiedPrefix_OrdinalIgnoreCase()
        {
            var message = new ValidatedMessage<string>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).StartsWith("SOME", StringComparison.OrdinalIgnoreCase, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateStartsWith_ReturnsNoErrors_IfValueDoesNotStartWithSpecifiedPrefix_OrdinalIgnoreCase()
        {
            var message = new ValidatedMessage<string>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).StartsWith("value", StringComparison.OrdinalIgnoreCase, RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        #endregion
    }
}