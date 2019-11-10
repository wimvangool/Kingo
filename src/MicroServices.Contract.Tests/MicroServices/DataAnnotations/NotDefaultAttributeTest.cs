using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices.DataAnnotations
{
    [TestClass]
    public sealed class NotDefaultAttributeTest : RequestMessageTest<NotDefaultAttributeTest.RequestMessageStub>
    {
        private readonly NotDefaultAttribute _attribute;

        public NotDefaultAttributeTest()
        {
            _attribute = new NotDefaultAttribute();
        }

        #region [====== IsValid ======]

        [TestMethod]
        public void IsValid_ReturnsTrue_IfValueIsNull()
        {
            Assert.IsTrue(_attribute.IsValid(null));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void IsValid_Throws_IfValueIsReferenceType()
        {
            _attribute.IsValid(new object());
        }

        [TestMethod]
        public void IsValid_ReturnsFalse_IfValueIsDefaultValue()
        {
            Assert.IsFalse(_attribute.IsValid(0));
        }

        [TestMethod]
        public void IsValid_ReturnsTrue_IfValueIsNonDefaultValue()
        {
            Assert.IsTrue(_attribute.IsValid(1));
        }

        #endregion

        #region [====== Error Message ======]

        public sealed class RequestMessageStub
        {
            [NotDefault]
            public Guid A
            {
                get;
                set;
            }

            [NotDefault(ErrorMessage = "{0} is not valid.")]
            public Guid B
            {
                get;
                set;
            }
        }

        [TestMethod]
        public void Message_IsNotValid_IfAHasDefaultValue()
        {
            AssertThat(message =>
            {
                message.A = Guid.Empty;

            }).IsNotValid(1).And(errors =>
            {
                errors["A"].HasError("A must have a non-default value.");
            });
        }

        [TestMethod]
        public void Message_IsNotValid_IfBHasDefaultValue()
        {
            AssertThat(message =>
            {
                message.B = Guid.Empty;

            }).IsNotValid(1).And(errors =>
            {
                errors["B"].HasError("B is not valid.");
            });
        }

        protected override RequestMessageStub CreateValidRequestMessage()
        {
            return new RequestMessageStub()
            {
                A = Guid.NewGuid(),
                B = Guid.NewGuid()
            };
        }

        #endregion
    }
}
