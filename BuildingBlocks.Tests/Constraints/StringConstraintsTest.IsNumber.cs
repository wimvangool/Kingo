using System;
using System.Globalization;
using Kingo.BuildingBlocks.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.BuildingBlocks.Constraints
{
    public sealed partial class StringConstraintsTest
    {
        #region [====== IsByte ======]

        [TestMethod]
        public void ValidateIsByte_ReturnsExpectedError_IfMemberIsNull()
        {
            var message = new ValidatedMessage<string>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsByte(RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsByte_ReturnsNoErrors_IfValueIsByte()
        {
            const byte value = 255;
            var message = new ValidatedMessage<string>(value.ToString());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsByte(RandomErrorMessage)
                .IsEqualTo(value, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsByte_ReturnsExpectedError_IfValueCannotBeConvertedToNumber()
        {
            var message = new ValidatedMessage<string>("xyz");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsByte(RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsByte_ReturnsDefaultError_IfValueCannotBeConvertedToNumber_And_NoErrorMessageIsSpecified()
        {
            var message = new ValidatedMessage<string>("xyz");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsByte();

            validator.Validate(message).AssertMemberError("Member (xyz) could not be converted to a byte.");
        }

        [TestMethod]
        public void ValidateIsByte_ReturnsExpectedError_IfValueIsTooSmallForByte()
        {
            var message = new ValidatedMessage<string>("-1");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsByte(RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsByte_ReturnsExpectedErrors_IfValueIsTooLargeForByte()
        {
            var message = new ValidatedMessage<string>("256");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsByte(RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        #endregion

        #region [====== IsSByte ======]

        [TestMethod]
        public void ValidateIsSByte_ReturnsExpectedError_IfMemberIsNull()
        {
            var message = new ValidatedMessage<string>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsSByte(RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsSByte_ReturnsNoErrors_IfValueIsPositiveSByte()
        {
            const sbyte value = 127;
            var message = new ValidatedMessage<string>(value.ToString());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsSByte(RandomErrorMessage)
                .IsEqualTo(value, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsSByte_ReturnsNoErrors_IfValueIsNegativeSByte()
        {
            const sbyte value = -128;
            var message = new ValidatedMessage<string>(value.ToString());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsSByte(RandomErrorMessage)
                .IsEqualTo(value, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsSByte_ReturnsExpectedError_IfValueCannotBeConvertedToNumber()
        {
            var message = new ValidatedMessage<string>("xyz");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsSByte(RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsSByte_ReturnsDefaultError_IfValueCannotBeConvertedToNumber_And_NoErrorMessageIsSpecified()
        {
            var message = new ValidatedMessage<string>("xyz");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsSByte();

            validator.Validate(message).AssertMemberError("Member (xyz) could not be converted to a signed byte.");
        }

        [TestMethod]
        public void ValidateIsSByte_ReturnsExpectedError_IfValueIsTooSmallForByte()
        {
            var message = new ValidatedMessage<string>("-129");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsSByte(RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsSByte_ReturnsExpectedError_IfValueIsTooLargeForByte()
        {
            var message = new ValidatedMessage<string>("128");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsSByte(RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        #endregion

        #region [====== IsChar ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ValidateIsChar_Throws_IfMemberIsNull()
        {
            var message = new ValidatedMessage<string>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsChar(RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsChar_ReturnsNoErrors_IfStringConsistsOfOnlyASingleCharacter()
        {
            const char value = 'a';
            var message = new ValidatedMessage<string>(value.ToString());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsChar(RandomErrorMessage)
                .IsEqualTo(value, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsChar_ReturnsExpectedError_IfValueCannotBeConvertedToCharacter()
        {
            var message = new ValidatedMessage<string>("xyz");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsChar(RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsChar_ReturnsDefaultError_IfValueCannotBeConvertedToCharacter_And_NoErrorMessageIsSpecified()
        {
            var message = new ValidatedMessage<string>("xyz");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsChar();

            validator.Validate(message).AssertMemberError("Member (xyz) could not be converted to a single character.");
        }

        [TestMethod]
        public void ValidateIsChar_ReturnsExpectedError_IfStringIsEmpty()
        {
            var message = new ValidatedMessage<string>(string.Empty);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsChar(RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsChar_ReturnsExpectedError_IfStringHasMoreThanOneCharacter()
        {
            var message = new ValidatedMessage<string>(Guid.NewGuid().ToString());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsChar(RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        #endregion

        #region [====== IsInt16 ======]

        [TestMethod]
        public void ValidateIsInt16_ReturnsExpectedError_IfMemberIsNull()
        {
            var message = new ValidatedMessage<string>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsInt16(RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsInt16_ReturnsNoErrors_IfValueIsPositiveInt16()
        {
            const short value = 32767;
            var message = new ValidatedMessage<string>(value.ToString());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInt16(RandomErrorMessage)
                .IsEqualTo(value, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsInt16_ReturnsNoErrors_IfValueIsNegativeInt16()
        {
            const short value = -32768;
            var message = new ValidatedMessage<string>(value.ToString());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInt16(RandomErrorMessage)
                .IsEqualTo(value, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsInt16_ReturnsExpectedError_IfValueCannotBeConvertedToNumber()
        {
            var message = new ValidatedMessage<string>("xyz");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsInt16(RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsInt16_ReturnsDefaultError_IfValueCannotBeConvertedToNumber_And_NoErrorMessageIsSpecified()
        {
            var message = new ValidatedMessage<string>("xyz");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsInt16();

            validator.Validate(message).AssertMemberError("Member (xyz) could not be converted to a 16-bit integer.");
        }

        [TestMethod]
        public void ValidateIsInt16_ReturnsExpectedError_IfValueIsTooSmallForInt16()
        {
            var message = new ValidatedMessage<string>("-32769");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsInt16(RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsInt16_ReturnsExpectedError_IfValueIsTooLargeForInt16()
        {
            var message = new ValidatedMessage<string>("32768");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsInt16(RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        #endregion

        #region [====== IsInt32 ======]

        [TestMethod]
        public void ValidateIsInt32_ReturnsExpectedError_IfMemberIsNull()
        {
            var message = new ValidatedMessage<string>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsInt32(RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsInt32_ReturnsNoErrors_IfValueIsPositiveInt32()
        {
            const int value = 2147483647;
            var message = new ValidatedMessage<string>(value.ToString());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInt32(RandomErrorMessage)
                .IsEqualTo(value, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsInt32_ReturnsNoErrors_IfValueIsNegativeInt32()
        {
            const int value = -2147483648;
            var message = new ValidatedMessage<string>(value.ToString());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInt32(RandomErrorMessage)
                .IsEqualTo(value, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsInt32_ReturnsExpectedError_IfValueCannotBeConvertedToNumber()
        {
            var message = new ValidatedMessage<string>("xyz");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsInt32(RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsInt32_ReturnsDefaultError_IfValueCannotBeConvertedToNumber_And_NoErrorMessageIsSpecified()
        {
            var message = new ValidatedMessage<string>("xyz");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsInt32();

            validator.Validate(message).AssertMemberError("Member (xyz) could not be converted to a 32-bit integer.");
        }

        [TestMethod]
        public void ValidateIsInt32_ReturnsExpectedError_IfValueIsTooSmallForInt32()
        {
            var message = new ValidatedMessage<string>("-2147483649");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsInt32(RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsInt32_ReturnsExpectedError_IfValueIsTooLargeForInt32()
        {
            var message = new ValidatedMessage<string>("2147483648");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsInt32(RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        #endregion

        #region [====== IsInt64 ======]

        [TestMethod]
        public void ValidateIsInt64_ReturnsExpectedError_IfMemberIsNull()
        {
            var message = new ValidatedMessage<string>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsInt64(RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsInt64_ReturnsNoErrors_IfValueIsPositiveInt64()
        {
            const long value = 9223372036854775807;
            var message = new ValidatedMessage<string>(value.ToString());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInt64(RandomErrorMessage)
                .IsEqualTo(value, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsInt64_ReturnsNoErrors_IfValueIsNegativeInt64()
        {
            const long value = -9223372036854775808;
            var message = new ValidatedMessage<string>(value.ToString());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInt64(RandomErrorMessage)
                .IsEqualTo(value, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsInt64_ReturnsExpectedError_IfValueCannotBeConvertedToNumber()
        {
            var message = new ValidatedMessage<string>("xyz");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsInt64(RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsInt64_ReturnsDefaultError_IfValueCannotBeConvertedToNumber_And_NoErrorMessageIsSpecified()
        {
            var message = new ValidatedMessage<string>("xyz");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsInt64();

            validator.Validate(message).AssertMemberError("Member (xyz) could not be converted to a 64-bit integer.");
        }

        [TestMethod]
        public void ValidateIsInt64_ReturnsExpectedError_IfValueIsTooSmallForInt64()
        {
            var message = new ValidatedMessage<string>("-9223372036854775809");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsInt64(RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsInt64_ReturnsExpectedError_IfValueIsTooLargeForInt64()
        {
            var message = new ValidatedMessage<string>("9223372036854775808");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsInt64(RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        #endregion

        #region [====== IsSingle ======]

        [TestMethod]
        public void ValidateIsSingle_ReturnsExpectedError_IfMemberIsNull()
        {
            var message = new ValidatedMessage<string>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsSingle(RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsSingle_ReturnsNoErrors_IfValueIsPositiveSingle()
        {
            const float value = 3.4E+38F;
            var message = new ValidatedMessage<string>("3.4E+37");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsSingle(RandomErrorMessage)
                .IsEqualTo(value, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsSingle_ReturnsNoErrors_IfValueIsNegativeSingle()
        {
            const float value = -3.4E+38F;
            var message = new ValidatedMessage<string>("-3.4E+37");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsSingle(RandomErrorMessage)
                .IsEqualTo(value, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsSingle_ReturnsExpectedError_IfValueCannotBeConvertedToNumber()
        {
            var message = new ValidatedMessage<string>("xyz");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsSingle(RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsSingle_ReturnsDefaultError_IfValueCannotBeConvertedToNumber_And_NoErrorMessageIsSpecified()
        {
            var message = new ValidatedMessage<string>("xyz");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsSingle();

            validator.Validate(message).AssertMemberError("Member (xyz) could not be converted to a 32-bit floating point number.");
        }

        [TestMethod]
        public void ValidateIsSingle_ReturnsExpectedError_IfValueIsTooSmallForSingle()
        {
            var message = new ValidatedMessage<string>("-3.4E+38");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsSingle(RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsSingle_ReturnsExpectedError_IfValueIsTooLargeForSingle()
        {
            var message = new ValidatedMessage<string>("3.4E+38");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsSingle(RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        #endregion

        #region [====== IsDouble ======]

        [TestMethod]
        public void ValidateIsDouble_ReturnsExpectedError_IfMemberIsNull()
        {
            var message = new ValidatedMessage<string>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsDouble(RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsDouble_ReturnsNoErrors_IfValueIsPositiveDouble()
        {
            const double value = 1.7E+308;
            var message = new ValidatedMessage<string>("1.7E+307");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsDouble(RandomErrorMessage)
                .IsEqualTo(value, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsDouble_ReturnsNoErrors_IfValueIsNegativeDouble()
        {
            const double value = -1.7E+308;
            var message = new ValidatedMessage<string>("-1.7E+307");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsDouble(RandomErrorMessage)
                .IsEqualTo(value, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsDouble_ReturnsExpectedError_IfValueCannotBeConvertedToNumber()
        {
            var message = new ValidatedMessage<string>("xyz");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsDouble(RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsDouble_ReturnsDefaultError_IfValueCannotBeConvertedToNumber_And_NoErrorMessageIsSpecified()
        {
            var message = new ValidatedMessage<string>("xyz");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsDouble();

            validator.Validate(message).AssertMemberError("Member (xyz) could not be converted to a 64-bit floating point number.");
        }

        [TestMethod]
        public void ValidateIsDouble_ReturnsExpectedError_IfValueIsTooSmallForDouble()
        {
            var message = new ValidatedMessage<string>("-1.7E+308");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsDouble(RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsDouble_ReturnsExpectedError_IfValueIsTooLargeForDouble()
        {
            var message = new ValidatedMessage<string>("1.7E+308");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsDouble(RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        #endregion

        #region [====== IsDecimal ======]

        [TestMethod]
        public void ValidateIsDecimal_ReturnsExpectedError_IfMemberIsNull()
        {
            var message = new ValidatedMessage<string>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsDecimal(RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsDecimal_ReturnsNoErrors_IfValueIsPositiveDecimal()
        {
            const decimal value = 79228162514264337593543950335M;
            var message = new ValidatedMessage<string>(value.ToString(CultureInfo.InvariantCulture));
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsDecimal(RandomErrorMessage)
                .IsEqualTo(value, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsDecimal_ReturnsNoErrors_IfValueIsNegativeDecimal()
        {
            const decimal value = -79228162514264337593543950335M;
            var message = new ValidatedMessage<string>(value.ToString(CultureInfo.InvariantCulture));
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsDecimal(RandomErrorMessage)
                .IsEqualTo(value, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsDecimal_ReturnsExpectedError_IfValueCannotBeConvertedToNumber()
        {
            var message = new ValidatedMessage<string>("xyz");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsDecimal(RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsDecimal_ReturnsDefaultError_IfValueCannotBeConvertedToNumber_And_NoErrorMessageIsSpecified()
        {
            var message = new ValidatedMessage<string>("xyz");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsDecimal();

            validator.Validate(message).AssertMemberError("Member (xyz) could not be converted to a 96-bit floating point number.");
        }

        [TestMethod]
        public void ValidateIsDecimal_ReturnsExpectedError_IfValueIsTooSmallForDecimal()
        {
            var message = new ValidatedMessage<string>("-79228162514264337593543950336");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsDecimal(RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsDecimal_ReturnsExpectedError_IfValueIsTooLargeForDecimal()
        {
            var message = new ValidatedMessage<string>("79228162514264337593543950336");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsDecimal(RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        #endregion
    }
}
