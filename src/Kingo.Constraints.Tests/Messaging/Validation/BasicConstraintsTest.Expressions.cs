using System;
using System.Collections.Generic;
using System.Linq;
using Kingo.Clocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging.Validation
{
    public sealed partial class BasicConstraintsTest
    {
        #region [====== Unsupported Expressions ======]

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
        [ExpectedException(typeof(ExpressionNotSupportedException))]
        public void VerifyThat_Throws_IfExpressionBody_StartsWithMethodCall()
        {
            var message = new ValidatedMessage<object>(new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.ToString()).IsNotEqualTo(string.Empty);
        }

        #endregion

        #region [====== Self-Constraints ======]

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

        private struct StringWrapper
        {
            public readonly string Value;

            public StringWrapper(string value)
            {
                Value = value;
            }
        }

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

        [TestMethod]
        public void VerifyThat_ReturnsExpectedError_IfExpressionIsMemberOfNullableValue_And_ConstraintIsNotSatisfied()
        {
            var message = new ValidatedMessage<StringWrapper?>(new StringWrapper("Some value"));
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member.Value.Value.Length).IsEqualTo(6);

            validator.Validate(message).AssertMemberError("Member.Value.Length (10) must be equal to '6'.", "Member.Value.Length", "Member.Value", "Member");
        }

        [TestMethod]
        public void VerifyThat_ReturnsNoErrors_IfExpressionIsMemberOfNullableValue_And_ConstraintIsSatisfied()
        {
            var message = new ValidatedMessage<StringWrapper?>(new StringWrapper("Some value"));
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member.Value.Value.Length).IsEqualTo(10);

            validator.Validate(message).AssertNoErrors();
        }

        #endregion        

        #region [====== Indexers - Arrays ======]

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

        #endregion

        #region [====== Indexers - MultiDimensional Arrays ======]

        [TestMethod]
        public void VerifyThat_ReturnsNoErrors_IfExpressionIsMultiDimensionalArrayIndexer_And_ConstraintIsSatisfied()
        {
            var message = new[,] { { 1, 2 }, { 3, 4 }, { 5, 6 } };
            var validator = new ConstraintValidator<int[,]>();

            validator.VerifyThat(m => m[0, 0]).IsEqualTo(1);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void VerifyThat_ReturnsNoErrors_IfExpressionIsMultiDimensionalArrayIndexer_And_IndexerArgumentUsesParameter_And_ConstraintIsSatisfied()
        {
            var message = new[,] { { 1, 2 }, { 3, 4 }, { 5, 6 } };
            var validator = new ConstraintValidator<int[,]>();

            validator.VerifyThat(m => m[1, m.GetLength(1) - 1]).IsEqualTo(4);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void VerifyThat_ReturnsExpectedError_IfExpressionIsMultiDimensionalArrayIndexer_And_ConstraintIsNotSatisfied()
        {
            var message = new[,] { { 1, 2 }, { 3, 4 }, { 5, 6 } };
            var validator = new ConstraintValidator<int[,]>();

            validator.VerifyThat(m => m[1, m.GetLength(1) - 1]).IsEqualTo(6);

            validator.Validate(message).AssertMemberError("Int32[1, 1] (4) must be equal to '6'.", "[1, 1]");
        }

        #endregion

        #region [====== Indexers - Dictionaries ======]

        [TestMethod]
        public void VerifyThat_ReturnsNoErrors_IfExpressionIsDictionaryIndexer_And_ConstraintIsSatisfied()
        {
            var message = new Dictionary<string, int>
            { { "0", 1 }, { "1", 2 } };
            var validator = new ConstraintValidator<Dictionary<string, int>>();

            validator.VerifyThat(m => m[0.ToString()]).IsEqualTo(1);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void VerifyThat_ReturnsExpectedError_IfExpressionIsDictionaryIndexer_And_ConstraintIsNotSatisfied()
        {
            var message = new Dictionary<string, int>
            { { "0", 1 }, { "1", 2 } };
            var validator = new ConstraintValidator<Dictionary<string, int>>();

            validator.VerifyThat(m => m[0.ToString()]).IsEqualTo(2);

            validator.Validate(message).AssertMemberError("Dictionary<String, Int32>[0] (1) must be equal to '2'.", "[0]");
        }           
   
        [TestMethod]
        public void VerifyThat_ReturnsNoError_IfExpressionIsArrayIndexer_And_IndexArgumentIsMethodCall_And_ConstraintIsSatisfied()
        {
            var message = new[] { 2, 3, 4 };
            var validator = new ConstraintValidator<int[]>();

            validator.VerifyThat(m => m[GetLastIndex(m)]).IsEqualTo(4);

            validator.Validate(message).AssertNoErrors();
        }

        private static int GetLastIndex(int[] array)
        {
            return array.Length - 1;
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
        public void VerifyThat_ReturnsExpectedError_IfExpressionIsArrayIndexerOnMember_And_IndexIsOutOfRange()
        {
            var message = new ValidatedMessage<int[]>(new[] { 0, 1, 2, 3 });
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member[m.Member.Length]).Satisfies(value => true);

            validator.Validate(message).AssertMemberError("Member (4 item(s)) contains no element at key or index [4].");
        }

        [TestMethod]
        public void VerifyThat_ReturnsExpectedError_IfExpressionIsArrayIndexerOnMember_And_ConstraintIsNotSatisfied()
        {
            var message = new ValidatedMessage<int[]>(new[] { 0, 1, 2, 3 });
            var validator = message.CreateConstraintValidator();
            
            validator.VerifyThat(m => m.Member[m.Member.Length - 1]).IsEqualTo(0);

            validator.Validate(message).AssertMemberError("Member[3] (3) must be equal to '0'.", "Member[3]", "Member");
        }

        [TestMethod]
        public void VerifyThat_ReturnsNoErrors_IfExpressionIsArrayIndexerOnMember_And_ConstraintIsSatisfied()
        {
            var message = new ValidatedMessage<int[]>(new[] { 0, 1, 2, 3 });
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member[m.Member.Length - 1]).IsEqualTo(3);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void VerifyThat_ReturnsExpectedError_IfExpressionIsDictionaryIndexerOnMember_And_IndexIsOutOfRange()
        {
            var message = new ValidatedMessage<Dictionary<char, string>>(new Dictionary<char, string>
            {
                { 'a', "Apple" },
                { 'b', "Banana" },
                { 'c', "Cinemon" }
            });
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member['d']).Satisfies(value => true);

            validator.Validate(message).AssertMemberError("Member (3 item(s)) contains no element at key or index [d].");
        }

        [TestMethod]
        public void VerifyThat_ReturnsExpectedError_IfExpressionIsDictionaryIndexerOnMember_And_ConstraintIsNotSatisfied()
        {
            var message = new ValidatedMessage<Dictionary<char, string>>(new Dictionary<char, string>
            {
                { 'a', "Apple" },
                { 'b', "Banana" },
                { 'c', "Cinemon" }
            });
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member[m.Member.Keys.First()]).IsEqualTo("Banana");

            validator.Validate(message).AssertMemberError("Member[a] (Apple) must be equal to 'Banana'.", "Member[a]", "Member");
        }

        [TestMethod]
        public void VerifyThat_ReturnsNoErrors_IfExpressionIsDictionaryIndexerOnMember_And_ConstraintIsSatisfied()
        {
            var message = new ValidatedMessage<Dictionary<char, string>>(new Dictionary<char, string>
            {
                { 'a', "Apple" },
                { 'b', "Banana" },
                { 'c', "Cinemon" }
            });
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member[m.Member.Keys.ElementAt(2)]).IsEqualTo("Cinemon");

            validator.Validate(message).AssertNoErrors();
        }

        #endregion
    }
}
