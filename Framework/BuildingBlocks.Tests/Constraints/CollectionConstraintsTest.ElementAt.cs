using System;
using System.Collections.Generic;
using System.Linq;
using Kingo.BuildingBlocks.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.BuildingBlocks.Constraints
{   
    public sealed partial class CollectionConstraintsTest
    {
        #region [====== ElementAt (IEnumerable<>) ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Validate_Enumerable_ElementAt_Throws_IfCollectionIsNull()
        {
            var message = new ValidatedMessage<IEnumerable<object>>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThatCollection(m => m.Member).ElementAt(0);

            validator.Validate(message);
        }

        [TestMethod]
        public void Validate_Enumerable_ElementAt_ReturnsExpectedError_IfCollectionIsEmpty()
        {
            var message = new ValidatedMessage<IEnumerable<object>>(Enumerable.Empty<object>());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThatCollection(m => m.Member).ElementAt(0, RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void Validate_Enumerable_ElementAt_ReturnsDefaultError_IfCollectionIsEmpty_And_NoErrorMessageIsSpecified()
        {
            var message = new ValidatedMessage<IEnumerable<object>>(new object[0]);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThatCollection(m => m.Member).ElementAt(0);

            validator.Validate(message).AssertMemberError("Member (0 item(s)) contains no element at index 0.");
        }

        [TestMethod]
        public void Validate_Enumerable_ElementAt_ReturnsNoErrors_IfCollectionContainsElement_At_Index0()
        {
            var value = new object();
            var message = new ValidatedMessage<IEnumerable<object>>(new[] { value });
            var validator = message.CreateConstraintValidator();

            validator.VerifyThatCollection(m => m.Member)
                .ElementAt(0, RandomErrorMessage)
                .IsSameInstanceAs(value, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void Validate_Enumerable_ElementAt_ReturnsNoErrors_IfCollectionContainsElement_At_Index1()
        {
            var value = new object();
            var message = new ValidatedMessage<IEnumerable<object>>(new[] { null, value, null });
            var validator = message.CreateConstraintValidator();

            validator.VerifyThatCollection(m => m.Member)
                .ElementAt(1, RandomErrorMessage)
                .IsSameInstanceAs(value, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void Validate_Enumerable_ElementAt_ChangesMemberNameAsExpected()
        {
            var value = new object();
            var message = new ValidatedMessage<IEnumerable<object>>(new[] { value });
            var validator = message.CreateConstraintValidator();

            validator.VerifyThatCollection(m => m.Member).ElementAt(0).IsNull();

            validator.Validate(message).AssertMemberError("Member[0] (System.Object) must be null.", "Member[0]");
        }

        #endregion        

        #region [====== ElementAt (Array) ======]

        [TestMethod]        
        public void Validate_Collection_ElementAt_ReturnsExpectedError_IfIndexIsNegative()
        {
            var message = new ValidatedMessage<object[]>(new object[0]);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThatCollection(m => m.Member).ElementAt(-1, RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Validate_Collection_ElementAt_Throws_IfCollectionIsNull()
        {
            var message = new ValidatedMessage<object[]>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThatCollection(m => m.Member).ElementAt(0);

            validator.Validate(message);
        }

        [TestMethod]
        public void Validate_Collection_ElementAt_ReturnsExpectedError_IfCollectionIsEmpty()
        {
            var message = new ValidatedMessage<object[]>(new object[0]);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThatCollection(m => m.Member).ElementAt(0, RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void Validate_Collection_ElementAt_ReturnsDefaultError_IfCollectionIsEmpty_And_NoErrorMessageIsSpecified()
        {
            var message = new ValidatedMessage<object[]>(new object[0]);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThatCollection(m => m.Member).ElementAt(0);

            validator.Validate(message).AssertMemberError("Member (0 item(s)) contains no element at index 0.");
        }

        [TestMethod]
        public void Validate_Collection_ElementAt_ReturnsNoErrors_IfCollectionContainsElement_At_Index0()
        {
            var value = new object();
            var message = new ValidatedMessage<object[]>(new[] { value });
            var validator = message.CreateConstraintValidator();

            validator.VerifyThatCollection(m => m.Member)
                .ElementAt(0, RandomErrorMessage)
                .IsSameInstanceAs(value, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void Validate_Collection_ElementAt_ReturnsNoErrors_IfCollectionContainsElement_At_Index1()
        {
            var value = new object();
            var message = new ValidatedMessage<object[]>(new[] { null, value, null });
            var validator = message.CreateConstraintValidator();

            validator.VerifyThatCollection(m => m.Member)
                .ElementAt(1, RandomErrorMessage)
                .IsSameInstanceAs(value, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void Validate_Collection_ElementAt_ChangesMemberNameAsExpected()
        {
            var value = new object();
            var message = new ValidatedMessage<object[]>(new[] { value });
            var validator = message.CreateConstraintValidator();

            validator.VerifyThatCollection(m => m.Member).ElementAt(0).IsNull();

            validator.Validate(message).AssertMemberError("Member[0] (System.Object) must be null.", "Member[0]");
        }

        #endregion        

        #region [====== ElementAt (Dictionary<,>) ======]

        private const string _Key = "Key";

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Validate_Dictionary_ElementAt_Throws_IfCollectionIsNull()
        {
            var message = new ValidatedMessage<Dictionary<string, object>>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThatCollection(m => m.Member).ElementAt(_Key);

            validator.Validate(message);
        }

        [TestMethod]
        public void Validate_Dictionary_ElementAt_ReturnsExpectedError_IfCollectionIsEmpty()
        {
            var message = new ValidatedMessage<Dictionary<string, object>>(new Dictionary<string, object>());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThatCollection(m => m.Member).ElementAt(_Key, RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void Validate_Dictionary_ElementAt_ReturnsDefaultError_IfCollectionIsEmpty_And_NoErrorMessageIsSpecified()
        {
            var message = new ValidatedMessage<Dictionary<string, object>>(new Dictionary<string, object>());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThatCollection(m => m.Member).ElementAt(_Key);

            validator.Validate(message).AssertMemberError("Member (0 item(s)) contains no element with key 'Key'.");
        }

        [TestMethod]
        public void Validate_Dictionary_ElementAt_ReturnsExpectedError_IfCollectionDoesNotContainsElementWithSpecifiedKey()
        {
            var value = new object();
            var dictionary = new Dictionary<string, object>() { { _Key, value } };
            var message = new ValidatedMessage<Dictionary<string, object>>(dictionary);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThatCollection(m => m.Member)
                .ElementAt(_Key, RandomErrorMessage)
                .IsSameInstanceAs(value, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void Validate_Dictionary_ElementAt_ReturnsNoErrors_IfCollectionContainsElement()
        {
            var value = new object();
            var dictionary = new Dictionary<string, object>() { { _Key, value } };
            var message = new ValidatedMessage<Dictionary<string, object>>(dictionary);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThatCollection(m => m.Member)
                .ElementAt(_Key, RandomErrorMessage)
                .IsSameInstanceAs(value, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void Validate_Dictionary_ElementAt_ChangesMemberNameAsExpected()
        {
            var value = new object();
            var dictionary = new Dictionary<string, object>() { { _Key, value } };
            var message = new ValidatedMessage<Dictionary<string, object>>(dictionary);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThatCollection(m => m.Member).ElementAt(_Key, RandomErrorMessage).IsNull();

            validator.Validate(message).AssertMemberError("Member[Key] (System.Object) must be null.", "Member[Key]");
        }

        #endregion        
    }
}
