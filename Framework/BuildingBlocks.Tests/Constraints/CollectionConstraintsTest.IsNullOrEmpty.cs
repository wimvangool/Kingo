using System.Collections.Generic;
using System.Linq;
using Kingo.BuildingBlocks.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.BuildingBlocks.Constraints
{    
    public sealed partial class CollectionConstraintsTest
    {        
        #region [====== IsNotNullOrEmpty (IEnumerable<>) ======]

        [TestMethod]
        public void Validate_Enumerable_IsNotNullOrEmpty_ReturnsExpectedError_IfCollectionIsNull()
        {
            var message = new ValidatedMessage<IEnumerable<object>>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThatCollection(m => m.Member).IsNotNullOrEmpty(RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void Validate_Enumerable_IsNotNullOrEmpty_ReturnsDefaultError_IfCollectionIsNull_And_NoErrorMessageIsSpecified()
        {
            var message = new ValidatedMessage<IEnumerable<object>>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThatCollection(m => m.Member).IsNotNullOrEmpty();

            validator.Validate(message).AssertMemberError("Member (<null>) must not be null and contain at least one element.");
        }

        [TestMethod]
        public void Validate_Enumerable_IsNotNullOrEmpty_ReturnsExpectedError_IfCollectionIsEmpty()
        {
            var message = new ValidatedMessage<IEnumerable<object>>(Enumerable.Empty<object>());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThatCollection(m => m.Member).IsNotNullOrEmpty(RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void Validate_Enumerable_IsNotNullOrEmpty_ReturnsNoErrors_IfCollectionHasOneElement()
        {
            var message = new ValidatedMessage<IEnumerable<object>>(new [] { new object() });
            var validator = message.CreateConstraintValidator();

            validator.VerifyThatCollection(m => m.Member).IsNotNullOrEmpty(RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        #endregion

        #region [====== IsNullOrEmpty (Enumerable<>) ======]

        [TestMethod]
        public void Validate_Enumerable_IsNullOrEmpty_ReturnsNoErrors_IfCollectionIsNull()
        {
            var message = new ValidatedMessage<IEnumerable<object>>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThatCollection(m => m.Member).IsNullOrEmpty();

            validator.Validate(message).AssertNoErrors();
        }        

        [TestMethod]
        public void Validate_Enumerable_IsNullOrEmpty_ReturnsNoErrors_IfCollectionIsEmpty()
        {
            var message = new ValidatedMessage<IEnumerable<object>>(Enumerable.Empty<object>());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThatCollection(m => m.Member).IsNullOrEmpty();

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void Validate_Enumerable_IsNullOrEmpty_ReturnsExpectedError_IfCollectionHasOneElement()
        {
            var message = new ValidatedMessage<IEnumerable<object>>(new[] { new object() });
            var validator = message.CreateConstraintValidator();

            validator.VerifyThatCollection(m => m.Member).IsNullOrEmpty(RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void Validate_Enumerable_IsNullOrEmpty_ReturnsDefaultError_IfCollectionHasOneElement_And_NoErrorMessageIsSpecified()
        {
            var message = new ValidatedMessage<IEnumerable<object>>(new[] { new object() });
            var validator = message.CreateConstraintValidator();

            validator.VerifyThatCollection(m => m.Member).IsNullOrEmpty();

            validator.Validate(message).AssertMemberError("Member (1 item(s)) must be null or empty.");
        }

        #endregion

        #region [====== IsNotNullOrEmpty (Array) ======]

        [TestMethod]
        public void Validate_Collection_IsNotNullOrEmpty_ReturnsExpectedError_IfCollectionIsNull()
        {
            var message = new ValidatedMessage<object[]>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThatCollection(m => m.Member).IsNotNullOrEmpty(RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void Validate_Collection_IsNotNullOrEmpty_ReturnsDefaultError_IfCollectionIsNull_And_NoErrorMessageIsSpecified()
        {
            var message = new ValidatedMessage<object[]>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThatCollection(m => m.Member).IsNotNullOrEmpty();

            validator.Validate(message).AssertMemberError("Member (<null>) must not be null and contain at least one element.");
        }

        [TestMethod]
        public void Validate_Collection_IsNotNullOrEmpty_ReturnsExpectedError_IfCollectionIsEmpty()
        {
            var message = new ValidatedMessage<object[]>(new object[0]);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThatCollection(m => m.Member).IsNotNullOrEmpty(RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void Validate_Collection_IsNotNullOrEmpty_ReturnsDefaultError_IfCollectionIsEmpty_And_NoErrorMessageIsSpecified()
        {
            var message = new ValidatedMessage<object[]>(new object[0]);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThatCollection(m => m.Member).IsNotNullOrEmpty();

            validator.Validate(message).AssertMemberError("Member (System.Object[]) must not be null and contain at least one element.");
        }

        [TestMethod]
        public void Validate_Collection_IsNotNullOrEmpty_ReturnsNoErrors_IfCollectionHasOneElement()
        {
            var message = new ValidatedMessage<object[]>(new[] { new object() });
            var validator = message.CreateConstraintValidator();

            validator.VerifyThatCollection(m => m.Member).IsNotNullOrEmpty(RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        #endregion

        #region [====== IsNullOrEmpty (Array) ======]

        [TestMethod]
        public void Validate_Collection_IsNullOrEmpty_ReturnsNoErrors_IfCollectionIsNull()
        {
            var message = new ValidatedMessage<object[]>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThatCollection(m => m.Member).IsNullOrEmpty();

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void Validate_Collection_IsNullOrEmpty_ReturnsNoErrors_IfCollectionIsEmpty()
        {
            var message = new ValidatedMessage<object[]>(new object[0]);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThatCollection(m => m.Member).IsNullOrEmpty();

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void Validate_Collection_IsNullOrEmpty_ReturnsExpectedError_IfCollectionHasOneElement()
        {
            var message = new ValidatedMessage<object[]>(new[] { new object() });
            var validator = message.CreateConstraintValidator();

            validator.VerifyThatCollection(m => m.Member).IsNullOrEmpty(RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void Validate_Collection_IsNullOrEmpty_ReturnsDefaultError_IfCollectionHasOneElement_And_NoErrorMessageIsSpecified()
        {
            var message = new ValidatedMessage<object[]>(new[] { new object() });
            var validator = message.CreateConstraintValidator();

            validator.VerifyThatCollection(m => m.Member).IsNullOrEmpty();

            validator.Validate(message).AssertMemberError("Member (1 item(s)) must be null or empty.");
        }

        #endregion
    }
}
