using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ServiceComponents.ComponentModel.Constraints
{
    [TestClass]
    public sealed class BooleanConstraintTest : ConstraintTest
    {
        #region [====== IsTrue ======]

        [TestMethod]
        public void ValidateIsTrue_ReturnsExpectedError_IfMemberIsFalse()
        {
            var message = new ValidatedMessage<bool>(false);
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).IsTrue(RandomErrorMessage);

            validator.Validate().AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsTrue_ReturnsDefaultError_IfMemberIsFalse__And_NoErrorMessageIsSpecified()
        {
            var message = new ValidatedMessage<bool>(false);
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).IsTrue();

            validator.Validate().AssertOneError("Member (false) must be true.");
        }

        [TestMethod]
        public void ValidateIsTrue_ReturnsNoErrors_IfMemberIsTrue()
        {
            var message = new ValidatedMessage<bool>(true);
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).IsTrue(RandomErrorMessage);

            validator.Validate().AssertNoErrors();
        }

        #endregion

        #region [====== IsFalse ======]

        [TestMethod]
        public void ValidateIsFalse_ReturnsExpectedError_IfMemberIsTrue()
        {
            var message = new ValidatedMessage<bool>(true);
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).IsFalse(RandomErrorMessage);

            validator.Validate().AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsFalse_ReturnsDefaultError_IfMemberIsTrue__And_NoErrorMessageIsSpecified()
        {
            var message = new ValidatedMessage<bool>(true);
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).IsFalse();

            validator.Validate().AssertOneError("Member (true) must be false.");
        }

        [TestMethod]
        public void ValidateIsFalse_ReturnsNoErrors_IfMemberIsFalse()
        {
            var message = new ValidatedMessage<bool>(false);
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).IsFalse(RandomErrorMessage);

            validator.Validate().AssertNoErrors();
        }

        #endregion
    }
}
