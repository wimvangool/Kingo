using System;
using Kingo.BuildingBlocks.Clocks;
using Kingo.BuildingBlocks.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.BuildingBlocks.Constraints
{
    public sealed partial class BasicConstraintsTest
    {
        #region [====== IsNotInRange ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ValidateIsNotInRange_Throws_IfRangeIsNull()
        {
            var member = Clock.Current.UtcDateAndTime().Millisecond;
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInstanceOf<int>(RandomErrorMessage)
                .IsNotInRange(null as IRange<int>, RandomErrorMessage);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ValidateIsNotInRange_Throws_IfNeitherValueImplementsComparable()
        {
            var member = Clock.Current.UtcDateAndTime().Millisecond;
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsNotInRange(new object(), new object(), RandomErrorMessage);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ValidateIsNotInRange_Throws_IfRangeIsInvalid()
        {
            var member = Clock.Current.UtcDateAndTime().Millisecond;
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInstanceOf<int>(RandomErrorMessage)
                .IsNotInRange(member, member - 1, RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsNotInRange_ReturnsExpectedError_IfMemberIsInSpecifiedRange()
        {
            var member = Clock.Current.UtcDateAndTime().Millisecond;
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInstanceOf<int>(RandomErrorMessage)
                .IsNotInRange(0, 1000, RangeOptions.RightExclusive, RandomErrorMessage);

            validator.Validate(message).AssertError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsNotInRange_ReturnsDefaultError_IfMemberIsInSpecifiedRange_And_NoErrorMessageIsSpecified()
        {
            var member = Clock.Current.UtcDateAndTime().Millisecond;
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInstanceOf<int>(RandomErrorMessage)
                .IsNotInRange(0, 1000, RangeOptions.RightExclusive);

            validator.Validate(message).AssertError(string.Format("Member ({0}) must not be within the following range: [0, 1000>.", member));
        }

        [TestMethod]
        public void ValidateIsNotInRange_ReturnsNoErrors_IfMemberIsNotInSpecifiedRange()
        {
            var member = Clock.Current.UtcDateAndTime().Millisecond;
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInstanceOf<int>(RandomErrorMessage)
                .IsNotInRange(1000, 2000, RangeOptions.RightExclusive, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        #endregion

        #region [====== IsNotInRange (Indirect) ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ValidateIsNotInRange_Throws_IfNeitherValueImplementsComparable_Indirect()
        {
            var member = Clock.Current.UtcDateAndTime().Millisecond;
            var message = new ValidatedMessage<object>(member, new object(), new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotInRange(m => m.Left, m => m.Right, RandomErrorMessage);

            validator.Validate(message);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ValidateIsNotInRange_Throws_IfRangeIsInvalid_Indirect()
        {
            var member = Clock.Current.UtcDateAndTime().Millisecond;
            var message = new ValidatedMessage<int>(member, member, member - 1);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotInRange(m => m.Left, m => m.Right, RandomErrorMessage);

            validator.Validate(message);
        }

        [TestMethod]
        public void ValidateIsNotInRange_ReturnsExpectedError_IfMemberIsInSpecifiedRange_Indirect()
        {
            var member = Clock.Current.UtcDateAndTime().Millisecond;
            var message = new ValidatedMessage<int>(member, 0, 1000);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotInRange(m => m.Left, m => m.Right, RangeOptions.RightExclusive, RandomErrorMessage);

            validator.Validate(message).AssertError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsNotInRange_ReturnsDefaultError_IfMemberIsInSpecifiedRange_And_NoErrorMessageIsSpecified_Indirect()
        {
            var member = Clock.Current.UtcDateAndTime().Millisecond;
            var message = new ValidatedMessage<int>(member, 0, 1000);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotInRange(m => m.Left, m => m.Right, RangeOptions.RightExclusive);

            validator.Validate(message).AssertError(string.Format("Member ({0}) must not be within the following range: [0, 1000>.", member));
        }

        [TestMethod]
        public void ValidateIsNotInRange_ReturnsNoErrors_IfMemberIsNotInSpecifiedRange_Indirect()
        {
            var member = Clock.Current.UtcDateAndTime().Millisecond;
            var message = new ValidatedMessage<int>(member, 1000, 2000);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotInRange(m => m.Left, m => m.Right, RangeOptions.RightExclusive, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        #endregion

        #region [====== IsInRange ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ValidateIsInRange_Throws_IfRangeIsNull()
        {
            var member = Clock.Current.UtcDateAndTime().Millisecond;
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInstanceOf<int>(RandomErrorMessage)
                .IsInRange(null as IRange<int>, RandomErrorMessage);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ValidateIsInRange_Throws_IfNeitherValueImplementsComparable()
        {
            var member = Clock.Current.UtcDateAndTime().Millisecond;
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInRange(new object(), new object(), RandomErrorMessage);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ValidateIsInRange_Throws_IfRangeIsInvalid()
        {
            var member = Clock.Current.UtcDateAndTime().Millisecond;
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInstanceOf<int>(RandomErrorMessage)
                .IsInRange(member, member - 1, RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsInRange_ReturnsNoErrors_IfMemberIsInSpecifiedRange()
        {
            var member = Clock.Current.UtcDateAndTime().Millisecond;
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInstanceOf<int>(RandomErrorMessage)
                .IsInRange(0, 1000, RangeOptions.RightExclusive, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsInRange_ReturnsExpectedError_IfMemberIsNotInSpecifiedRange()
        {
            var member = Clock.Current.UtcDateAndTime().Millisecond;
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInstanceOf<int>(RandomErrorMessage)
                .IsInRange(1000, 2000, RangeOptions.RightExclusive, RandomErrorMessage);

            validator.Validate(message).AssertError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsInRange_ReturnsDefaultError_IfMemberIsNotInSpecifiedRange_And_NoErrorMessageIsSpecified()
        {
            var member = Clock.Current.UtcDateAndTime().Millisecond;
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInstanceOf<int>(RandomErrorMessage)
                .IsInRange(1000, 2000, RangeOptions.RightExclusive);

            validator.Validate(message).AssertError(string.Format("Member ({0}) must be within the following range: [1000, 2000>.", member));
        }

        #endregion

        #region [====== IsInRange (Indirect) ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ValidateIsInRange_Throws_IfNeitherValueImplementsComparable_Indirect()
        {
            var member = Clock.Current.UtcDateAndTime().Millisecond;
            var message = new ValidatedMessage<object>(member, new object(), new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsInRange(m => m.Left, m => m.Right);

            validator.Validate(message);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ValidateIsInRange_Throws_IfRangeIsInvalid_Indirect()
        {
            var member = Clock.Current.UtcDateAndTime().Millisecond;
            var message = new ValidatedMessage<int>(member, member, member - 1);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsInRange(m => m.Left, m => m.Right);

            validator.Validate(message);
        }

        [TestMethod]
        public void ValidateIsInRange_ReturnsNoErrors_IfMemberIsInSpecifiedRange_Indirect()
        {
            var member = Clock.Current.UtcDateAndTime().Millisecond;
            var message = new ValidatedMessage<int>(member, 0, 1000);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsInRange(m => m.Left, m => m.Right, RangeOptions.RightExclusive, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsInRange_ReturnsExpectedError_IfMemberIsNotInSpecifiedRange_Indirect()
        {
            var member = Clock.Current.UtcDateAndTime().Millisecond;
            var message = new ValidatedMessage<int>(member, 1000, 2000);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsInRange(m => m.Left, m => m.Right, RangeOptions.RightExclusive, RandomErrorMessage);

            validator.Validate(message).AssertError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsInRange_ReturnsDefaultError_IfMemberIsNotInSpecifiedRange_And_NoErrorMessageIsSpecified_Indirect()
        {
            var member = Clock.Current.UtcDateAndTime().Millisecond;
            var message = new ValidatedMessage<int>(member, 1000, 2000);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsInRange(m => m.Left, m => m.Right, RangeOptions.RightExclusive);

            validator.Validate(message).AssertError(string.Format("Member ({0}) must be within the following range: [1000, 2000>.", member));
        }

        #endregion
    }
}
