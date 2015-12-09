using System;
using Kingo.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Constraints
{    
    public sealed partial class BasicConstraintsTest
    {
        #region [====== And ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void And_Throws_IfArgumentIsNull()
        {
            var message = new ValidatedMessage<object>(new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotNull().And(null);
        }

        [TestMethod]
        public void And_ReturnsNoErrors_IfChildValidationSucceeds()
        {
            var message = new ValidatedMessage<object>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotNull().IsInstanceOf<string>().And(v =>
            {
                v.VerifyThat(value => value.Length).IsEqualTo(10);
            });

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void And_ReturnsExpectedError_IfChildValidationFails()
        {
            var message = new ValidatedMessage<object>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotNull().IsInstanceOf<string>().And(v =>
            {
                v.VerifyThat(value => value.Length).IsNotEqualTo(10, RandomErrorMessage);
            });

            validator.Validate(message).AssertMemberError(RandomErrorMessage, "Member", "Member.Length");
        }

        [TestMethod]
        public void And_ReturnsExpectedError_IfChildOfChildValidationFails()
        {
            var message = new ValidatedMessage<ValidatedMessage<object>>(new ValidatedMessage<object>("Some value"));
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotNull().And(v1 =>
            {
                v1.VerifyThat(childMessage => childMessage.Member).IsInstanceOf<string>().And(v2 =>
                {
                    v2.VerifyThat(value => value.Length).IsNotEqualTo(10, RandomErrorMessage);
                });
            });

            validator.Validate(message).AssertMemberError(RandomErrorMessage, "Member", "Member.Member", "Member.Member.Length");
        }

        [TestMethod]
        public void And_ReturnsExpectedError_IfBothParentAndChildValidationFails()
        {
            var message = new ValidatedMessage<string>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNull();
            validator.VerifyThat(m => m.Member).IsNotNull().And(v =>
            {
                v.VerifyThat(value => value.Length).IsEqualTo(6);
            });

            validator.Validate(message)
                .AssertMemberError("Member (Some value) must be null.", "Member")
                .AssertMemberError("Member.Length (10) must be equal to '6'.", "Member.Length");
        }

        [TestMethod]
        public void And_ReturnsExpectedError_IfParentNameIsTransformedName()
        {
            var message = new ValidatedMessage<object[]>(new object[]{ "", "Some value", null });
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member[1]).IsInstanceOf<string>().And(v =>
            {
                v.VerifyThat(value => value.Length).IsEqualTo(3);
            });

            validator.Validate(message).AssertMemberError("Member[1].Length (10) must be equal to '3'.", "Member[1].Length", "Member[1]", "Member");
        }

        [TestMethod]
        public void And_ReturnsExpectedError_IfParentAndChildNameAreTransformedNames()
        {
            var message = new ValidatedMessage<object[][]>(new [] { new object[] { null, "Some value" } });
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member[0]).IsNotNull().And(v =>
            {
                v.VerifyThat(array => array.Length).IsEqualTo(2);
                v.VerifyThat(array => array[1]).IsInstanceOf<string>().And(v2 =>
                {
                    v2.VerifyThat(value => value.Length).IsEqualTo(3);
                });
            });

            validator.Validate(message).AssertMemberError("Member[0][1].Length (10) must be equal to '3'.", "Member[0][1].Length", "Member[0][1]", "Member[0]", "Member");
        }

        #endregion
    }
}
