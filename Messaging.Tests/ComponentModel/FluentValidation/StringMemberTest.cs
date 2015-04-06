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

        #region [====== IsSByte ======]

        [TestMethod]
        public void ValidateIsSByte_ReturnsNoErrors_IfValueIsPositiveSByte()
        {
            var message = new ValidatedMessage<string>("127");
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member)
                .IsSByte(_errorMessage)
                .IsEqualTo(127, _errorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsSByte_ReturnsNoErrors_IfValueIsNegativeSByte()
        {
            var message = new ValidatedMessage<string>("-128");
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member)
                .IsSByte(_errorMessage)
                .IsEqualTo(-128, _errorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsSByte_ReturnsNoErrors_IfValueIsTooSmallForByte()
        {
            var message = new ValidatedMessage<string>("-129");
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsSByte(_errorMessage);

            validator.Validate().AssertOneError(_errorMessage);
        }

        [TestMethod]
        public void ValidateIsSByte_ReturnsNoErrors_IfValueIsTooLargeForByte()
        {
            var message = new ValidatedMessage<string>("128");
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsSByte(_errorMessage);

            validator.Validate().AssertOneError(_errorMessage);
        }

        #endregion

        #region [====== IsChar ======]

        [TestMethod]
        public void ValidateIsChar_ReturnsNoErrors_IfStringConsistsOfOnlyASingleCharacter()
        {
            var message = new ValidatedMessage<string>("a");
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member)
                .IsChar(_errorMessage)
                .IsEqualTo('a', _errorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsChar_ReturnsExpectedError_IfStringIsEmpty()
        {
            var message = new ValidatedMessage<string>(string.Empty);
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsChar(_errorMessage);

            validator.Validate().AssertOneError(_errorMessage);
        }

        [TestMethod]
        public void ValidateIsChar_ReturnsExpectedError_IfStringHasMoreThanOneCharacter()
        {
            var message = new ValidatedMessage<string>(Guid.NewGuid().ToString());
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsChar(_errorMessage);

            validator.Validate().AssertOneError(_errorMessage);
        }

        #endregion

        #region [====== IsInt16 ======]

        [TestMethod]
        public void ValidateIsInt16_ReturnsNoErrors_IfValueIsPositiveInt16()
        {
            var message = new ValidatedMessage<string>("32767");
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member)
                .IsInt16(_errorMessage)
                .IsEqualTo(32767, _errorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsInt16_ReturnsNoErrors_IfValueIsNegativeInt16()
        {
            var message = new ValidatedMessage<string>("-32768");
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member)
                .IsInt16(_errorMessage)
                .IsEqualTo(-32768, _errorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsInt16_ReturnsNoErrors_IfValueIsTooSmallForInt16()
        {
            var message = new ValidatedMessage<string>("-32769");
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsInt16(_errorMessage);

            validator.Validate().AssertOneError(_errorMessage);
        }

        [TestMethod]
        public void ValidateIsInt16_ReturnsNoErrors_IfValueIsTooLargeForInt16()
        {
            var message = new ValidatedMessage<string>("32768");
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsInt16(_errorMessage);

            validator.Validate().AssertOneError(_errorMessage);
        }

        #endregion

        #region [====== IsUInt16 ======]

        [TestMethod]
        public void ValidateIsUInt16_ReturnsNoErrors_IfValueIsUInt16()
        {
            var message = new ValidatedMessage<string>("65535");
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member)
                .IsUInt16(_errorMessage)
                .IsEqualTo(65535, _errorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsUInt16_ReturnsNoErrors_IfValueIsTooSmallForUInt16()
        {
            var message = new ValidatedMessage<string>("-1");
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsUInt16(_errorMessage);

            validator.Validate().AssertOneError(_errorMessage);
        }

        [TestMethod]
        public void ValidateIsUInt16_ReturnsNoErrors_IfValueIsTooLargeForUInt16()
        {
            var message = new ValidatedMessage<string>("65536");
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsUInt16(_errorMessage);

            validator.Validate().AssertOneError(_errorMessage);
        }

        #endregion

        #region [====== IsInt32 ======]

        [TestMethod]
        public void ValidateIsInt32_ReturnsNoErrors_IfValueIsPositiveInt32()
        {
            var message = new ValidatedMessage<string>("2147483647");
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member)
                .IsInt32(_errorMessage)
                .IsEqualTo(2147483647, _errorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsInt32_ReturnsNoErrors_IfValueIsNegativeInt32()
        {
            var message = new ValidatedMessage<string>("-2147483648");
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member)
                .IsInt32(_errorMessage)
                .IsEqualTo(-2147483648, _errorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsInt32_ReturnsNoErrors_IfValueIsTooSmallForInt32()
        {
            var message = new ValidatedMessage<string>("-2147483649");
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsInt32(_errorMessage);

            validator.Validate().AssertOneError(_errorMessage);
        }

        [TestMethod]
        public void ValidateIsInt32_ReturnsNoErrors_IfValueIsTooLargeForInt32()
        {
            var message = new ValidatedMessage<string>("2147483648");
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsInt32(_errorMessage);

            validator.Validate().AssertOneError(_errorMessage);
        }

        #endregion

        #region [====== IsUInt32 ======]

        [TestMethod]
        public void ValidateIsUInt32_ReturnsNoErrors_IfValueIsUInt32()
        {
            var message = new ValidatedMessage<string>("4294967295");
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member)
                .IsUInt32(_errorMessage)
                .IsEqualTo(4294967295, _errorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsUInt32_ReturnsNoErrors_IfValueIsTooSmallForUInt32()
        {
            var message = new ValidatedMessage<string>("-1");
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsUInt32(_errorMessage);

            validator.Validate().AssertOneError(_errorMessage);
        }

        [TestMethod]
        public void ValidateIsUInt32_ReturnsNoErrors_IfValueIsTooLargeForUInt32()
        {
            var message = new ValidatedMessage<string>("4294967296");
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsUInt32(_errorMessage);

            validator.Validate().AssertOneError(_errorMessage);
        }

        #endregion

        #region [====== IsInt64 ======]

        [TestMethod]
        public void ValidateIsInt64_ReturnsNoErrors_IfValueIsPositiveInt64()
        {
            var message = new ValidatedMessage<string>("9223372036854775807");
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member)
                .IsInt64(_errorMessage)
                .IsEqualTo(9223372036854775807, _errorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsInt64_ReturnsNoErrors_IfValueIsNegativeInt64()
        {
            var message = new ValidatedMessage<string>("-9223372036854775808");
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member)
                .IsInt64(_errorMessage)
                .IsEqualTo(-9223372036854775808, _errorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsInt64_ReturnsNoErrors_IfValueIsTooSmallForInt64()
        {
            var message = new ValidatedMessage<string>("-9223372036854775809");
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsInt64(_errorMessage);

            validator.Validate().AssertOneError(_errorMessage);
        }

        [TestMethod]
        public void ValidateIsInt64_ReturnsNoErrors_IfValueIsTooLargeForInt64()
        {
            var message = new ValidatedMessage<string>("9223372036854775808");
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsInt64(_errorMessage);

            validator.Validate().AssertOneError(_errorMessage);
        }

        #endregion

        #region [====== IsUInt64 ======]

        [TestMethod]
        public void ValidateIsUInt64_ReturnsNoErrors_IfValueIsUInt64()
        {
            var message = new ValidatedMessage<string>("18446744073709551615");
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member)
                .IsUInt64(_errorMessage)
                .IsEqualTo(18446744073709551615, _errorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsUInt64_ReturnsNoErrors_IfValueIsTooSmallForUInt64()
        {
            var message = new ValidatedMessage<string>("-1");
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsUInt64(_errorMessage);

            validator.Validate().AssertOneError(_errorMessage);
        }

        [TestMethod]
        public void ValidateIsUInt64_ReturnsNoErrors_IfValueIsTooLargeForUInt64()
        {
            var message = new ValidatedMessage<string>("18446744073709551616");
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsUInt64(_errorMessage);

            validator.Validate().AssertOneError(_errorMessage);
        }

        #endregion

        #region [====== IsSingle ======]

        [TestMethod]
        public void ValidateIsSingle_ReturnsNoErrors_IfValueIsPositiveSingle()
        {
            var message = new ValidatedMessage<string>("3.4E+37");
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member)
                .IsSingle(_errorMessage)
                .IsEqualTo(3.4E+38F, _errorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsSingle_ReturnsNoErrors_IfValueIsNegativeSingle()
        {
            var message = new ValidatedMessage<string>("-3.4E+37");
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member)
                .IsSingle(_errorMessage)
                .IsEqualTo(-3.4E+38F, _errorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsSingle_ReturnsNoErrors_IfValueIsTooSmallForSingle()
        {
            var message = new ValidatedMessage<string>("-3.4E+38");
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsSingle(_errorMessage);

            validator.Validate().AssertOneError(_errorMessage);
        }

        [TestMethod]
        public void ValidateIsSingle_ReturnsNoErrors_IfValueIsTooLargeForSingle()
        {
            var message = new ValidatedMessage<string>("3.4E+38");
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsSingle(_errorMessage);

            validator.Validate().AssertOneError(_errorMessage);
        }

        #endregion

        #region [====== IsDouble ======]

        [TestMethod]
        public void ValidateIsDouble_ReturnsNoErrors_IfValueIsPositiveDouble()
        {
            var message = new ValidatedMessage<string>("1.7E+307");
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member)
                .IsDouble(_errorMessage)
                .IsEqualTo(1.7E+308, _errorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsDouble_ReturnsNoErrors_IfValueIsNegativeDouble()
        {
            var message = new ValidatedMessage<string>("-1.7E+307");
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member)
                .IsDouble(_errorMessage)
                .IsEqualTo(-1.7E+308, _errorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsDouble_ReturnsNoErrors_IfValueIsTooSmallForDouble()
        {
            var message = new ValidatedMessage<string>("-1.7E+308");
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsDouble(_errorMessage);

            validator.Validate().AssertOneError(_errorMessage);
        }

        [TestMethod]
        public void ValidateIsDouble_ReturnsNoErrors_IfValueIsTooLargeForDouble()
        {
            var message = new ValidatedMessage<string>("1.7E+308");
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsDouble(_errorMessage);

            validator.Validate().AssertOneError(_errorMessage);
        }

        #endregion

        #region [====== IsDecimal ======]

        [TestMethod]
        public void ValidateIsDecimal_ReturnsNoErrors_IfValueIsPositiveDecimal()
        {
            var message = new ValidatedMessage<string>("79228162514264337593543950335");
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member)
                .IsDecimal(_errorMessage)
                .IsEqualTo(79228162514264337593543950335M, _errorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsDecimal_ReturnsNoErrors_IfValueIsNegativeDecimal()
        {
            var message = new ValidatedMessage<string>("-79228162514264337593543950335");
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member)
                .IsDecimal(_errorMessage)
                .IsEqualTo(-79228162514264337593543950335M, _errorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsDecimal_ReturnsNoErrors_IfValueIsTooSmallForDecimal()
        {
            var message = new ValidatedMessage<string>("-79228162514264337593543950336");
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsDecimal(_errorMessage);

            validator.Validate().AssertOneError(_errorMessage);
        }

        [TestMethod]
        public void ValidateIsDecimal_ReturnsNoErrors_IfValueIsTooLargeForDecimal()
        {
            var message = new ValidatedMessage<string>("79228162514264337593543950336");
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsDecimal(_errorMessage);

            validator.Validate().AssertOneError(_errorMessage);
        }

        #endregion

        private static ValidatedMessage<string> NewValidatedMessage()
        {
            return new ValidatedMessage<string>(Guid.NewGuid().ToString("N"));
        }
    }
}
