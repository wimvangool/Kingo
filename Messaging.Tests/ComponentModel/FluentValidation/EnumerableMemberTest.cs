using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Syztem.ComponentModel.FluentValidation
{
    [TestClass]
    public sealed class EnumerableMemberTest
    {
        private string _errorMessage;

        [TestInitialize]
        public void Setup()
        {
            _errorMessage = Guid.NewGuid().ToString("N");
        }

        #region [====== IsNotNullOrEmpty ======]

        [TestMethod]
        public void ValidateIsNotNullOrEmpty_ReturnsExpectedError_IfCollectionIsNull()
        {
            var message = new ValidatedMessage<IEnumerable<object>>(null);
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsNotNullOrEmpty(_errorMessage);

            validator.Validate().AssertOneError(_errorMessage);
        }

        [TestMethod]
        public void ValidateIsNotNullOrEmpty_ReturnsExpectedError_IfCollectionIsEmpty()
        {
            var message = new ValidatedMessage<IEnumerable<object>>(Enumerable.Empty<object>());
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsNotNullOrEmpty(_errorMessage);

            validator.Validate().AssertOneError(_errorMessage);
        }

        [TestMethod]
        public void ValidateIsNotNullOrEmpty_ReturnsNoErrors_IfCollectionHasOneElement()
        {
            var message = new ValidatedMessage<IEnumerable<object>>(new [] { new object() });
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsNotNullOrEmpty(_errorMessage);

            validator.Validate().AssertNoErrors();
        }

        #endregion

        #region [====== ElementAt ======]

        [TestMethod]
        public void ValidateElementAt_ReturnsExpectedError_IfCollectionIsNull()
        {
            var message = new ValidatedMessage<IEnumerable<object>>(null);
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).ElementAt(0, _errorMessage);

            validator.Validate().AssertOneError(_errorMessage);
        }

        [TestMethod]
        public void ValidateElementAt_ReturnsExpectedError_IfCollectionIsEmpty()
        {
            var message = new ValidatedMessage<IEnumerable<object>>(Enumerable.Empty<object>());
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).ElementAt(0, _errorMessage);

            validator.Validate().AssertOneError(_errorMessage);
        }

        [TestMethod]
        public void ValidateElementAt_ReturnsNoErrors_IfCollectionContainsSpecifiedElement()
        {
            var value = new object();
            var message = new ValidatedMessage<IEnumerable<object>>(new [] { value });
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member)
                .ElementAt(0, _errorMessage)
                .IsSameInstanceAs(value, _errorMessage);

            validator.Validate().AssertNoErrors();
        }

        #endregion
    }
}
