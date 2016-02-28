using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Kingo.Constraints;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging
{
    [TestClass]
    public sealed partial class MessageTest
    {                
        #region [====== MessageWithoutAttributes ======]

        private sealed class MessageWithoutAttributes : Message
        {
            internal readonly int Value;
    
            public MessageWithoutAttributes(int value)
            {
                Value = value;
            }
        }

        #endregion

        #region [====== MessageWithPrimitiveMembers ======]

        [DataContract]
        private sealed class MessageWithPrimitiveMembers : Message
        {
            [DataMember]
            internal readonly int IntValue;

            [DataMember]
            internal readonly List<int> IntValues;

            public MessageWithPrimitiveMembers(int intValue, IEnumerable<int> values)
            {
                IntValue = intValue;                
                IntValues = new List<int>(values);
            }           

            protected override IValidator CreateValidator()
            {
                var validator = new ConstraintValidator<MessageWithPrimitiveMembers>();

                validator.VerifyThat(m => m.IntValue).IsGreaterThan(0);
                validator.VerifyThat(m => m.IntValues).IsNotNullOrEmpty();

                return validator;
            }
        }

        #endregion

        #region [====== MessageWithMemberOfOwnType ======]

        [DataContract]
        private sealed class MessageWithMemberOfOwnType : Message
        {
            [DataMember] internal readonly int Value;
            [DataMember] internal readonly MessageWithMemberOfOwnType Child;           

            public MessageWithMemberOfOwnType(int value, MessageWithMemberOfOwnType child)
            {
                Value = value;
                Child = child;
            }
        }

        #endregion        

        #region [====== Copy ======]

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void Copy_Throws_IfMessageLacksRequiredAttributes()
        {            
            new MessageWithoutAttributes(GenerateIntValue()).Copy();            
        }

        [TestMethod]
        public void Copy_ReturnsExpectedCopy_IfMessageIsSerializable()
        {
            var message = new SerializableMessage(GenerateIntValue(), GenerateStringValue());
            var copy = message.Copy() as SerializableMessage;

            Assert.IsNotNull(copy);
            Assert.AreNotSame(message, copy);
            Assert.AreEqual(message, copy);
        }

        [TestMethod]
        public void Copy_ReturnsExpectedCopy_IfMessageHasNoMembersToCopy()
        {
            var message = new EmptyMessage();
            var copy = message.Copy() as EmptyMessage;

            Assert.IsNotNull(copy);
            Assert.AreNotSame(message, copy);            
        }

        [TestMethod]
        public void Copy_ReturnsExpectedCopy_IsMessageSpecifiesMemberAttributes()
        {
            var values = new[] { GenerateIntValue(), GenerateIntValue() + 1, GenerateIntValue() + 2 };
            var message = new MessageWithPrimitiveMembers(GenerateIntValue(), values);
            var copy = (MessageWithPrimitiveMembers) message.Copy();

            Assert.IsNotNull(copy);
            Assert.AreNotSame(message, copy);
            Assert.AreEqual(message.IntValue, copy.IntValue);
            Assert.IsNotNull(copy.IntValues);
            Assert.AreEqual(3, copy.IntValues.Count);
            Assert.AreEqual(values[0], copy.IntValues[0]);
            Assert.AreEqual(values[1], copy.IntValues[1]);
            Assert.AreEqual(values[2], copy.IntValues[2]);
        }        
            
        [TestMethod]
        public void Copy_ReturnsExpectedCopy_IfMessageContainsMemberOfOwnType()
        {
            var child = new MessageWithMemberOfOwnType(GenerateIntValue(), null);
            var message = new MessageWithMemberOfOwnType(GenerateIntValue() + 1, child);
            var copy = (MessageWithMemberOfOwnType) message.Copy();

            Assert.IsNotNull(copy);
            Assert.AreNotSame(message, copy);
            Assert.AreEqual(message.Value, copy.Value);
            Assert.IsNotNull(copy.Child);
            Assert.AreNotSame(child, copy.Child);
            Assert.AreEqual(child.Value, copy.Child.Value);
        }

        #endregion

        #region [====== Validate ======]

        [TestMethod]
        public void Validate_ReturnsNoErrors_IfMessageIsValid()
        {
            var message = new MessageWithPrimitiveMembers(1, Enumerable.Range(0, 10));
            var errorInfo = message.Validate();

            Assert.IsNotNull(errorInfo);
            Assert.IsFalse(errorInfo.HasErrors);
        }

        [TestMethod]
        public void Validate_ReturnsExpectedErrors_IfMessageIsNotValid()
        {
            var message = new MessageWithPrimitiveMembers(0, Enumerable.Empty<int>());
            var errorInfo = message.Validate();

            Assert.IsNotNull(errorInfo);
            Assert.IsTrue(errorInfo.HasErrors);            
            Assert.AreEqual(2, errorInfo.MemberErrors.Count);
            Assert.AreEqual("IntValue (0) must be greater than '0'.", errorInfo.MemberErrors["IntValue"]);
            Assert.AreEqual("IntValues must not be null and contain at least one element.", errorInfo.MemberErrors["IntValues"]);
        }

        #endregion                
    }
}
