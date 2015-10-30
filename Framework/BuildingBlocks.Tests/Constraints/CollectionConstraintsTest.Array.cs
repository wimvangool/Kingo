using System;
using Kingo.BuildingBlocks.Clocks;
using Kingo.BuildingBlocks.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.BuildingBlocks.Constraints
{
    [TestClass]
    public sealed class ArrayConstraintsTest : ConstraintTestBase
    {
        #region [====== Length ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Length_Throws_IfArrayIsNull()
        {
            var message = new ValidatedMessage<object[]>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).Length();

            validator.Validate(message);
        }

        [TestMethod]
        public void LengthIsNotEqualTo_ReturnsExpectedErrorMessage_IfLengthIsNotEqualToSpecifiedValue()
        {
            var message = new ValidatedMessage<object[]>(new object[3]);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).Length().IsNotEqualTo(3);

            validator.Validate(message).AssertMemberError("Member.Length (3) must not be equal to '3'.");
        }

        [TestMethod]
        public void Length_ReturnsNumberOfElements_IfCollectionIsNotNull()
        {
            var count = Clock.Current.LocalDateAndTime().Hour;
            var message = new ValidatedMessage<object[]>(new object[count]);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).Length().IsEqualTo(count);

            validator.Validate(message).AssertNoErrors();
        }

        #endregion        
    }
}
