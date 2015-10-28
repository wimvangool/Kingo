using System;
using System.Collections.Generic;
using Kingo.BuildingBlocks.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.BuildingBlocks.Constraints
{
    [TestClass]
    public sealed class DictionaryConstraintTest : ConstraintTestBase
    {
        private const string _Key = "Key";

        #region [====== ElementAt ======]        

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ValidateElementAt_Throws_IfCollectionIsNull()
        {
            var message = new ValidatedMessage<IDictionary<string, object>>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).ElementAt(_Key);

            validator.Validate(message);
        }        

        [TestMethod]
        public void ValidateElementAt_ReturnsExpectedError_IfCollectionIsEmpty()
        {
            var message = new ValidatedMessage<IDictionary<string, object>>(new Dictionary<string, object>());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).ElementAt(_Key, RandomErrorMessage);

            validator.Validate(message).AssertError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateElementAt_ReturnsDefaultError_IfCollectionIsEmpty_And_NoErrorMessageIsSpecified()
        {
            var message = new ValidatedMessage<IDictionary<string, object>>(new Dictionary<string, object>());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).ElementAt(_Key);

            validator.Validate(message).AssertError("Member (0 item(s)) contains no value with key 'Key'.");
        }

        [TestMethod]
        public void ValidateElementAt_ReturnsExpectedError_IfCollectionDoesNotContainsElementWithSpecifiedKey()
        {
            var value = new object();
            var dictionary = new Dictionary<string, object>() { { _Key, value } };
            var message = new ValidatedMessage<IDictionary<string, object>>(dictionary);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .ElementAt(_Key, RandomErrorMessage)
                .IsSameInstanceAs(value, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateElementAt_ReturnsNoErrors_IfCollectionContainsElement()
        {
            var value = new object();
            var dictionary = new Dictionary<string, object>() { { _Key, value } };
            var message = new ValidatedMessage<IDictionary<string, object>>(dictionary);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .ElementAt(_Key, RandomErrorMessage)
                .IsSameInstanceAs(value, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateElementAt_ChangesMemberNameAsExpected()
        {
            var value = new object();
            var dictionary = new Dictionary<string, object>() { { _Key, value } };
            var message = new ValidatedMessage<IDictionary<string, object>>(dictionary);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).ElementAt(_Key, RandomErrorMessage).IsNull();

            validator.Validate(message).AssertError("Member[Key] (System.Object) must be null.", "Member[Key]");
        }

        #endregion
    }
}
