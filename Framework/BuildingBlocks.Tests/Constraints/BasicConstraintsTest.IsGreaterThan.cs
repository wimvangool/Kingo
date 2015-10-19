using System.Collections.Generic;
using Kingo.BuildingBlocks.Clocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.BuildingBlocks.Constraints
{
    public sealed partial class BasicConstraintsTest
    {
        #region [====== IsGreaterThan ======]

        [TestMethod]
        public void ValidateIsGreaterThanByComparer_ReturnsExpectedError_IfMemberIsSmallerThanOtherValue_And_ComparerIsNull()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsGreaterThan(member + 1, null, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsGreaterThanByComparer_ReturnsExpectedError_IfMemberIsEqualToOtherValue_And_ComparerIsNull()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsGreaterThan(member, null, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsGreaterThanByComparer_ReturnsDefaultError_IfMemberIsEqualToOtherValue_And_ComparerIsNull_And_NoErrorMessageIsSpecified()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsGreaterThan(member, null);

            validator.Validate(message).AssertOneError(string.Format("Member ({0}) must be greater than '{0}'.", member));
        }

        [TestMethod]
        public void ValidateIsGreaterThanByComparer_ReturnsNoErrors_IfMemberIsGreaterThanOtherValue_And_ComparerIsNull()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsGreaterThan(member - 1, null, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsGreaterThanByComparer_ReturnsNoErrors_IfComparerReturnsNegativeNumber()
        {
            // A negative result means that the specified value is smaller than the member.
            var comparer = new ComparerStub<object>(-1);
            var message = new ValidatedMessage<object>(new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsGreaterThan(new object(), comparer, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsGreaterThanByComparer_ReturnsExpectedError_IfComparerReturnsZero()
        {
            // Zero means that the specified value is equal to the member.
            var comparer = new ComparerStub<object>(0);
            var message = new ValidatedMessage<object>(new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsGreaterThan(new object(), comparer, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsGreaterThanByComparer_ReturnsExpectedError_IfComparerReturnsPositiveNumber()
        {
            // A positive result means that the specified value is greater than the member.
            var comparer = new ComparerStub<object>(1);
            var message = new ValidatedMessage<object>(new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsGreaterThan(new object(), comparer, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsGreaterThan_ReturnsExpectedError_IfMemberIsGreaterThanOtherValue()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsGreaterThan(member + 1, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsGreaterThan_ReturnsExpectedError_IfMemberIsEqualToOtherValue()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsGreaterThan(member, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsGreaterThan_ReturnsNoErrors_IfMemberIsGreaterThanOtherValue()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsGreaterThan(member - 1, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        #endregion

        #region [====== IsGreaterThan (Indirect) ======]

        [TestMethod]
        public void ValidateIsGreaterThanByComparer_ReturnsExpectedError_IfMemberIsSmallerThanOtherValue_And_ComparerIsNull_Indirect()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<long>(member, member + 1);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsGreaterThan(m => m.Other, null, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsGreaterThanByComparer_ReturnsExpectedError_IfMemberIsEqualToOtherValue_And_ComparerIsNull_Indirect()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<long>(member, member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsGreaterThan(m => m.Other, null, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsGreaterThanByComparer_ReturnsDefaultError_IfMemberIsEqualToOtherValue_And_ComparerIsNull_And_NoErrorMessageIsSpecified_Indirect()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<long>(member, member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsGreaterThan(m => m.Other, null);

            validator.Validate(message).AssertOneError(string.Format("Member ({0}) must be greater than 'Other ({0})'.", member));
        }

        [TestMethod]
        public void ValidateIsGreaterThanByComparer_ReturnsNoErrors_IfMemberIsGreaterThanOtherValue_And_ComparerIsNull_Indirect()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<long>(member, member - 1);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsGreaterThan(m => m.Other, null, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsGreaterThanByComparer_ReturnsNoErrors_IfComparerReturnsNegativeNumber_Indirect()
        {
            // A negative result means that the specified value is smaller than the member.
            var comparer = new ComparerStub<object>(-1);
            var message = new ValidatedMessage<object>(new object(), new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsGreaterThan(m => m.Other, comparer, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsGreaterThanByComparer_ReturnsExpectedError_IfComparerReturnsZero_Indirect()
        {
            // Zero means that the specified value is equal to the member.
            var comparer = new ComparerStub<object>(0);
            var message = new ValidatedMessage<object>(new object(), new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsGreaterThan(m => m.Other, comparer, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsGreaterThanByComparer_ReturnsExpectedError_IfComparerReturnsPositiveNumber_Indirect()
        {
            // A positive result means that the specified value is greater than the member.
            var comparer = new ComparerStub<object>(1);
            var message = new ValidatedMessage<object>(new object(), new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsGreaterThan(m => m.Other, comparer, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsGreaterThan_ReturnsExpectedError_IfMemberIsGreaterThanOtherValue_Indirect()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<long>(member, member + 1);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsGreaterThan(m => m.Other, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsGreaterThan_ReturnsExpectedError_IfMemberIsEqualToOtherValue_Indirect()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<long>(member, member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsGreaterThan(m => m.Other, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsGreaterThan_ReturnsNoErrors_IfMemberIsGreaterThanOtherValue_Indirect()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<long>(member, member - 1);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsGreaterThan(m => m.Other, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        #endregion

        #region [====== IsGreaterThanOrEqualTo ======]

        [TestMethod]
        public void ValidateIsGreaterThanOrEqualToByComparer_ReturnsExpectedErrors_IfMemberIsSmallerThanOtherValue_And_ComparerIsNull()
        {
            IComparer<long> comparer = null;
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsGreaterThanOrEqualTo(member + 1, comparer, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsGreaterThanOrEqualToByComparer_ReturnsExpectedErrors_IfMemberIsSmallerThanOtherValue_And_ComparerIsNull_And_NoErrorMessageIsSpecified()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsGreaterThanOrEqualTo(member + 1, null);

            validator.Validate(message).AssertOneError(string.Format("Member ({0}) must be greater than or equal to '{1}'.", member, member + 1));
        }

        [TestMethod]
        public void ValidateIsGreaterThanOrEqualToByComparer_ReturnsNoErrors_IfMemberIsEqualToOtherValue_And_ComparerIsNull()
        {
            IComparer<long> comparer = null;
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsGreaterThanOrEqualTo(member, comparer, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsGreaterThanOrEqualToByComparer_ReturnsNoErrors_IfMemberIsGreaterThanOtherValue_And_ComparerIsNull()
        {
            IComparer<long> comparer = null;
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsGreaterThanOrEqualTo(member - 1, comparer, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsGreaterThanOrEqualToByComparer_ReturnsNoErrors_IfComparerReturnsNegativeNumber()
        {
            // A negative result means that the specified value is smaller than the member.
            var comparer = new ComparerStub<object>(-1);
            var message = new ValidatedMessage<object>(new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsGreaterThanOrEqualTo(new object(), comparer, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsGreaterThanOrEqualToByComparer_ReturnsNoErrors_IfComparerReturnsZero()
        {
            // Zero means that the specified value is equal to the member.
            var comparer = new ComparerStub<object>(0);
            var message = new ValidatedMessage<object>(new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsGreaterThanOrEqualTo(new object(), comparer, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsGreaterThanOrEqualToByComparer_ReturnsExpectedError_IfComparerReturnsPositiveNumber()
        {
            // A positive result means that the specified value is greater than the member.
            var comparer = new ComparerStub<object>(1);
            var message = new ValidatedMessage<object>(new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsGreaterThanOrEqualTo(new object(), comparer, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsGreaterThanOrEqualTo_ReturnsExpectedError_IfMemberIsGreaterThanOtherValue()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsGreaterThanOrEqualTo(member + 1, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsGreaterThanOrEqualTo_ReturnsNoErrors_IfMemberIsEqualToOtherValue()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsGreaterThanOrEqualTo(member, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsGreaterThanOrEqualTo_ReturnsNoErrors_IfMemberIsGreaterThanOtherValue()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsGreaterThanOrEqualTo(member - 1, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        #endregion

        #region [====== IsGreaterThanOrEqualTo (Indirect) ======]

        [TestMethod]
        public void ValidateIsGreaterThanOrEqualToByComparer_ReturnsExpectedErrors_IfMemberIsSmallerThanOtherValue_And_ComparerIsNull_Indirect()
        {
            IComparer<long> comparer = null;
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<long>(member, member + 1);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsGreaterThanOrEqualTo(m => m.Other, comparer, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsGreaterThanOrEqualToByComparer_ReturnsExpectedErrors_IfMemberIsSmallerThanOtherValue_And_ComparerIsNull_And_NoErrorMessageIsSpecified_Indirect()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<long>(member, member + 1);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsGreaterThanOrEqualTo(m => m.Other, null);

            validator.Validate(message).AssertOneError(string.Format("Member ({0}) must be greater than or equal to 'Other ({1})'.", member, member + 1));
        }

        [TestMethod]
        public void ValidateIsGreaterThanOrEqualToByComparer_ReturnsNoErrors_IfMemberIsEqualToOtherValue_And_ComparerIsNull_Indirect()
        {
            IComparer<long> comparer = null;
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<long>(member, member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsGreaterThanOrEqualTo(m => m.Other, comparer, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsGreaterThanOrEqualToByComparer_ReturnsNoErrors_IfMemberIsGreaterThanOtherValue_And_ComparerIsNull_Indirect()
        {
            IComparer<long> comparer = null;
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<long>(member, member - 1);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsGreaterThanOrEqualTo(m => m.Other, comparer, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsGreaterThanOrEqualToByComparer_ReturnsNoErrors_IfComparerReturnsNegativeNumber_Indirect()
        {
            // A negative result means that the specified value is smaller than the member.
            var comparer = new ComparerStub<object>(-1);
            var message = new ValidatedMessage<object>(new object(), new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsGreaterThanOrEqualTo(m => m.Other, comparer, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsGreaterThanOrEqualToByComparer_ReturnsNoErrors_IfComparerReturnsZero_Indirect()
        {
            // Zero means that the specified value is equal to the member.
            var comparer = new ComparerStub<object>(0);
            var message = new ValidatedMessage<object>(new object(), new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsGreaterThanOrEqualTo(m => m.Other, comparer, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsGreaterThanOrEqualToByComparer_ReturnsExpectedError_IfComparerReturnsPositiveNumber_Indirect()
        {
            // A positive result means that the specified value is greater than the member.
            var comparer = new ComparerStub<object>(1);
            var message = new ValidatedMessage<object>(new object(), new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsGreaterThanOrEqualTo(m => m.Other, comparer, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsGreaterThanOrEqualTo_ReturnsExpectedError_IfMemberIsGreaterThanOtherValue_Indirect()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<long>(member, member + 1);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsGreaterThanOrEqualTo(m => m.Other, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsGreaterThanOrEqualTo_ReturnsNoErrors_IfMemberIsEqualToOtherValue_Indirect()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<long>(member, member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsGreaterThanOrEqualTo(m => m.Other, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsGreaterThanOrEqualTo_ReturnsNoErrors_IfMemberIsGreaterThanOtherValue_Indirect()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<long>(member, member - 1);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsGreaterThanOrEqualTo(m => m.Other, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        #endregion
    }
}
