using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.BuildingBlocks.Constraints
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

            validator.Validate(message).AssertOneError(RandomErrorMessage, "Member.Length");
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

            validator.Validate(message).AssertOneError(RandomErrorMessage, "Member.Member.Length");
        }

        #endregion
    }
}
