using System;
using Kingo.BuildingBlocks.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.BuildingBlocks.Constraints
{
    public sealed partial class StringConstraintsTest
    {
        #region [====== HasLengthOf ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ValidateHasLengthOf_Throws_IfLengthIsNegative()
        {
            var message = new ValidatedMessage<string>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).HasLengthOf(-1);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void ValidateHasLengthOf_Throws_IfMemberIsNull()
        {
            var message = new ValidatedMessage<string>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).HasLengthOf(10);

            validator.Validate(message);
        }

        [TestMethod]
        public void ValidateHasLengthOf_ReturnsExpectedError_IfValueDoesNotHaveSpecifiedLength()
        {
            var message = new ValidatedMessage<string>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).HasLengthOf(9, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateHasLengthOf_ReturnsDefaultError_IfValueDoesNotHaveSpecifiedLength_And_NoErrorMessageIsSpecified()
        {
            var message = new ValidatedMessage<string>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).HasLengthOf(9);

            validator.Validate(message).AssertOneError("Member (Some value) must have a length of 9 character(s).");
        }

        [TestMethod]
        public void ValidateHasLengthOf_ReturnsNoErrors_IfValueHasSpecifiedLength()
        {
            var message = new ValidatedMessage<string>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).HasLengthOf(10, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        #endregion

        #region [====== HasLengthBetween ======]

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void ValidateHasLengthBetween_Throws_IfMemberIsNull()
        {
            var message = new ValidatedMessage<string>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).HasLengthBetween(0, 10);

            validator.Validate(message);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ValidateHasLengthBetween_Throws_IfMaximumIsNegative()
        {
            var message = new ValidatedMessage<string>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).HasLengthBetween(-2, -1, RandomErrorMessage);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ValidateHasLengthBetween_Throws_IfMaximumIsSmallerThanMinimum()
        {
            var message = new ValidatedMessage<string>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).HasLengthBetween(8, 7, RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateHasLengthBetween_ReturnsExpectedError_IfLengthOfValueIsNotInSpecifiedRange()
        {
            var message = new ValidatedMessage<string>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).HasLengthBetween(0, 9, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateHasLengthBetween_ReturnsDefaultError_IfLengthOfValueIsNotInSpecifiedRange_And_NoErrorMessageIsSpecified()
        {
            var message = new ValidatedMessage<string>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).HasLengthBetween(0, 9);

            validator.Validate(message).AssertOneError("Member's Length (Some value) must be within range [0, 9].");
        }

        [TestMethod]
        public void ValidateHasLengthBetween_ReturnsNoErrors_IfLengthOfValueIsInSpecifiedRange()
        {
            var message = new ValidatedMessage<string>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).HasLengthBetween(9, 11, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        #endregion
    }
}
