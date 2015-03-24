using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel.FluentValidation
{
    [TestClass]
    public sealed class StringMemberTest
    {        
        private string _errorMessage;

        [TestInitialize]
        public void Setup()
        {
            _errorMessage = Guid.NewGuid().ToString("N");
        }

        #region [====== IsNotNullOrEmpty ======]

        [TestMethod]
        public void ValidateIsNotNullOrEmpty_ReturnsNoErrors_IfStringIsNotNullOrEmpty()
        {            
            var message = NewValidatedMessage();
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsNotNullOrEmpty(_errorMessage);

            validator.Validate().AssertNoErrors();            
        }

        [TestMethod]
        public void ValidateIsNotNullOrEmpty_ReturnsExpectedError_IfStringIsNull()
        {            
            var message = new ValidatedMessage<string>(null);
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsNotNullOrEmpty(_errorMessage);

            validator.Validate().AssertOneError(_errorMessage);            
        }

        [TestMethod]
        public void ValidateIsNotNullOrEmpty_ReturnsExpectedError_IfStringIsEmpty()
        {            
            var message = new ValidatedMessage<string>(string.Empty);
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsNotNullOrEmpty(_errorMessage);

            validator.Validate().AssertOneError(_errorMessage);            
        }

        #endregion

        #region [====== IsNotNullOrWhiteSpace ======]

        [TestMethod]
        public void ValidateIsNotNullOrWhiteSpace_ReturnsNoErrors_IfStringIsNotNullOrWhiteSpace()
        {            
            var message = NewValidatedMessage();
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsNotNullOrWhiteSpace(_errorMessage);

            validator.Validate().AssertNoErrors();           
        }

        [TestMethod]
        public void ValidateIsNotNullOrWhiteSpace_ReturnsExpectedError_IfStringIsNull()
        {            
            var message = new ValidatedMessage<string>(null);
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsNotNullOrWhiteSpace(_errorMessage);

            validator.Validate().AssertOneError(_errorMessage);            
        }

        [TestMethod]
        public void ValidateIsNotNullOrWhiteSpace_ReturnsExpectedError_IfStringIsEmpty()
        {            
            var message = new ValidatedMessage<string>(string.Empty);
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsNotNullOrWhiteSpace(_errorMessage);

            validator.Validate().AssertOneError(_errorMessage);            
        }

        [TestMethod]
        public void ValidateIsNotNullOrWhiteSpace_ReturnsExpectedError_IfStringIsWhiteSpace()
        {            
            var message = new ValidatedMessage<string>("     ");
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsNotNullOrWhiteSpace(_errorMessage);

            validator.Validate().AssertOneError(_errorMessage);            
        }

        #endregion

        private static ValidatedMessage<string> NewValidatedMessage()
        {
            return new ValidatedMessage<string>(Guid.NewGuid().ToString("N"));
        }
    }
}
