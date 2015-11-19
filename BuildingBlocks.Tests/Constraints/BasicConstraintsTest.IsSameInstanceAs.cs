using System;
using Kingo.BuildingBlocks.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.BuildingBlocks.Constraints
{
    public sealed partial class BasicConstraintsTest
    {
        #region [====== IsNotSameInstanceAs ======]

        [TestMethod]
        public void ValidateIsNotSameInstanceAs_ReturnsExpectedError_IfBothObjectsAreNull()
        {
            var message = new ValidatedMessage<object>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotSameInstanceAs(null as object, RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsNotSameInstanceAs_ReturnsDefaultError_IfBothObjectsAreNull_And_NoErrorMessageIsSpecified()
        {
            var message = new ValidatedMessage<object>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotSameInstanceAs(null as object);

            validator.Validate(message).AssertMemberError("Member (<null>) must not refer to the same instance as '<null>'.");
        }

        [TestMethod]
        public void ValidateIsNotSameInstanceAs_ReturnsExpectedError_IfObjectsAreSameInstance()
        {
            var member = new object();
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotSameInstanceAs(member, RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsNotSameInstanceAs_ReturnsDefaultError_IfObjectsAreSameInstance_And_NoErrorMessageIsSpecified()
        {
            var member = new object();
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotSameInstanceAs(member);

            validator.Validate(message).AssertMemberError("Member (System.Object) must not refer to the same instance as 'System.Object'.");
        }

        [TestMethod]
        public void ValidateIsNotSameInstanceAs_ReturnsNoErrors_IfObjectsAreNotSameInstance()
        {
            var member = new object();
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotSameInstanceAs(new object(), RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        #endregion

        #region [====== IsNotSameInstanceAs (Indirect) ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ValidateIsNotSameInstanceAs_Throws_IfOtherFactoryIsNull()
        {
            var message = new ValidatedMessage<object>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotSameInstanceAs(null);
        }

        [TestMethod]
        public void ValidateIsNotSameInstanceAs_ReturnsExpectedError_IfBothObjectsAreNull_Indirect()
        {
            var message = new ValidatedMessage<object>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotSameInstanceAs(m => null, RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsNotSameInstanceAs_ReturnsDefaultError_IfBothObjectsAreNull_And_NoErrorMessageIsSpecified_Indirect()
        {
            var message = new ValidatedMessage<object>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotSameInstanceAs(m => null);

            validator.Validate(message).AssertMemberError("Member (<null>) must not refer to the same instance as '<null>'.");
        }

        [TestMethod]
        public void ValidateIsNotSameInstanceAs_ReturnsExpectedError_IfObjectsAreSameInstance_Indirect()
        {
            var member = new object();
            var message = new ValidatedMessage<object>(member, member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotSameInstanceAs(m => m.Other, RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsNotSameInstanceAs_ReturnsDefaultError_IfObjectsAreSameInstance_And_NoErrorMessageIsSpecified_Indirect()
        {
            var member = new object();
            var message = new ValidatedMessage<object>(member, member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotSameInstanceAs(m => m.Other);

            validator.Validate(message).AssertMemberError("Member (System.Object) must not refer to the same instance as 'System.Object'.");
        }

        [TestMethod]
        public void ValidateIsNotSameInstanceAs_ReturnsNoErrors_IfObjectsAreNotSameInstance_Indirect()
        {
            var member = new object();
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotSameInstanceAs(m => new object(), RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        #endregion

        #region [====== IsSameInstanceAs ======]

        [TestMethod]
        public void ValidateIsSameInstanceAs_ReturnsNoErrors_IfBothObjectsAreNull()
        {
            var message = new ValidatedMessage<object>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsSameInstanceAs(null as object, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsSameInstanceAs_ReturnsNoErrors_IfObjectsAreSameInstance()
        {
            var member = new object();
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsSameInstanceAs(member, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsSameInstanceAs_ReturnsExpectedError_IfObjectsAreNotSameInstance()
        {
            var member = new object();
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsSameInstanceAs(new object(), RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsSameInstanceAs_ReturnsDefaultError_IfObjectsAreNotSameInstance_And_NoErrorMessageIsSpecified()
        {
            var member = new object();
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsSameInstanceAs(new object());

            validator.Validate(message).AssertMemberError("Member (System.Object) must refer to the same instance as 'System.Object'.");
        }

        #endregion

        #region [====== IsSameInstanceAs (Indirect) ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ValidateIsSameInstanceAs_Throws_IfOtherFactoryIsNull()
        {
            var message = new ValidatedMessage<object>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsSameInstanceAs(null);
        }

        [TestMethod]
        public void ValidateIsSameInstanceAs_ReturnsNoErrors_IfBothObjectsAreNull_Indirect()
        {
            var message = new ValidatedMessage<object>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsSameInstanceAs(m => null, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsSameInstanceAs_ReturnsNoErrors_IfObjectsAreSameInstance_Indirect()
        {
            var member = new object();
            var message = new ValidatedMessage<object>(member, member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsSameInstanceAs(m => m.Other, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsSameInstanceAs_ReturnsExpectedError_IfObjectsAreNotSameInstance_Indirect()
        {
            var member = new object();
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsSameInstanceAs(m => new object(), RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsSameInstanceAs_ReturnsDefaultError_IfObjectsAreNotSameInstance_And_NoErrorMessageIsSpecified_Indirect()
        {
            var member = new object();
            var message = new ValidatedMessage<object>(member, new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsSameInstanceAs(m => m.Other);

            validator.Validate(message).AssertMemberError("Member (System.Object) must refer to the same instance as 'System.Object'.");
        }

        #endregion
    }
}
