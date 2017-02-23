using Kingo.Clocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging.Constraints
{
    public sealed partial class BasicConstraintsTest
    {
        #region [====== IsSmallerThan ======]

        [TestMethod]
        public void ValidateIsSmallerThanByComparer_ReturnsNoErrors_IfMemberIsSmallerThanOtherValue_And_ComparerIsNull()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsSmallerThan(member + 1, null, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsSmallerThanByComparer_ReturnsExpectedError_IfMemberIsEqualToOtherValue_And_ComparerIsNull()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsSmallerThan(member, null, RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsSmallerThanByComparer_ReturnsDefaultError_IfMemberIsEqualToOtherValue_And_ComparerIsNull_And_NoErrorMessageIsSpecified()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsSmallerThan(member, null);

            validator.Validate(message).AssertMemberError(string.Format("Member ({0}) must be smaller than '{0}'.", member));
        }

        [TestMethod]
        public void ValidateIsSmallerThanByComparer_ReturnsExpectedError_IfMemberIsGreaterThanOtherValue_And_ComparerIsNull()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsSmallerThan(member - 1, null, RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsSmallerThanByComparer_ReturnsExpectedError_IfComparerReturnsNegativeNumber()
        {
            // A negative result means that the specified value is smaller than the member.
            var comparer = new ComparerStub<object>(-1);
            var message = new ValidatedMessage<object>(new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsSmallerThan(new object(), comparer, RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsSmallerThanByComparer_ReturnsExpectedError_IfComparerReturnsZero()
        {
            // Zero means that the specified value is equal to the member.
            var comparer = new ComparerStub<object>(0);
            var message = new ValidatedMessage<object>(new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsSmallerThan(new object(), comparer, RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsSmallerThanByComparer_ReturnsNoErrors_IfComparerReturnsPositiveNumber()
        {
            // A positive result means that the specified value is greater than the member.
            var comparer = new ComparerStub<object>(1);
            var message = new ValidatedMessage<object>(new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsSmallerThan(new object(), comparer, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsSmallerThan_ReturnsNoErrors_IfMemberIsSmallerThanOtherValue()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsSmallerThan(member + 1, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsSmallerThan_ReturnsExpectedError_IfMemberIsEqualToOtherValue()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsSmallerThan(member, RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsSmallerThan_ReturnsExpectedError_IfMemberIsGreaterThanOtherValue()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsSmallerThan(member - 1, RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        #endregion

        #region [====== IsSmallerThan (Indirect) ======]

        [TestMethod]
        public void ValidateIsSmallerThanByComparer_ReturnsNoErrors_IfMemberIsSmallerThanOtherValue_And_ComparerIsNull_Indirect()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<long>(member, member + 1);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsSmallerThan(m => m.Other, null, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsSmallerThanByComparer_ReturnsExpectedError_IfMemberIsEqualToOtherValue_And_ComparerIsNull_Indirect()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<long>(member, member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsSmallerThan(m => m.Other, null, RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsSmallerThanByComparer_ReturnsDefaultError_IfMemberIsEqualToOtherValue_And_ComparerIsNull_And_NoErrorMessageIsSpecified_Indirect()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<long>(member, member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsSmallerThan(m => m.Other, null);

            validator.Validate(message).AssertMemberError(string.Format("Member ({0}) must be smaller than '{0}'.", member));
        }

        [TestMethod]
        public void ValidateIsSmallerThanByComparer_ReturnsExpectedError_IfMemberIsGreaterThanOtherValue_And_ComparerIsNull_Indirect()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<long>(member, member - 1);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsSmallerThan(m => m.Other, null, RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsSmallerThanByComparer_ReturnsExpectedError_IfComparerReturnsNegativeNumber_Indirect()
        {
            // A negative result means that the specified value is smaller than the member.
            var comparer = new ComparerStub<object>(-1);
            var message = new ValidatedMessage<object>(new object(), new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsSmallerThan(m => m.Other, comparer, RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsSmallerThanByComparer_ReturnsExpectedError_IfComparerReturnsZero_Indirect()
        {
            // Zero means that the specified value is equal to the member.
            var comparer = new ComparerStub<object>(0);
            var message = new ValidatedMessage<object>(new object(), new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsSmallerThan(m => m.Other, comparer, RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsSmallerThanByComparer_ReturnsNoErrors_IfComparerReturnsPositiveNumber_Indirect()
        {
            // A positive result means that the specified value is greater than the member.
            var comparer = new ComparerStub<object>(1);
            var message = new ValidatedMessage<object>(new object(), new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsSmallerThan(m => m.Other, comparer, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsSmallerThan_ReturnsNoErrors_IfMemberIsSmallerThanOtherValue_Indirect()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<long>(member, member + 1);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsSmallerThan(m => m.Other, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsSmallerThan_ReturnsExpectedError_IfMemberIsEqualToOtherValue_Indirect()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<long>(member, member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsSmallerThan(m => m.Other, RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsSmallerThan_ReturnsExpectedError_IfMemberIsGreaterThanOtherValue_Indirect()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<long>(member, member - 1);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsSmallerThan(m => m.Other, RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        #endregion

        #region [====== IsSmallerThanOrEqualTo ======]

        [TestMethod]
        public void ValidateIsSmallerThanOrEqualToByComparer_ReturnsNoErrors_IfMemberIsSmallerThanOtherValue_And_ComparerIsNull()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsSmallerThanOrEqualTo(member + 1, null, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsSmallerThanOrEqualToByComparer_ReturnsNoErrors_IfMemberIsEqualToOtherValue_And_ComparerIsNull()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsSmallerThanOrEqualTo(member, null, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsSmallerThanOrEqualToByComparer_ReturnsExpectedError_IfMemberIsGreaterThanOtherValue_And_ComparerIsNull()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsSmallerThanOrEqualTo(member - 1, null, RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsSmallerThanOrEqualToByComparer_ReturnsDefaultError_IfMemberIsGreaterThanOtherValue_And_ComparerIsNull_And_NoErrorMessageIsSpecified()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsSmallerThanOrEqualTo(member - 1, null);

            validator.Validate(message).AssertMemberError(string.Format("Member ({0}) must be smaller than or equal to '{1}'.", member, member - 1));
        }

        [TestMethod]
        public void ValidateIsSmallerThanOrEqualToByComparer_ReturnsExpectedError_IfComparerReturnsNegativeNumber()
        {
            // A negative result means that the specified value is smaller than the member.
            var comparer = new ComparerStub<object>(-1);
            var message = new ValidatedMessage<object>(new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsSmallerThanOrEqualTo(new object(), comparer, RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsSmallerThanOrEqualToByComparer_ReturnsNoErrors_IfComparerReturnsZero()
        {
            // Zero means that the specified value is equal to the member.
            var comparer = new ComparerStub<object>(0);
            var message = new ValidatedMessage<object>(new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsSmallerThanOrEqualTo(new object(), comparer, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsSmallerThanOrEqualToByComparer_ReturnsNoErrors_IfComparerReturnsPositiveNumber()
        {
            // A positive result means that the specified value is greater than the member.
            var comparer = new ComparerStub<object>(1);
            var message = new ValidatedMessage<object>(new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsSmallerThanOrEqualTo(new object(), comparer, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsSmallerThanOrEqualTo_ReturnsNoErrors_IfMemberIsSmallerThanOtherValue()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsSmallerThanOrEqualTo(member + 1, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsSmallerThanOrEqualTo_ReturnsNoErrors_IfMemberIsEqualToOtherValue()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsSmallerThanOrEqualTo(member, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsSmallerThanOrEqualTo_ReturnsExpectedError_IfMemberIsGreaterThanOtherValue()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsSmallerThanOrEqualTo(member - 1, RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        #endregion

        #region [====== IsSmallerThanOrEqualTo (Indirect) ======]

        [TestMethod]
        public void ValidateIsSmallerThanOrEqualToByComparer_ReturnsNoErrors_IfMemberIsSmallerThanOtherValue_And_ComparerIsNull_Indirect()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<long>(member, member + 1);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsSmallerThanOrEqualTo(m => m.Other, null, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsSmallerThanOrEqualToByComparer_ReturnsNoErrors_IfMemberIsEqualToOtherValue_And_ComparerIsNull_Indirect()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<long>(member, member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsSmallerThanOrEqualTo(m => m.Other, null, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsSmallerThanOrEqualToByComparer_ReturnsExpectedError_IfMemberIsGreaterThanOtherValue_And_ComparerIsNull_Indirect()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<long>(member, member - 1);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsSmallerThanOrEqualTo(m => m.Other, null, RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsSmallerThanOrEqualToByComparer_ReturnsDefaultError_IfMemberIsGreaterThanOtherValue_And_ComparerIsNull_And_NoErrorMessageIsSpecified_Indirect()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<long>(member, member - 1);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsSmallerThanOrEqualTo(m => m.Other, null);

            validator.Validate(message).AssertMemberError(string.Format("Member ({0}) must be smaller than or equal to '{1}'.", member, member - 1));
        }

        [TestMethod]
        public void ValidateIsSmallerThanOrEqualToByComparer_ReturnsExpectedError_IfComparerReturnsNegativeNumber_Indirect()
        {
            // A negative result means that the specified value is smaller than the member.
            var comparer = new ComparerStub<object>(-1);
            var message = new ValidatedMessage<object>(new object(), new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsSmallerThanOrEqualTo(m => m.Other, comparer, RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsSmallerThanOrEqualToByComparer_ReturnsNoErrors_IfComparerReturnsZero_Indirect()
        {
            // Zero means that the specified value is equal to the member.
            var comparer = new ComparerStub<object>(0);
            var message = new ValidatedMessage<object>(new object(), new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsSmallerThanOrEqualTo(m => m.Other, comparer, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsSmallerThanOrEqualToByComparer_ReturnsNoErrors_IfComparerReturnsPositiveNumber_Indirect()
        {
            // A positive result means that the specified value is greater than the member.
            var comparer = new ComparerStub<object>(1);
            var message = new ValidatedMessage<object>(new object(), new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsSmallerThanOrEqualTo(m => m.Other, comparer, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsSmallerThanOrEqualTo_ReturnsNoErrors_IfMemberIsSmallerThanOtherValue_Indirect()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<long>(member, member + 1);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsSmallerThanOrEqualTo(m => m.Other, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsSmallerThanOrEqualTo_ReturnsNoErrors_IfMemberIsEqualToOtherValue_Indirect()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<long>(member, member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsSmallerThanOrEqualTo(m => m.Other, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsSmallerThanOrEqualTo_ReturnsExpectedError_IfMemberIsGreaterThanOtherValue_Indirect()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<long>(member, member - 1);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsSmallerThanOrEqualTo(m => m.Other, RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        #endregion
    }
}
