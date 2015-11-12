using System;
using Kingo.BuildingBlocks.Clocks;
using Kingo.BuildingBlocks.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.BuildingBlocks.Constraints
{
    public sealed partial class BasicConstraintsTest
    {
        #region [====== Complex Expressions ======]

        //[TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void VerifyThat_Throws_IfExpressionIsNull()
        {
            var message = new ValidatedMessage<object>(new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat<object>(null);
        }

        //[TestMethod]
        [ExpectedException(typeof(ExpressionNotSupportedException))]
        public void VerifyThat_Throws_IfExpressionBody_IsConstant()
        {
            var message = new ValidatedMessage<object>(new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => 0).IsEqualTo(0);
        }

        //[TestMethod]
        [ExpectedException(typeof(ExpressionNotSupportedException))]
        public void VerifyThat_Throws_IfExpressionBody_IsParameterOnly()
        {
            var message = new ValidatedMessage<object>(new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m).IsNotNull();
        }

        //[TestMethod]
        [ExpectedException(typeof(ExpressionNotSupportedException))]
        public void VerifyThat_Throws_IfExpressionBody_StartsWithNonParameterVariable()
        {
            var message = new ValidatedMessage<object>(new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => message).IsNotNull();
        }

        //[TestMethod]
        public void VerifyThat_ReturnsNoErrors_IfExpressionBody_IsSimpleMemberAccess_And_ConstraintIsSatisfied()
        {
            var message = new ValidatedMessage<object>(new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotNull();

            validator.Validate(message).AssertNoErrors();
        }

        //[TestMethod]
        public void VerifyThat_ReturnsNoErrors_IfExpressionIsFieldOfMember_And_ConstraintIsSatisfied()
        {
            var value = Clock.Current.LocalDateAndTime().Millisecond;
            var message = new ValidatedMessage<ValidatedMessage<int>>(new ValidatedMessage<int>(value));
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member.Member).IsEqualTo(value);

            validator.Validate(message).AssertNoErrors();
        }

        //[TestMethod]
        public void VerifyThat_ReturnsExpectedErrors_IfExpressionIsFieldOfMember_And_ConstraintIsNotSatisfied()
        {
            var value = Clock.Current.LocalDateAndTime().Millisecond;
            var message = new ValidatedMessage<ValidatedMessage<int>>(new ValidatedMessage<int>(value));
            var validator = message.CreateConstraintValidator();

            //validator.VerifyThat(m => m.Member).IsNotNull().And(member => member.Member).IsNotEqualTo(value, RandomErrorMessage);
            validator.VerifyThat(m => m.Member.Member).IsNotEqualTo(value, RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage, "Member.Member", "Member");
        }

        //[TestMethod]
        public void VerifyThat_ReturnsExpectedErrorsIfExpressionIsFieldOfMember_And_InternalMemberIsNull()
        {
            var message = new ValidatedMessage<ValidatedMessage<int>>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member.Member).IsEqualTo(8);

            validator.Validate(message).AssertMemberError("Cannot evaluate constraint because member 'Member' is <null>.");
        }

        //[TestMethod]
        public void VerifyThat_ReturnsExpectedError_IfExpressionIsArrayIndexer_And_ConstraintFails()
        {
            var message = new ValidatedMessage<int[]>(new[] { 0, 1, 2, 3 });
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member[1]).IsEqualTo(5, RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        #endregion
    }
}
