using System;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.BuildingBlocks.Constraints
{
    public sealed partial class StringConstraintsTest
    {
        #region [====== DoesNotMatch ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ValidateDoesNotMatch_Throws_IfPatternIsNull()
        {
            var message = new ValidatedMessage<string>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).DoesNotMatch(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ValidateDoesNotMatch_Throws_IfMemberIsNull()
        {
            var message = new ValidatedMessage<string>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).DoesNotMatch("x");

            validator.Validate(message);
        }

        [TestMethod]
        public void ValidateDoesNotMatch_ReturnsExpectedError_IfValueMatchesSpecifiedPattern()
        {
            var message = new ValidatedMessage<string>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).DoesNotMatch("v.l", RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateDoesNotMatch_ReturnsDefaultError_IfValueMatchesSpecifiedPattern_And_NoErrorMessageIsSpecified()
        {
            var message = new ValidatedMessage<string>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).DoesNotMatch("v.l");

            validator.Validate(message).AssertOneError("Member (Some value) must not match pattern 'v.l'.");
        }

        [TestMethod]
        public void ValidateDoesNotMatch_ReturnsNoErrors_IfValueDoesNotMatchSpecifiedPattern()
        {
            var message = new ValidatedMessage<string>("Some VALUE");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).DoesNotMatch("v.l", RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateDoesNotMatch_ReturnsExpectedError_IfValueMatchesSpecifiedPattern_IgnoreCase()
        {
            var message = new ValidatedMessage<string>("Some VALUE");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).DoesNotMatch("v.l", RegexOptions.IgnoreCase, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateDoesNotMatch_ReturnsNoErrors_IfValueDoesNotMatchSpecifiedPattern_IgnoreCase()
        {
            var message = new ValidatedMessage<string>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).DoesNotMatch("valeu", RegexOptions.IgnoreCase, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        #endregion

        #region [====== Matches ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ValidateMatches_Throws_IfPatternIsNull()
        {
            var message = new ValidatedMessage<string>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).Matches(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ValidateMatches_Throws_IfMemberIsNull()
        {
            var message = new ValidatedMessage<string>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).Matches("x");

            validator.Validate(message);
        }

        [TestMethod]
        public void ValidateMatches_ReturnsNoErrors_IfValueMatchesSpecifiedPattern()
        {
            var message = new ValidatedMessage<string>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).Matches("v.l", RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateMatches_ReturnsExpectedError_IfValueDoesNotMatchSpecifiedPattern()
        {
            var message = new ValidatedMessage<string>("Some VALUE");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).Matches("v.l", RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateMatches_ReturnsDefaultError_IfValueDoesNotMatchSpecifiedPattern()
        {
            var message = new ValidatedMessage<string>("Some VALUE");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).Matches("v.l");

            validator.Validate(message).AssertOneError("Member (Some VALUE) must match pattern 'v.l'.");
        }

        [TestMethod]
        public void ValidateMatches_ReturnsNoErrors_IfValueMatchesSpecifiedPattern_IgnoreCase()
        {
            var message = new ValidatedMessage<string>("Some VALUE");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).Matches("v.l", RegexOptions.IgnoreCase, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateMatches_ReturnsExpectedError_IfValueDoesNotMatchSpecifiedPattern_IgnoreCase()
        {
            var message = new ValidatedMessage<string>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).Matches("valeu", RegexOptions.IgnoreCase, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        #endregion
    }
}
