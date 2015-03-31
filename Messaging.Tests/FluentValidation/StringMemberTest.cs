using System.Text.RegularExpressions;
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

        #region [====== IsNotEqualTo ======]

        [TestMethod]
        public void ValidateIsNotEqualTo_ReturnsExpectedError_IfMemberIsEqualToValue_Ordinal()
        {
            var message = new ValidatedMessage<string>("Some value");
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsNotEqualTo("Some value", StringComparison.Ordinal, _errorMessage);

            validator.Validate().AssertOneError(_errorMessage);
        }

        [TestMethod]
        public void ValidateIsNotEqualTo_ReturnsNoErrors_IfMemberIsNotEqualToValue_Ordinal()
        {
            var message = new ValidatedMessage<string>("Some value");
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsNotEqualTo("Some VALUE", StringComparison.Ordinal, _errorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsNotEqualTo_ReturnsExpectedError_IfMemberIsEqualToValue_OrdinalIgnoreCase()
        {
            var message = new ValidatedMessage<string>("Some value");
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsNotEqualTo("Some value", StringComparison.OrdinalIgnoreCase, _errorMessage);

            validator.Validate().AssertOneError(_errorMessage);
        }

        [TestMethod]
        public void ValidateIsNotEqualTo_ReturnsExpectedError_IfMemberIsEqualToValueExceptForCasing_OrdinalIgnoreCase()
        {
            var message = new ValidatedMessage<string>("Some value");
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsNotEqualTo("Some VALUE", StringComparison.OrdinalIgnoreCase, _errorMessage);

            validator.Validate().AssertOneError(_errorMessage);
        }

        [TestMethod]
        public void ValidateIsNotEqualTo_ReturnsNoErrors_IfMemberIsNotEqualToValue_OrdinalIgnoreCase()
        {
            var message = new ValidatedMessage<string>("Some value");
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsNotEqualTo("Some other value", StringComparison.OrdinalIgnoreCase, _errorMessage);

            validator.Validate().AssertNoErrors();
        }

        #endregion

        #region [====== IsEqualTo ======]

        [TestMethod]
        public void ValidateIsEqualTo_ReturnsNoErrors_IfMemberIsEqualToValue_Ordinal()
        {
            var message = new ValidatedMessage<string>("Some value");
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsEqualTo("Some value", StringComparison.Ordinal, _errorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsEqualTo_ReturnsExpectedError_IfMemberIsNotEqualToValue_Ordinal()
        {
            var message = new ValidatedMessage<string>("Some value");
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsEqualTo("Some VALUE", StringComparison.Ordinal, _errorMessage);

            validator.Validate().AssertOneError(_errorMessage);
        }

        [TestMethod]
        public void ValidateIsEqualTo_ReturnsNoErrors_IfMemberIsEqualToValue_OrdinalIgnoreCase()
        {
            var message = new ValidatedMessage<string>("Some value");
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsEqualTo("Some value", StringComparison.OrdinalIgnoreCase, _errorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsEqualTo_ReturnsNoErrors_IfMemberIsEqualToValueExceptForCasing_OrdinalIgnoreCase()
        {
            var message = new ValidatedMessage<string>("Some value");
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsEqualTo("Some VALUE", StringComparison.OrdinalIgnoreCase, _errorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsEqualTo_ReturnsExpectedError_IfMemberIsNotEqualToValue_OrdinalIgnoreCase()
        {
            var message = new ValidatedMessage<string>("Some value");
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsEqualTo("Some other value", StringComparison.OrdinalIgnoreCase, _errorMessage);

            validator.Validate().AssertOneError(_errorMessage);
        }

        #endregion

        #region [====== StartsWith ======]

        [TestMethod]
        public void ValidateStartsWith_ReturnsNoErrors_IfValueStartsWithSpecifiedPrefix()
        {
            var message = new ValidatedMessage<string>("Some value");
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).StartsWith("Some", _errorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateStartsWith_ReturnsExpectedError_IfValueIsNull()
        {
            var message = new ValidatedMessage<string>(null);
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).StartsWith("SOME", _errorMessage);

            validator.Validate().AssertOneError(_errorMessage);
        }

        [TestMethod]
        public void ValidateStartsWith_ReturnsExpectedError_IfValueDoesNotStartWithSpecifiedPrefix()
        {
            var message = new ValidatedMessage<string>("Some value");
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).StartsWith("SOME", _errorMessage);

            validator.Validate().AssertOneError(_errorMessage);
        }

        [TestMethod]
        public void ValidateStartsWith_ReturnsNoErrors_IfValueStartsWithSpecifiedPrefix_OrdinalIgnoreCase()
        {
            var message = new ValidatedMessage<string>("Some value");
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).StartsWith("SOME", StringComparison.OrdinalIgnoreCase, _errorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateStartsWith_ReturnsNoErrors_IfValueDoesNotStartWithSpecifiedPrefix_OrdinalIgnoreCase()
        {
            var message = new ValidatedMessage<string>("Some value");
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).StartsWith("value", StringComparison.OrdinalIgnoreCase, _errorMessage);

            validator.Validate().AssertOneError(_errorMessage);
        }

        #endregion

        #region [====== EndsWith ======]

        [TestMethod]
        public void ValidateEndsWith_ReturnsNoErrors_IfValueEndsWithSpecifiedPostfix()
        {
            var message = new ValidatedMessage<string>("Some value");
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).EndsWith("value", _errorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateEndsWith_ReturnsExpectedError_IfValueIsNull()
        {
            var message = new ValidatedMessage<string>(null);
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).EndsWith("value", _errorMessage);

            validator.Validate().AssertOneError(_errorMessage);
        }

        [TestMethod]
        public void ValidateEndsWith_ReturnsExpectedError_IfValueDoesNotEndWithSpecifiedPostfix()
        {
            var message = new ValidatedMessage<string>("Some value");
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).EndsWith("VALUE", _errorMessage);

            validator.Validate().AssertOneError(_errorMessage);
        }

        [TestMethod]
        public void ValidateEndsWith_ReturnsNoErrors_IfValueEndsWithSpecifiedPostfix_OrdinalIgnoreCase()
        {
            var message = new ValidatedMessage<string>("Some value");
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).EndsWith("VALUE", StringComparison.OrdinalIgnoreCase, _errorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateEndsWith_ReturnsNoErrors_IfValueDoesNotEndWithSpecifiedPostfix_OrdinalIgnoreCase()
        {
            var message = new ValidatedMessage<string>("Some value");
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).EndsWith("Some", StringComparison.OrdinalIgnoreCase, _errorMessage);

            validator.Validate().AssertOneError(_errorMessage);
        }

        #endregion

        #region [====== IsNoMatch ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ValidateIsNoMatch_Throws_IfPatternIsNull()
        {
            var message = new ValidatedMessage<string>("Some value");
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsNoMatch(null);
        }

        [TestMethod]
        public void ValidateIsNoMatch_ReturnsExpectedError_IfValueMatchesSpecifiedPattern()
        {
            var message = new ValidatedMessage<string>("Some value");
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsNoMatch("v.l", _errorMessage);

            validator.Validate().AssertOneError(_errorMessage);
        }

        [TestMethod]
        public void ValidateIsNoMatch_ReturnsNoErrors_IfValueDoesNotMatchSpecifiedPattern()
        {
            var message = new ValidatedMessage<string>("Some VALUE");
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsNoMatch("v.l", _errorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsNoMatch_ReturnsExpectedError_IfValueMatchesSpecifiedPattern_IgnoreCase()
        {
            var message = new ValidatedMessage<string>("Some VALUE");
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsNoMatch("v.l", RegexOptions.IgnoreCase, _errorMessage);

            validator.Validate().AssertOneError(_errorMessage);
        }

        [TestMethod]
        public void ValidateIsNoMatch_ReturnsNoErrors_IfValueDoesNotMatchSpecifiedPattern_IgnoreCase()
        {
            var message = new ValidatedMessage<string>("Some value");
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsNoMatch("valeu", RegexOptions.IgnoreCase, _errorMessage);

            validator.Validate().AssertNoErrors();
        }

        #endregion

        #region [====== IsMatch ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ValidateIsMatch_Throws_IfPatternIsNull()
        {
            var message = new ValidatedMessage<string>("Some value");
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsMatch(null);
        }

        [TestMethod]        
        public void ValidateIsMatch_ReturnsNoErrors_IfValueMatchesSpecifiedPattern()
        {
            var message = new ValidatedMessage<string>("Some value");
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsMatch("v.l", _errorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsMatch_ReturnsExpectedError_IfValueDoesNotMatchSpecifiedPattern()
        {
            var message = new ValidatedMessage<string>("Some VALUE");
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsMatch("v.l", _errorMessage);

            validator.Validate().AssertOneError(_errorMessage);
        }

        [TestMethod]
        public void ValidateIsMatch_ReturnsNoErrors_IfValueMatchesSpecifiedPattern_IgnoreCase()
        {
            var message = new ValidatedMessage<string>("Some VALUE");
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsMatch("v.l", RegexOptions.IgnoreCase, _errorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsMatch_ReturnsExpectedError_IfValueDoesNotMatchSpecifiedPattern_IgnoreCase()
        {
            var message = new ValidatedMessage<string>("Some value");
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsMatch("valeu", RegexOptions.IgnoreCase, _errorMessage);

            validator.Validate().AssertOneError(_errorMessage);
        }

        #endregion

        #region [====== IsByte ======]

        [TestMethod]
        public void ValidateIsByte_ReturnsNoErrors_IfValueIsByte()
        {            
            var message = new ValidatedMessage<string>("255");
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member)
                .IsByte(_errorMessage)
                .IsEqualTo(255, _errorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsByte_ReturnsNoErrors_IfValueIsTooSmallForByte()
        {
            var message = new ValidatedMessage<string>("-1");
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsByte(_errorMessage);                

            validator.Validate().AssertOneError(_errorMessage);
        }

        [TestMethod]
        public void ValidateIsByte_ReturnsNoErrors_IfValueIsTooLargeForByte()
        {
            var message = new ValidatedMessage<string>("256");
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsByte(_errorMessage);

            validator.Validate().AssertOneError(_errorMessage);
        }

        #endregion

        private static ValidatedMessage<string> NewValidatedMessage()
        {
            return new ValidatedMessage<string>(Guid.NewGuid().ToString("N"));
        }
    }
}
