using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Kingo.Messaging.Validation.Constraints;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging.Validation
{
    [TestClass]
    public sealed partial class MessageTest
    {
        #region [====== SomeMessage ======]

        private sealed class SomeMessage : Message { }

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

            protected override ValidateMethod Implement(ValidateMethod method) =>
                base.Implement(method).Add(this, CreateValidator, true);

            private static IMessageValidator<MessageWithPrimitiveMembers> CreateValidator()
            {
                var validator = new ConstraintMessageValidator<MessageWithPrimitiveMembers>();

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
