using System;
using System.Collections.Generic;
using Kingo.BuildingBlocks.Clocks;
using Kingo.BuildingBlocks.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.BuildingBlocks.Constraints
{
    [TestClass]
    public sealed class ListConstraintTest : ConstraintTestBase
    {
        #region [====== Count ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Count_Throws_IfCollectionIsNull()
        {
            var message = new ValidatedMessage<IList<object>>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).Count();

            validator.Validate(message);
        }

        [TestMethod]
        public void LengthIsNotEqualTo_ReturnsExpectedErrorMessage_IfLengthIsNotEqualToSpecifiedValue()
        {
            var message = new ValidatedMessage<IList<object>>(new object[3]);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).Count().IsNotEqualTo(3);

            validator.Validate(message).AssertMemberError("Member.Count (3) must not be equal to '3'.");
        }

        [TestMethod]
        public void Count_ReturnsNumberOfElements_IfCollectionIsNotNull()
        {
            var count = Clock.Current.LocalDateAndTime().Hour;
            var message = new ValidatedMessage<IList<object>>(new object[count]);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).Count().IsEqualTo(count);

            validator.Validate(message).AssertNoErrors();
        }

        #endregion

        #region [====== ElementAt ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ValidateElementAt_Throws_IfIndexIsNegative()
        {
            var message = new ValidatedMessage<IList<object>>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).ElementAt(-1);    
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ValidateElementAt_Throws_IfCollectionIsNull()
        {
            var message = new ValidatedMessage<IList<object>>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).ElementAt(0);

            validator.Validate(message);
        }        

        [TestMethod]
        public void ValidateElementAt_ReturnsExpectedError_IfCollectionIsEmpty()
        {
            var message = new ValidatedMessage<IList<object>>(new object[0]);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).ElementAt(0, RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateElementAt_ReturnsDefaultError_IfCollectionIsEmpty_And_NoErrorMessageIsSpecified()
        {
            var message = new ValidatedMessage<IList<object>>(new object[0]);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).ElementAt(0);

            validator.Validate(message).AssertMemberError("Member (0 item(s)) contains no element at index 0.");
        }

        [TestMethod]
        public void ValidateElementAt_ReturnsNoErrors_IfCollectionContainsElement_At_Index0()
        {
            var value = new object();
            var message = new ValidatedMessage<IList<object>>(new[] { value });
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .ElementAt(0, RandomErrorMessage)
                .IsSameInstanceAs(value, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateElementAt_ReturnsNoErrors_IfCollectionContainsElement_At_Index1()
        {
            var value = new object();
            var message = new ValidatedMessage<IList<object>>(new[] { null, value, null });
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .ElementAt(1, RandomErrorMessage)
                .IsSameInstanceAs(value, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateElementAt_ChangesMemberNameAsExpected()
        {
            var value = new object();
            var message = new ValidatedMessage<IList<object>>(new[] { value });
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).ElementAt(0, RandomErrorMessage).IsNull();

            validator.Validate(message).AssertMemberError("Member[0] (System.Object) must be null.", "Member[0]");
        }

        #endregion
    }
}
