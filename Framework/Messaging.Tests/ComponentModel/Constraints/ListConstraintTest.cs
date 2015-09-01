using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ServiceComponents.ComponentModel.Constraints
{
    [TestClass]
    public sealed class ListConstraintTest : ConstraintTest
    {               
        #region [====== ElementAt ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ValidateElementAt_Throws_IfIndexIsNegative()
        {
            var message = new ValidatedMessage<IList<object>>(null);
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).ElementAt(-1);    
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void ValidateElementAt_Throws_IfCollectionIsNull()
        {
            var message = new ValidatedMessage<IList<object>>(null);
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).ElementAt(0);

            validator.Validate();
        }        

        [TestMethod]
        public void ValidateElementAt_ReturnsExpectedError_IfCollectionIsEmpty()
        {
            var message = new ValidatedMessage<IList<object>>(new object[0]);
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).ElementAt(0, RandomErrorMessage);

            validator.Validate().AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateElementAt_ReturnsDefaultError_IfCollectionIsEmpty_And_NoErrorMessageIsSpecified()
        {
            var message = new ValidatedMessage<IList<object>>(new object[0]);
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).ElementAt(0);

            validator.Validate().AssertOneError("Member (0 item(s)) contains no element at index 0.");
        }

        [TestMethod]
        public void ValidateElementAt_ReturnsNoErrors_IfCollectionContainsSpecifiedElement()
        {
            var value = new object();
            var message = new ValidatedMessage<IList<object>>(new[] { value });
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member)
                .ElementAt(0, RandomErrorMessage)
                .IsSameInstanceAs(value, RandomErrorMessage);

            validator.Validate().AssertNoErrors();
        }

        #endregion
    }
}
