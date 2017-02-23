using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging.Constraints
{
    public sealed partial class StringConstraintsTest
    {
        #region [====== DoesNotContain ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ValidateDoesNotContain_Throws_IfSpecifiedValueIsNull()
        {
            var message = new ValidatedMessage<string>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).DoesNotContain(null, RandomErrorMessage);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ValidateDoesNotContain_Throws_IfMemberIsNull()
        {
            var message = new ValidatedMessage<string>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).DoesNotContain("x");

            validator.Validate(message);
        }

        [TestMethod]
        public void ValidateDoesNotContain_ReturnsNoErrors_IfValueDoesNotContainSpecifiedValue()
        {
            var message = new ValidatedMessage<string>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).DoesNotContain("xyz", RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateDoesNotContain_ReturnsExpectedError_IfValueContainsValue()
        {
            var message = new ValidatedMessage<string>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).DoesNotContain("e va", RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateDoesNotContain_ReturnsExpectedError_IfValueContainsValue_And_NoErrorMessageIsSpecified()
        {
            var message = new ValidatedMessage<string>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).DoesNotContain("e va");

            validator.Validate(message).AssertMemberError("Member (Some value) must not contain 'e va'.");
        }

        #endregion

        #region [====== Contains ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ValidateContains_Throws_IfSpecifiedValueIsNull()
        {
            var message = new ValidatedMessage<string>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).Contains(null, RandomErrorMessage);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ValidateContains_Throws_IfMemberIsNull()
        {
            var message = new ValidatedMessage<string>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).Contains("x");

            validator.Validate(message);
        }

        [TestMethod]
        public void ValidateContains_ReturnsExpectedError_IfValueDoesNotContainSpecifiedValue()
        {
            var message = new ValidatedMessage<string>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).Contains("xyz", RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateContains_ReturnsExpectedError_IfValueDoesNotContainSpecifiedValue_And_NoErrorMessageIsSpecified()
        {
            var message = new ValidatedMessage<string>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).Contains("xyz");

            validator.Validate(message).AssertMemberError("Member (Some value) must contain 'xyz'.");
        }

        [TestMethod]
        public void ValidateContains_ReturnsNoErrors_IfValueContainsSpecifiedValue()
        {
            var message = new ValidatedMessage<string>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).Contains("e va", RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        #endregion
    }
}
