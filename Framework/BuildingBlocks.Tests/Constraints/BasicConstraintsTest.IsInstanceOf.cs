using Kingo.BuildingBlocks.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.BuildingBlocks.Constraints
{
    public sealed partial class BasicConstraintsTest
    {
        #region [====== IsNotInstanceOf =======]

        [TestMethod]
        public void ValidateIsNotInstanceOf_ReturnsNoErrors_IfMemberIsNull()
        {
            var message = new ValidatedMessage<object>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotInstanceOf(typeof(object), RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsNotInstanceOfOther_ReturnsNoErrors_IfObjectIsNull()
        {
            var message = new ValidatedMessage<object>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotInstanceOf<object>(RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsNotInstanceOfOther_ReturnsExpectedError_IfObjectIsNotOfSpecifiedType()
        {
            var message = new ValidatedMessage<object>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotInstanceOf<object>(RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsNotInstanceOfOther_ReturnsDefaultError_IfObjectIsNotOfSpecifiedType_And_NoErrorMessageIsSpecified()
        {
            var message = new ValidatedMessage<object>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotInstanceOf<object>();

            validator.Validate(message).AssertMemberError("Member of type 'System.String' must not be an instance of type 'System.Object'.");
        }

        [TestMethod]
        public void ValidateIsNotInstanceOfOther_ReturnsNoErrors_IfObjectIsNotOfSpecifiedType()
        {
            var message = new ValidatedMessage<object>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotInstanceOf<int>(RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsNotInstanceOf_ReturnsExpectedError_IfObjectIsNotOfSpecifiedType()
        {
            var message = new ValidatedMessage<object>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsNotInstanceOf(typeof(object), RandomErrorMessage)
                .IsNotInstanceOf(typeof(string), RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsNotInstanceOf_ReturnsNoError_IfObjectIsNotOfSpecifiedType()
        {
            var message = new ValidatedMessage<object>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotInstanceOf(typeof(int), RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        #endregion

        #region [====== IsNotInstanceOf (Indirect) =======]

        [TestMethod]
        public void ValidateIsNotInstanceOfOther_ReturnsNoErrors_IfObjectIsNotOfSpecifiedType_Indirect()
        {
            var message = new ValidatedMessage<object>("Some value")
            {
                ExpectedMemberType = typeof(int)
            };
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotInstanceOf(m => m.ExpectedMemberType);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsNotInstanceOfOther_ReturnsExpectedError_IfObjectIsOfSpecifiedType_Indirect()
        {
            var message = new ValidatedMessage<object>("Some value")
            {
                ExpectedMemberType = typeof(string)
            };
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotInstanceOf(m => m.ExpectedMemberType, RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        #endregion

        #region [====== IsInstanceOf =======]

        [TestMethod]
        public void ValidateIsInstanceOfOther_ReturnsNoErrors_IfMemberIsNull_And_ValueIsReferenceType()
        {
            var message = new ValidatedMessage<object>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsInstanceOf<object>(RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsInstanceOfOther_ReturnsExpectedError_IfMemberIsNull_And_ValueIsValueType()
        {
            var message = new ValidatedMessage<object>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsInstanceOf<int>(RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsInstanceOfOther_ReturnsExpectedError_IfObjectIsNotOfSpecifiedType()
        {
            var message = new ValidatedMessage<object>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsInstanceOf<int>(RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsInstanceOfOther_ReturnsDefaultError_IfObjectIsNotOfSpecifiedType_And_NoErrorMessageIsSpecified()
        {
            var message = new ValidatedMessage<object>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsInstanceOf<int>();

            validator.Validate(message).AssertMemberError("Member of type 'System.String' must be an instance of type 'System.Int32'.");
        }

        [TestMethod]
        public void ValidateIsInstanceOfOther_ReturnsNoErrors_IfObjectIsOfSpecifiedType()
        {
            var message = new ValidatedMessage<object>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInstanceOf<object>(RandomErrorMessage)
                .IsInstanceOf<string>(RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsInstanceOf_ReturnsNoErrors_IfObjectIsNull_And_TypeIsReferenceType()
        {
            var message = new ValidatedMessage<object>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsInstanceOf(typeof(object));

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsInstanceOf_ReturnsExpectedErrors_IfObjectIsNull_And_TypeIsValueType()
        {
            var message = new ValidatedMessage<object>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsInstanceOf(typeof(int), RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsInstanceOf_ReturnsExpectedError_IfObjectIsNotOfSpecifiedType()
        {
            var message = new ValidatedMessage<object>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsInstanceOf(typeof(int), RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsInstanceOf_ReturnsDefaultError_IfObjectIsNotOfSpecifiedType_And_NoErrorMessageIsSpecified()
        {
            var message = new ValidatedMessage<object>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsInstanceOf(typeof(int));

            validator.Validate(message).AssertMemberError("Member of type 'System.String' must be an instance of type 'System.Int32'.");
        }

        [TestMethod]
        public void ValidateIsInstanceOf_ReturnsNoErrors_IfObjectIsOfSpecifiedType()
        {
            var message = new ValidatedMessage<object>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInstanceOf(typeof(object), RandomErrorMessage)
                .IsInstanceOf(typeof(string), RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        #endregion

        #region [====== IsInstanceOf (Indirect) =======]

        [TestMethod]
        public void ValidateIsInstanceOfOther_ReturnsExpectedError_IfObjectIsNotOfSpecifiedType_Indirect()
        {
            var message = new ValidatedMessage<object>("Some value")
            {
                ExpectedMemberType = typeof(int)
            };
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsInstanceOf(m => m.ExpectedMemberType, RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsInstanceOfOther_ReturnsNoErrors_IfObjectIsOfSpecifiedType_Indirect()
        {
            var message = new ValidatedMessage<object>("Some value")
            {
                ExpectedMemberType = typeof(string)
            };
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsInstanceOf(m => m.ExpectedMemberType);

            validator.Validate(message).AssertNoErrors();
        }

        #endregion
    }
}
