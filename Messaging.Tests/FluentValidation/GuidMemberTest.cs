using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel.FluentValidation
{
    [TestClass]
    public sealed class GuidMemberTest
    {
        private string _errorMessage;

        [TestInitialize]
        public void Setup()
        {
            _errorMessage = Guid.NewGuid().ToString("N");
        }

        #region [====== IsNotEmpty ======]

        [TestMethod]
        public void ValidateIsNotEmpty_ReturnsExpectedError_IfGuidIsEmpty()
        {
            var message = new ValidatedMessage<Guid>(Guid.Empty);
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsNotEmpty(_errorMessage);

            validator.Validate().AssertOneError(_errorMessage);
        }

        [TestMethod]
        public void ValidateIsNotEmpty_ReturnsNoErrors_IfGuidIsNotEmpty()
        {
            var message = new ValidatedMessage<Guid>(Guid.NewGuid());
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsNotEmpty(_errorMessage);

            validator.Validate().AssertNoErrors();
        }

        #endregion

        #region [====== IsEmpty ======]

        [TestMethod]
        public void ValidateIsEmpty_ReturnsNoErrors_IfGuidIsEmpty()
        {
            var message = new ValidatedMessage<Guid>(Guid.Empty);
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsEmpty(_errorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsEmpty_ReturnsExpectedError_IfGuidIsNotEmpty()
        {
            var message = new ValidatedMessage<Guid>(Guid.NewGuid());
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsEmpty(_errorMessage);

            validator.Validate().AssertOneError(_errorMessage);
        }

        #endregion
    }
}
