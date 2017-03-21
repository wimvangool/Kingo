using System;
using System.Collections.Generic;
using Kingo.Clocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging.Validation.Constraints
{
    public sealed partial class BasicConstraintsTest
    {
        #region [====== IsNotEqualTo ======]

        [TestMethod]
        public void ValidateIsNotEqualTo_ReturnsExpectedError_IfObjectsAreBothNull()
        {
            var message = new ValidatedMessage<object>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotEqualTo(null as object, RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsNotEqualTo_ReturnsDefaultError_IfObjectsAreBothNull_And_NoErrorMessageIsSpecified()
        {
            var message = new ValidatedMessage<object>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotEqualTo(null as object);

            validator.Validate(message).AssertMemberError("Member (<null>) must not be equal to '<null>'.");
        }

        [TestMethod]
        public void ValidateIsNotEqualTo_ReturnsExpectedError_IfObjectsAreSameInstance()
        {
            var member = new object();
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotEqualTo(member, RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsNotEqualTo_ReturnsDefaultError_IfObjectsAreSameInstance_And_NoErrorMessageIsSpecified()
        {
            var member = new object();
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotEqualTo(member);

            validator.Validate(message).AssertMemberError("Member (System.Object) must not be equal to 'System.Object'.");
        }

        [TestMethod]
        public void ValidateIsNotEqualTo_ReturnsExpectedError_IfObjectsAreEqual()
        {
            var memberValue = Guid.NewGuid();
            var message = new ValidatedMessage<object>(memberValue.ToString());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotEqualTo(memberValue.ToString(), RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsNotEqualTo_ReturnsDefaultError_IfObjectsAreEqual_And_NoErrorMessageIsSpecified()
        {
            var memberValue = Guid.NewGuid();
            var message = new ValidatedMessage<object>(memberValue.ToString());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotEqualTo(memberValue.ToString());

            validator.Validate(message).AssertMemberError(string.Format("Member ({0}) must not be equal to '{0}'.", memberValue));
        }

        [TestMethod]
        public void ValidateIsNotEqualTo_ReturnsNoErrors_IfMemberIsNullAndTheOtherIsNot()
        {
            var message = new ValidatedMessage<object>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotEqualTo(new object(), RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsNotEqualTo_ReturnsNoErrors_IfOtherIsNullAndTheMemberIsNot()
        {
            var message = new ValidatedMessage<object>(new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotEqualTo(null as object, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsNotEqualTo_ReturnsNoErrors_IfObjectsAreNotNullAndNotEqual()
        {
            var message = new ValidatedMessage<object>(new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotEqualTo(new object(), RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsNotEqualTo_ReturnsNoErrors_IfStringsAreNotEqual()
        {
            var message = new ValidatedMessage<object>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotEqualTo("Some other value", RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsNotEqualToByComparer_ReturnsExpectedError_IfObjectsAreBothNull_And_ComparerIsNull()
        {
            var message = new ValidatedMessage<object>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotEqualTo(null as object, null, RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsNotEqualToByComparer_ReturnsExpectedError_IfObjectsAreSameInstance_And_ComparerIsNull()
        {
            IEqualityComparer<object> comparer = null;
            var member = new object();
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotEqualTo(member, comparer, RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsNotEqualToByComparer_ReturnsExpectedError_IfObjectsAreEqual_And_ComparerIsNull()
        {
            IEqualityComparer<object> comparer = null;
            var memberValue = Guid.NewGuid();
            var message = new ValidatedMessage<object>(memberValue.ToString());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotEqualTo(memberValue.ToString(), comparer, RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsNotEqualToByComparer_ReturnsNoErrors_IfMemberIsNullAndTheOtherIsNot_And_ComparerIsNull()
        {
            IEqualityComparer<object> comparer = null;
            var message = new ValidatedMessage<object>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotEqualTo(new object(), comparer, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsNotEqualToByComparer_ReturnsNoErrors_IfOtherIsNullAndTheMemberIsNot_And_ComparerIsNull()
        {
            IEqualityComparer<object> comparer = null;
            var message = new ValidatedMessage<object>(new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotEqualTo(null as object, comparer, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsNotEqualToByComparer_ReturnsNoErrors_IfObjectsAreNotNullAndNotEqual_And_ComparerIsNull()
        {
            IEqualityComparer<object> comparer = null;
            var message = new ValidatedMessage<object>(new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotEqualTo(new object(), comparer, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsNotEqualToByComparer_ReturnsNoErrors_IfStringsAreNotEqual_And_ComparerIsNull()
        {
            IEqualityComparer<object> comparer = null;
            var message = new ValidatedMessage<object>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotEqualTo("Some other value", comparer, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsNotEqualToByComparer_ReturnsExpectedError_IfComparerReturnsTrue()
        {
            var comparer = new EqualityComparerStub<object>(true);
            var message = new ValidatedMessage<object>(Guid.NewGuid());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotEqualTo(Guid.NewGuid(), comparer, RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsNotEqualToByComparer_ReturnsNoErrors_IfComparerReturnsFalse()
        {
            var comparer = new EqualityComparerStub<object>(false);
            var message = new ValidatedMessage<object>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotEqualTo(null as object, comparer, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsNotEqualToOther_ReturnsExpectedError_IfValuesAreEqual()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsNotEqualTo(member, RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsNotEqualToOther_ReturnsNoErrors_IfValuesAreNotEqual()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsNotEqualTo(member + 1, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        #endregion

        #region [====== IsNotEqualTo (Indirect) ======]

        [TestMethod]
        public void ValidateIsNotEqualTo_ReturnsExpectedError_IfObjectsAreBothNull_Indirect()
        {
            var message = new ValidatedMessage<object>(null, null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotEqualTo(m => m.Other, RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsNotEqualTo_ReturnsDefaultError_IfObjectsAreBothNull_And_NoErrorMessageIsSpecified_Indirect()
        {
            var message = new ValidatedMessage<object>(null, null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotEqualTo(m => m.Other);

            validator.Validate(message).AssertMemberError("Member (<null>) must not be equal to '<null>'.");
        }

        [TestMethod]
        public void ValidateIsNotEqualTo_ReturnsExpectedError_IfObjectsAreSameInstance_Indirect()
        {
            var member = new object();
            var message = new ValidatedMessage<object>(member, member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotEqualTo(m => m.Other, RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsNotEqualTo_ReturnsDefaultError_IfObjectsAreSameInstance_And_NoErrorMessageIsSpecified_Indirect()
        {
            var member = new object();
            var message = new ValidatedMessage<object>(member, member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotEqualTo(m => m.Other);

            validator.Validate(message).AssertMemberError("Member (System.Object) must not be equal to 'System.Object'.");
        }

        [TestMethod]
        public void ValidateIsNotEqualTo_ReturnsExpectedError_IfObjectsAreEqual_Indirect()
        {
            var memberValue = Guid.NewGuid();
            var message = new ValidatedMessage<object>(memberValue.ToString());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotEqualTo(memberValue.ToString(), RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsNotEqualTo_ReturnsDefaultError_IfObjectsAreEqual_And_NoErrorMessageIsSpecified_Indirect()
        {
            var memberValue = Guid.NewGuid();
            var message = new ValidatedMessage<object>(memberValue.ToString(), memberValue.ToString());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotEqualTo(m => m.Other);

            validator.Validate(message).AssertMemberError(string.Format("Member ({0}) must not be equal to '{0}'.", memberValue));
        }

        [TestMethod]
        public void ValidateIsNotEqualTo_ReturnsNoErrors_IfMemberIsNullAndTheOtherIsNot_Indirect()
        {
            var message = new ValidatedMessage<object>(null, new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotEqualTo(m => m.Other, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsNotEqualTo_ReturnsNoErrors_IfOtherIsNullAndTheMemberIsNot_Indirect()
        {
            var message = new ValidatedMessage<object>(new object(), null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotEqualTo(m => m.Other, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsNotEqualTo_ReturnsNoErrors_IfObjectsAreNotNullAndNotEqual_Indirect()
        {
            var message = new ValidatedMessage<object>(new object(), new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotEqualTo(m => m.Other, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsNotEqualTo_ReturnsNoErrors_IfStringsAreNotEqual_Indirect()
        {
            var message = new ValidatedMessage<object>("Some value", "Some other value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotEqualTo(m => m.Other, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsNotEqualToByComparer_ReturnsExpectedError_IfObjectsAreBothNull_And_ComparerIsNull_Indirect()
        {
            var message = new ValidatedMessage<object>(null, null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotEqualTo(m => m.Other, null, RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsNotEqualToByComparer_ReturnsExpectedError_IfObjectsAreSameInstance_And_ComparerIsNull_Indirect()
        {
            IEqualityComparer<object> comparer = null;
            var member = new object();
            var message = new ValidatedMessage<object>(member, member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotEqualTo(m => m.Other, comparer, RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsNotEqualToByComparer_ReturnsExpectedError_IfObjectsAreEqual_And_ComparerIsNull_Indirect()
        {
            IEqualityComparer<object> comparer = null;
            var memberValue = Guid.NewGuid();
            var message = new ValidatedMessage<object>(memberValue.ToString(), memberValue.ToString());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotEqualTo(m => m.Other, comparer, RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsNotEqualToByComparer_ReturnsNoErrors_IfMemberIsNullAndTheOtherIsNot_And_ComparerIsNull_Indirect()
        {
            IEqualityComparer<object> comparer = null;
            var message = new ValidatedMessage<object>(null, new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotEqualTo(m => m.Other, comparer, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsNotEqualToByComparer_ReturnsNoErrors_IfOtherIsNullAndTheMemberIsNot_And_ComparerIsNull_Indirect()
        {
            IEqualityComparer<object> comparer = null;
            var message = new ValidatedMessage<object>(new object(), null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotEqualTo(m => m.Other, comparer, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsNotEqualToByComparer_ReturnsNoErrors_IfObjectsAreNotNullAndNotEqual_And_ComparerIsNull_Indirect()
        {
            IEqualityComparer<object> comparer = null;
            var message = new ValidatedMessage<object>(new object(), new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotEqualTo(m => m.Other, comparer, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsNotEqualToByComparer_ReturnsNoErrors_IfStringsAreNotEqual_And_ComparerIsNull_Indirect()
        {
            IEqualityComparer<object> comparer = null;
            var message = new ValidatedMessage<object>("Some value", "Some other value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotEqualTo(m => m.Other, comparer, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsNotEqualToByComparer_ReturnsExpectedError_IfComparerReturnsTrue_Indirect()
        {
            var comparer = new EqualityComparerStub<object>(true);
            var message = new ValidatedMessage<object>(Guid.NewGuid(), Guid.NewGuid());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotEqualTo(m => m.Other, comparer, RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsNotEqualToByComparer_ReturnsNoErrors_IfComparerReturnsFalse_Indirect()
        {
            var comparer = new EqualityComparerStub<object>(false);
            var message = new ValidatedMessage<object>(null, null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotEqualTo(m => m.Other, comparer, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsNotEqualToOther_ReturnsExpectedError_IfValuesAreEqual_Indirect()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member, member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsNotEqualTo(m => m.Other, RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsNotEqualToOther_ReturnsNoErrors_IfValuesAreNotEqual_Indirect()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member, member + 1);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsNotEqualTo(m => m.Other, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        #endregion

        #region [====== IsEqualTo ======]

        [TestMethod]
        public void ValidateIsEqualTo_ReturnsNoErrors_IfObjectsAreBothNull()
        {
            var message = new ValidatedMessage<object>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsEqualTo(null as object, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsEqualTo_ReturnsNoErrors_IfObjectsAreSameInstance()
        {
            var member = new object();
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsEqualTo(member, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsEqualTo_ReturnsNoErrors_IfObjectsAreEqual()
        {
            var memberValue = Guid.NewGuid();
            var message = new ValidatedMessage<object>(memberValue.ToString());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsEqualTo(memberValue.ToString(), RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsEqualTo_ReturnsExpectedError_IfMemberIsNullAndTheOtherIsNot()
        {
            var message = new ValidatedMessage<object>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsEqualTo(new object(), RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsEqualTo_ReturnsDefaultError_IfMemberIsNullAndTheOtherIsNot_And_NoErrorMessageIsSpecified()
        {
            var message = new ValidatedMessage<object>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsEqualTo(new object());

            validator.Validate(message).AssertMemberError("Member (<null>) must be equal to 'System.Object'.");
        }

        [TestMethod]
        public void ValidateIsEqualTo_ReturnsExpectedError_IfOtherIsNullAndTheMemberIsNot()
        {
            var message = new ValidatedMessage<object>(new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsEqualTo(null as object, RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsEqualTo_ReturnsDefaultError_IfOtherIsNullAndTheMemberIsNot_And_NoErrorMessageIsSpecified()
        {
            var message = new ValidatedMessage<object>(new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsEqualTo(null as object);

            validator.Validate(message).AssertMemberError("Member (System.Object) must be equal to '<null>'.");
        }

        [TestMethod]
        public void ValidateIsEqualTo_ReturnsExpectedError_IfObjectsAreNotNullAndNotEqual()
        {
            var message = new ValidatedMessage<object>(new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsEqualTo(new object(), RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsEqualTo_ReturnsDefaultError_IfObjectsAreNotNullAndNotEqual_And_NoErrorMessageIsSpecified()
        {
            var message = new ValidatedMessage<object>(new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsEqualTo(new object());

            validator.Validate(message).AssertMemberError("Member (System.Object) must be equal to 'System.Object'.");
        }

        [TestMethod]
        public void ValidateIsEqualTo_ReturnsExpectedError_IfStringsAreNotEqual()
        {
            var message = new ValidatedMessage<object>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsEqualTo("Some other value", RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsEqualToByComparer_ReturnsNoErrors_IfObjectsAreBothNull_And_ComparerIsNull()
        {
            var message = new ValidatedMessage<object>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsEqualTo(null as object, null, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsEqualToByComparer_ReturnsNoErrors_IfObjectsAreSameInstance_And_ComparerIsNull()
        {
            var member = new object();
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsEqualTo(member, null, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsEqualToByComparer_ReturnsNoErrors_IfObjectsAreEqual_And_ComparerIsNull()
        {
            var memberValue = Guid.NewGuid();
            var message = new ValidatedMessage<object>(memberValue.ToString());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsEqualTo(memberValue.ToString(), null, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsEqualToByComparer_ReturnsExpectedError_IfMemberIsNullAndTheOtherIsNot_And_ComparerIsNull()
        {
            var message = new ValidatedMessage<object>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsEqualTo(new object(), null, RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsEqualToByComparer_ReturnsExpectedError_IfOtherIsNullAndTheMemberIsNot_And_ComparerIsNull()
        {
            var message = new ValidatedMessage<object>(new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsEqualTo(null as object, null, RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsEqualToByComparer_ReturnsExpectedError_IfObjectsAreNotNullAndNotEqual_And_ComparerIsNull()
        {
            var message = new ValidatedMessage<object>(new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsEqualTo(new object(), null, RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsEqualToByComparer_ReturnsExpectedError_IfStringsAreNotEqual_And_ComparerIsNull()
        {
            var message = new ValidatedMessage<object>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsEqualTo("Some other value", null, RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsEqualToByComparer_ReturnsNoErrors_IfComparerReturnsTrue()
        {
            var comparer = new EqualityComparerStub<object>(true);
            var message = new ValidatedMessage<object>(Guid.NewGuid());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsEqualTo(Guid.NewGuid(), comparer, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsEqualToByComparer_ReturnsExpectedError_IfComparerReturnsFalse()
        {
            var comparer = new EqualityComparerStub<object>(false);
            var message = new ValidatedMessage<object>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsEqualTo(null as object, comparer, RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsEqualToOther_ReturnsNoErrors_IfValuesAreEqual()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsEqualTo(member, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsEqualToOther_ReturnsExpectedError_IfValuesAreNotEqual()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsEqualTo(member + 1, RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        #endregion

        #region [====== IsEqualTo (Indirect) ======]

        [TestMethod]
        public void ValidateIsEqualTo_ReturnsNoErrors_IfObjectsAreBothNull_Indirect()
        {
            var message = new ValidatedMessage<object>(null, null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsEqualTo(m => m.Other, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsEqualTo_ReturnsNoErrors_IfObjectsAreSameInstance_Indirect()
        {
            var member = new object();
            var message = new ValidatedMessage<object>(member, member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsEqualTo(m => m.Other, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsEqualTo_ReturnsNoErrors_IfObjectsAreEqual_Indirect()
        {
            var member = Guid.NewGuid();
            var message = new ValidatedMessage<object>(member, member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsEqualTo(m => m.Other, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsEqualTo_ReturnsExpectedError_IfMemberIsNullAndTheOtherIsNot_Indirect()
        {
            var message = new ValidatedMessage<object>(null, new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsEqualTo(m => m.Other, RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsEqualTo_ReturnsDefaultError_IfMemberIsNullAndTheOtherIsNot_And_NoErrorMessageIsSpecified_Indirect()
        {
            var message = new ValidatedMessage<object>(null, new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsEqualTo(m => m.Other);

            validator.Validate(message).AssertMemberError("Member (<null>) must be equal to 'System.Object'.");
        }

        [TestMethod]
        public void ValidateIsEqualTo_ReturnsExpectedError_IfOtherIsNullAndTheMemberIsNot_Indirect()
        {
            var message = new ValidatedMessage<object>(new object(), null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsEqualTo(m => m.Other, RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsEqualTo_ReturnsDefaultError_IfOtherIsNullAndTheMemberIsNot_And_NoErrorMessageIsSpecified_Indirect()
        {
            var message = new ValidatedMessage<object>(new object(), null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsEqualTo(m => m.Other);

            validator.Validate(message).AssertMemberError("Member (System.Object) must be equal to '<null>'.");
        }

        [TestMethod]
        public void ValidateIsEqualTo_ReturnsExpectedError_IfObjectsAreNotNullAndNotEqual_Indirect()
        {
            var message = new ValidatedMessage<object>(new object(), new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsEqualTo(m => m.Other, RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsEqualTo_ReturnsDefaultError_IfObjectsAreNotNullAndNotEqual_And_NoErrorMessageIsSpecified_Indirect()
        {
            var message = new ValidatedMessage<object>(new object(), new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsEqualTo(m => m.Other);

            validator.Validate(message).AssertMemberError("Member (System.Object) must be equal to 'System.Object'.");
        }

        [TestMethod]
        public void ValidateIsEqualTo_ReturnsExpectedError_IfStringsAreNotEqual_Indirect()
        {
            var message = new ValidatedMessage<object>("Some value", "Some other value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsEqualTo(m => m.Other, RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsEqualToByComparer_ReturnsNoErrors_IfObjectsAreBothNull_And_ComparerIsNull_Indirect()
        {
            var message = new ValidatedMessage<object>(null, null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsEqualTo(m => m.Other, null, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsEqualToByComparer_ReturnsNoErrors_IfObjectsAreSameInstance_And_ComparerIsNull_Indirect()
        {
            var member = new object();
            var message = new ValidatedMessage<object>(member, member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsEqualTo(m => m.Other, null, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsEqualToByComparer_ReturnsNoErrors_IfObjectsAreEqual_And_ComparerIsNull_Indirect()
        {
            var member = Guid.NewGuid();
            var message = new ValidatedMessage<object>(member, member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsEqualTo(m => m.Other, null, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsEqualToByComparer_ReturnsExpectedError_IfMemberIsNullAndTheOtherIsNot_And_ComparerIsNull_Indirect()
        {
            var message = new ValidatedMessage<object>(null, new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsEqualTo(m => m.Other, null, RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsEqualToByComparer_ReturnsExpectedError_IfOtherIsNullAndTheMemberIsNot_And_ComparerIsNull_Indirect()
        {
            var message = new ValidatedMessage<object>(new object(), null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsEqualTo(m => m.Other, null, RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsEqualToByComparer_ReturnsExpectedError_IfObjectsAreNotNullAndNotEqual_And_ComparerIsNull_Indirect()
        {
            var message = new ValidatedMessage<object>(new object(), new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsEqualTo(m => m.Other, null, RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsEqualToByComparer_ReturnsExpectedError_IfStringsAreNotEqual_And_ComparerIsNull_Indirect()
        {
            var message = new ValidatedMessage<object>("Some value", "Some other value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsEqualTo(m => m.Other, null, RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsEqualToByComparer_ReturnsNoErrors_IfComparerReturnsTrue_Indirect()
        {
            var comparer = new EqualityComparerStub<object>(true);
            var message = new ValidatedMessage<object>(Guid.NewGuid(), Guid.NewGuid());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsEqualTo(m => m.Other, comparer, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsEqualToByComparer_ReturnsExpectedError_IfComparerReturnsFalse_Indirect()
        {
            var comparer = new EqualityComparerStub<object>(false);
            var message = new ValidatedMessage<object>(null, null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsEqualTo(m => m.Other, comparer, RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsEqualToOther_ReturnsNoErrors_IfValuesAreEqual_Indirect()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member, member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsEqualTo(m => m.Other, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsEqualToOther_ReturnsExpectedError_IfValuesAreNotEqual_Indirect()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member, member + 1);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsEqualTo(m => m.Other, RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        #endregion
    }
}
