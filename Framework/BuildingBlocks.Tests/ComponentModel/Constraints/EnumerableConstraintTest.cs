using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.BuildingBlocks.ComponentModel.Constraints
{
    [TestClass]
    public sealed class EnumerableConstraintTest : ConstraintTest
    {        
        #region [====== IsNotNullOrEmpty ======]

        [TestMethod]
        public void ValidateIsNotNullOrEmpty_ReturnsExpectedError_IfCollectionIsNull()
        {
            var message = new ValidatedMessage<IEnumerable<object>>(null);
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).IsNotNullOrEmpty(RandomErrorMessage);

            validator.Validate().AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsNotNullOrEmpty_ReturnsDefaultError_IfCollectionIsNull_And_NoErrorMessageIsSpecified()
        {
            var message = new ValidatedMessage<IEnumerable<object>>(null);
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).IsNotNullOrEmpty();

            validator.Validate().AssertOneError("Member (<null>) must not be null and contain at least one element.");
        }

        [TestMethod]
        public void ValidateIsNotNullOrEmpty_ReturnsExpectedError_IfCollectionIsEmpty()
        {
            var message = new ValidatedMessage<IEnumerable<object>>(Enumerable.Empty<object>());
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).IsNotNullOrEmpty(RandomErrorMessage);

            validator.Validate().AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsNotNullOrEmpty_ReturnsNoErrors_IfCollectionHasOneElement()
        {
            var message = new ValidatedMessage<IEnumerable<object>>(new [] { new object() });
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).IsNotNullOrEmpty(RandomErrorMessage);

            validator.Validate().AssertNoErrors();
        }

        #endregion

        #region [====== IsNullOrEmpty ======]

        [TestMethod]
        public void ValidateIsNullOrEmpty_ReturnsNoErrors_IfCollectionIsNull()
        {
            var message = new ValidatedMessage<IEnumerable<object>>(null);
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).IsNullOrEmpty();

            validator.Validate().AssertNoErrors();
        }        

        [TestMethod]
        public void ValidateIsNullOrEmpty_ReturnsNoErrors_IfCollectionIsEmpty()
        {
            var message = new ValidatedMessage<IEnumerable<object>>(Enumerable.Empty<object>());
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).IsNullOrEmpty();

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsNullOrEmpty_ReturnsExpectedError_IfCollectionHasOneElement()
        {
            var message = new ValidatedMessage<IEnumerable<object>>(new[] { new object() });
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).IsNullOrEmpty(RandomErrorMessage);

            validator.Validate().AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsNullOrEmpty_ReturnsDefaultError_IfCollectionHasOneElement_And_NoErrorMessageIsSpecified()
        {
            var message = new ValidatedMessage<IEnumerable<object>>(new[] { new object() });
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).IsNullOrEmpty();

            validator.Validate().AssertOneError("Member must be null or empty.");
        }

        #endregion

        #region [====== ElementAt ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ValidateElementAt_Throws_IfCollectionIsNull()
        {
            var message = new ValidatedMessage<IEnumerable<object>>(null);
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).ElementAt(0);

            validator.Validate();
        }      

        [TestMethod]
        public void ValidateElementAt_ReturnsExpectedError_IfCollectionIsEmpty()
        {
            var message = new ValidatedMessage<IEnumerable<object>>(Enumerable.Empty<object>());
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).ElementAt(0, RandomErrorMessage);

            validator.Validate().AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateElementAt_ReturnsDefaultError_IfCollectionIsEmpty_And_NoErrorMessageIsSpecified()
        {
            var message = new ValidatedMessage<IEnumerable<object>>(new object[0]);
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).ElementAt(0);

            validator.Validate().AssertOneError("Member contains no element at index 0.");
        }

        [TestMethod]
        public void ValidateElementAt_ReturnsNoErrors_IfCollectionContainsSpecifiedElement()
        {
            var value = new object();
            var message = new ValidatedMessage<IEnumerable<object>>(new [] { value });
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member)
                .ElementAt(0, RandomErrorMessage)
                .IsSameInstanceAs(value, RandomErrorMessage);

            validator.Validate().AssertNoErrors();
        }

        #endregion
    }
}
