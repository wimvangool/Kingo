using System;
using System.Collections.Generic;
using Kingo.BuildingBlocks.Clocks;
using Kingo.BuildingBlocks.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.BuildingBlocks.Constraints
{
    public sealed partial class BasicConstraintsTest
    {
        #region [====== Self-Constraints ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void VerifyThat_Throws_IfExpressionIsNull()
        {
            var message = new ValidatedMessage<object>(new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat<object>(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ExpressionNotSupportedException))]
        public void VerifyThat_Throws_IfExpressionBody_IsConstant()
        {
            var message = new ValidatedMessage<object>(new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => 0).IsEqualTo(0);
        }        

        [TestMethod]
        [ExpectedException(typeof(ExpressionNotSupportedException))]
        public void VerifyThat_Throws_IfExpressionBody_StartsWithNonParameterIdentifier()
        {
            var message = new ValidatedMessage<object>(new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => message).IsNotNull();
        }

        [TestMethod]
        public void VerifyThat_ReturnsNoErrors_IfExpressionBody_IsParameterOnly_And_ConstraintIsSatisfied()
        {
            var message = new ValidatedMessage<object>(new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m).IsNotNull();

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void VerifyThat_ReturnsExpectedError_IfExpressionBody_IsParameterOnly_And_ConstraintIsNotSatisfied()
        {
            var message = new ValidatedMessage<object>(new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m).IsNull("{member} is not valid.");

            validator.Validate(message).AssertInstanceError("ValidatedMessage<Object> is not valid.");
        }

        [TestMethod]
        public void VerifyThat_ReturnsExpectedError_IfExpressionBody_IsParameterOnly_And_ConstraintIsNotSatisfied_And_MessageIsOfNestedGenericType()
        {
            var message = new ValidatedMessage<Dictionary<string, object>>(new Dictionary<string, object>());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m).IsNull("{member} is not valid.");

            validator.Validate(message).AssertInstanceError("ValidatedMessage<Dictionary<String, Object>> is not valid.");
        }

        #endregion

        #region [====== Simple Member Access ======]

        [TestMethod]
        public void VerifyThat_ReturnsNoErrors_IfExpressionBody_IsSimpleMemberAccess_And_ConstraintIsSatisfied()
        {
            var message = new ValidatedMessage<object>(new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotNull();

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void VerifyThat_ReturnsExpectedError_IfExpressionIsFieldOfMember_And_ParentMemberIsNull()
        {
            var value = Clock.Current.LocalDateAndTime().Millisecond;
            var message = new ValidatedMessage<ValidatedMessage<int>>(null);
            var validator = message.CreateConstraintValidator();
            
            validator.VerifyThat(m => m.Member.Member).IsEqualTo(value);

            validator.Validate(message).AssertMemberError("Member (<null>) must refer to an instance of an object.", "Member");
        }

        [TestMethod]
        public void VerifyThat_ReturnsNoErrors_IfExpressionIsFieldOfMember_And_ConstraintIsSatisfied()
        {
            var value = Clock.Current.LocalDateAndTime().Millisecond;
            var message = new ValidatedMessage<ValidatedMessage<int>>(new ValidatedMessage<int>(value));
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member.Member).IsEqualTo(value);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void VerifyThat_ReturnsExpectedErrors_IfExpressionIsFieldOfMember_And_ConstraintIsNotSatisfied()
        {
            var value = Clock.Current.LocalDateAndTime().Millisecond;
            var message = new ValidatedMessage<ValidatedMessage<int>>(new ValidatedMessage<int>(value));
            var validator = message.CreateConstraintValidator();
            
            validator.VerifyThat(m => m.Member.Member).IsNotEqualTo(value, RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage, "Member.Member", "Member");
        }

        #endregion

        #region [====== Nullables ======]

        [TestMethod]
        public void VerifyThat_ReturnsExpectedError_IfExpressionIsValueOfNullableMember_And_ParentMemberIsNull()
        {
            var value = Clock.Current.LocalDateAndTime().Millisecond;
            var message = new ValidatedMessage<int?>(null);
            var validator = message.CreateConstraintValidator();
            
            validator.VerifyThat(m => m.Member.Value).IsEqualTo(value);

            validator.Validate(message).AssertMemberError("Member (<null>) must have a value.", "Member");
        }

        [TestMethod]
        public void VerifyThat_ReturnsNoErrors_IfExpressionIsValueOfNullableMember_And_ConstraintIsSatisfied()
        {
            var value = Clock.Current.LocalDateAndTime().Millisecond;
            var message = new ValidatedMessage<int?>(value);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member.Value).IsEqualTo(value);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void VerifyThat_ReturnsExpectedError_IfExpressionIsValueOfNullableMember_And_ConstraintIsNotSatisfied()
        {            
            var message = new ValidatedMessage<int?>(10);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member.Value).IsNotEqualTo(10);

            validator.Validate(message).AssertMemberError("Member (10) must not be equal to '10'.", "Member");
        }

        #endregion        

        #region [====== Indexers ======]

        [TestMethod]
        public void VerifyThat_ReturnsNoErrors_IfExpressionIsArrayIndexer_And_ConstraintIsSatisfied()
        {
            var message = new [] { 1, 2, 3 };
            var validator = new ConstraintValidator<int[]>();
           
            validator.VerifyThat(m => m[0]).IsEqualTo(1);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void VerifyThat_ReturnsNoErrors_IfExpressionIsArrayIndexer_And_IndexerArgumentUsesParameter_And_ConstraintIsSatisfied()
        {
            var message = new[] { 1, 2, 3 };
            var validator = new ConstraintValidator<int[]>();

            validator.VerifyThat(m => m[m.Length - 1]).IsEqualTo(3);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void VerifyThat_ReturnsExpectedError_IfExpressionIsArrayIndexer_And_ConstraintIsNotSatisfied()
        {
            var message = new[] { 1, 2, 3 };
            var validator = new ConstraintValidator<int[]>();

            validator.VerifyThat(m => m[0]).IsNotEqualTo(1);

            validator.Validate(message).AssertMemberError("Int32[0] (1) must not be equal to '1'.", "[0]");
        }

        [TestMethod]
        public void VerifyThat_ReturnsNoErrors_IfExpressionIsDictionaryIndexer_And_ConstraintIsSatisfied()
        {
            var message = new Dictionary<string, int>() { { "0", 1 }, { "1", 2 } };
            var validator = new ConstraintValidator<Dictionary<string, int>>();

            validator.VerifyThat(m => m[0.ToString()]).IsEqualTo(1);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void VerifyThat_ReturnsExpectedError_IfExpressionIsDictionaryIndexer_And_ConstraintIsNotSatisfied()
        {
            var message = new Dictionary<string, int>() { { "0", 1 }, { "1", 2 } };
            var validator = new ConstraintValidator<Dictionary<string, int>>();

            validator.VerifyThat(m => m[0.ToString()]).IsEqualTo(2);

            validator.Validate(message).AssertMemberError("Dictionary<String, Int32>[0] (1) must be equal to '2'.", "[0]");
        }              

        #endregion

        #region [====== Array Length ======]

        [TestMethod]
        public void VerifyThat_ReturnsNoErrors_IfExpressionIsArrayLength_And_ConstraintIsSatisfied()
        {
            var message = new[] { 1, 2, 3 };
            var validator = new ConstraintValidator<int[]>();

            validator.VerifyThat(m => m.Length).IsEqualTo(3);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void VerifyThat_ReturnsExpectedError_IfExpressionIsArrayLength_And_ConstraintIsNotSatisfied()
        {
            var message = new[] { 1, 2, 3 };
            var validator = new ConstraintValidator<int[]>();

            validator.VerifyThat(m => m.Length).IsEqualTo(1);

            validator.Validate(message).AssertMemberError("Length (3) must be equal to '1'.", "Length");
        }

        #endregion

        #region [====== Expression Analysis - Advanced ======]

        [TestMethod]
        public void VerifyThat_ReturnsExpectedError_IfExpressionIsDeeplyNestedMember_And_ParentIsNull()
        {
            var message = new ValidatedMessage<ValidatedMessage<string>>(new ValidatedMessage<string>(null));
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member.Member.Length).Satisfies(value => true);

            validator.Validate(message).AssertMemberError("Member.Member (<null>) must refer to an instance of an object.", "Member.Member", "Member");
        }

        [TestMethod]
        public void VerifyThat_ReturnsExpectedError_IfExpressionIsDeeplyNestedMember_And_ConstraintIsNotSatisfied()
        {
            var message = new ValidatedMessage<ValidatedMessage<string>>(new ValidatedMessage<string>("Some value"));
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member.Member.Length).IsEqualTo(11);

            validator.Validate(message).AssertMemberError("Member.Member.Length (10) must be equal to '11'.", "Member.Member.Length", "Member.Member", "Member");
        }  

        [TestMethod]
        public void VerifyThat_ReturnsExpectedError_IfExpressionIsArrayIndexerOnMember_And_ParentIsNull()
        {
            var message = new ValidatedMessage<int[]>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member[0]).Satisfies(value => true);

            validator.Validate(message).AssertMemberError("Member (<null>) must refer to an instance of an object.", "Member");
        }

        [TestMethod]
        public void VerifyThat_ReturnsExpectedError_IfExpressionIsArrayIndexerOnMember_And_ConstraintIsNotSatisfied()
        {
            var message = new ValidatedMessage<int[]>(new[] { 0, 1, 2, 3 });
            var validator = message.CreateConstraintValidator();
            
            validator.VerifyThat(m => m.Member[m.Member.Length - 1]).IsEqualTo(0);

            validator.Validate(message).AssertMemberError("Member[3] (3) must be equal to '0'.");
        }  

        #endregion
    }
}
