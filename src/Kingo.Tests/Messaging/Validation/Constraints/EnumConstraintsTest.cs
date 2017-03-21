using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging.Validation.Constraints
{
    [TestClass]
    public sealed class EnumConstraintsTest : ConstraintTestBase
    {
        #region [====== Enums ======]

        private enum NonFlagsEnum
        {
            Zero = 0,

            One = 1,

            Two = 2,

            Three = One | Two,

            Five = 5
        }

        [Flags]
        private enum FlagsEnum
        {
            Zero = 0,

            One = 1,

            Two = 2,

            Three = One | Two,

            Four = 4
        }

        #endregion

        #region [====== IsInRangeOfValidValues ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ValidateIsInRangeOfValidValues_Throws_IfValueIsNotOfEnumType()
        {
            var message = new ValidatedMessage<int>(0);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsInRangeOfValidValues();

            validator.Validate(message);
        }

        [TestMethod]
        public void ValidateIsInRangeOfValidValues_ReturnsExpectedError_IfValueIsNonFlagsEnum_And_ValueDoesNotMatchAnyEnumValue()
        {
            var message = new ValidatedMessage<NonFlagsEnum>((NonFlagsEnum) 4);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsInRangeOfValidValues();

            validator.Validate(message).AssertMemberError("Member (4) has a value or contains bitflags that are not defined by type NonFlagsEnum.");
        }

        [TestMethod]
        public void ValidateIsInRangeOfValidValues_ReturnsNoErrors_IfValueIsNonFlagsEnum_And_ValueMatchesCertainEnumValue()
        {
            var message = new ValidatedMessage<NonFlagsEnum>(NonFlagsEnum.Five);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsInRangeOfValidValues();

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsInRangeOfValidValues_ReturnsNoErrors_IfValueIsNonFlagsEnum_And_ValueMatchesBitwiseOREnumValue()
        {
            var message = new ValidatedMessage<NonFlagsEnum>(NonFlagsEnum.Three);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsInRangeOfValidValues();

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsInRangeOfValidValues_ReturnsExpectedError_IfValueIsFlagsEnum_And_ValueIsNotInBitScopeOfAnyEnumValue()
        {
            var message = new ValidatedMessage<FlagsEnum>((FlagsEnum) 12);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsInRangeOfValidValues();

            validator.Validate(message).AssertMemberError("Member (12) has a value or contains bitflags that are not defined by type FlagsEnum.");
        }

        [TestMethod]
        public void ValidateIsInRangeOfValidValues_ReturnsNoErrors_IfValueIsFlagsEnum_And_ValueIsCombinationOfDefinedValues()
        {
            var message = new ValidatedMessage<FlagsEnum>((FlagsEnum) 5);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsInRangeOfValidValues();

            validator.Validate(message).AssertNoErrors();
        }       

        [TestMethod]
        public void ValidateIsInRangeOfValidValues_ReturnsNoErrors_IfValueIsFlagsEnum_And_ValueMatchesCertainEnumValue()
        {
            var message = new ValidatedMessage<FlagsEnum>(FlagsEnum.Zero);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsInRangeOfValidValues();

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsInRangeOfValidValues_ReturnsNoErrors_IfValueIsFlagsEnum_And_ValueMatchesBitwiseOREnumValue()
        {
            var message = new ValidatedMessage<FlagsEnum>(FlagsEnum.Three);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsInRangeOfValidValues();

            validator.Validate(message).AssertNoErrors();
        }

        #endregion

        #region [====== IsDefined ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ValidateIsDefined_Throws_IfValueIsNotOfEnumType()
        {
            var message = new ValidatedMessage<int>(0);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsDefined();

            validator.Validate(message);
        }

        [TestMethod]        
        public void ValidateIsDefined_ReturnsExpectedError_IfValueIsNonFlagsEnum_And_ValueDoesNotMatchAnyEnumValue()
        {
            var message = new ValidatedMessage<NonFlagsEnum>((NonFlagsEnum) 4);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsDefined();

            validator.Validate(message).AssertMemberError("Member (4) is not defined in Enum of type NonFlagsEnum.");
        }

        [TestMethod]
        public void ValidateIsDefined_ReturnsNoErrors_IfValueIsNonFlagsEnum_And_ValueMatchesCertainEnumValue()
        {
            var message = new ValidatedMessage<NonFlagsEnum>(NonFlagsEnum.Five);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsDefined();

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsDefined_ReturnsNoErrors_IfValueIsNonFlagsEnum_And_ValueMatchesBitwiseOREnumValue()
        {
            var message = new ValidatedMessage<NonFlagsEnum>(NonFlagsEnum.Three);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsDefined();

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsDefined_ReturnsExpectedError_IfValueIsFlagsEnum_And_ValueIsNotInBitScopeOfAnyEnumValue()
        {
            var message = new ValidatedMessage<FlagsEnum>((FlagsEnum)12);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsDefined();

            validator.Validate(message).AssertMemberError("Member (12) is not defined in Enum of type FlagsEnum.");
        }

        [TestMethod]
        public void ValidateIsDefined_ReturnsExpectedError_IfValueIsFlagsEnum_And_ValueIsCombinationOfDefinedValues()
        {
            var message = new ValidatedMessage<FlagsEnum>((FlagsEnum) 5);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsDefined();

            validator.Validate(message).AssertMemberError("Member (One, Four) is not defined in Enum of type FlagsEnum.");
        }        

        [TestMethod]
        public void ValidateIsDefined_ReturnsNoErrors_IfValueIsFlagsEnum_And_ValueMatchesCertainEnumValue()
        {
            var message = new ValidatedMessage<FlagsEnum>(FlagsEnum.Zero);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsDefined();

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsDefined_ReturnsNoErrors_IfValueIsFlagsEnum_And_ValueMatchesBitwiseOREnumValue()
        {
            var message = new ValidatedMessage<FlagsEnum>(FlagsEnum.Three);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsDefined();

            validator.Validate(message).AssertNoErrors();
        }

        #endregion

        #region [====== HasFlag ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ValidateHasFlag_Throws_IfFlagIsNotOfEnumType()
        {
            var message = new ValidatedMessage<int>(0);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).HasFlag(2);

            validator.Validate(message);
        }

        [TestMethod]        
        public void ValidateHasFlag_ReturnsExpectedError_IfValueDoesNotHaveAllBitFlagsSet()
        {
            var message = new ValidatedMessage<FlagsEnum>(FlagsEnum.Zero);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).HasFlag(FlagsEnum.One | FlagsEnum.Four);

            validator.Validate(message).AssertMemberError("Member (Zero) does not have all bitflags specified by 'One, Four' set.");
        }

        [TestMethod]
        public void ValidateHasFlag_ReturnsNoErrors_IfValueHasAllBitFlagsSet()
        {
            var message = new ValidatedMessage<FlagsEnum>(FlagsEnum.Three | FlagsEnum.Four);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).HasFlag(FlagsEnum.One | FlagsEnum.Four);

            validator.Validate(message).AssertNoErrors();
        }

        #endregion
    }
}
