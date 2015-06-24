using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel.FluentValidation
{
    [TestClass]
    public sealed class ListMemberTest
    {
        private string _errorMessage;

        [TestInitialize]
        public void Setup()
        {
            _errorMessage = Guid.NewGuid().ToString("N");
        }        

        #region [====== ElementAt ======]

        [TestMethod]
        public void ValidateElementAt_ReturnsExpectedError_IfCollectionIsNull()
        {
            var message = new ValidatedMessage<IList<object>>(null);
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).ElementAt(0, _errorMessage);

            validator.Validate().AssertOneError(_errorMessage);
        }

        [TestMethod]
        public void ValidateElementAt_ReturnsExpectedError_IfCollectionIsEmpty()
        {
            var message = new ValidatedMessage<IList<object>>(new object[0]);
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).ElementAt(0, _errorMessage);

            validator.Validate().AssertOneError(_errorMessage);
        }

        [TestMethod]
        public void ValidateElementAt_ReturnsNoErrors_IfCollectionContainsSpecifiedElement()
        {
            var value = new object();
            var message = new ValidatedMessage<IList<object>>(new[] { value });
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member)
                .ElementAt(0, _errorMessage)
                .IsSameInstanceAs(value, _errorMessage);

            validator.Validate().AssertNoErrors();
        }

        #endregion
    }
}
