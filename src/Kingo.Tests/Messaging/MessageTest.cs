using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Kingo.Messaging.Constraints;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging
{
    [TestClass]
    public sealed partial class MessageTest
    {
        #region [====== SomeMessage ======]

        private sealed class SomeMessage : Message
        {
            public static IReadOnlyList<IMessage> AsReadOnlyList()
            {
                return new SomeMessage();
            }

            public static IMessageStream AsMessageStream()
            {
                return new SomeMessage();
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

            protected override IMessageValidator CreateValidator()
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

        #region [====== IReadOnlyList<IMessage> ======]        

        [TestMethod]
        public void Count_ReturnsOne()
        {
            Assert.AreEqual(1, SomeMessage.AsReadOnlyList().Count);
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void Item_Throws_IfIndexIsNegative()
        {
            SomeMessage.AsReadOnlyList()[-1].IgnoreValue();
        }

        [TestMethod]
        public void Item_ReturnsSelf_IfIndexIsZero()
        {
            var message = SomeMessage.AsReadOnlyList();

            Assert.AreSame(message, message[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void Item_Throws_IfIndexIsGreaterThanZero()
        {
            SomeMessage.AsReadOnlyList()[1].IgnoreValue();
        }

        #endregion

        #region [====== Append(IMessageStream) ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Append_Throws_IfStreamIsNull()
        {
            SomeMessage.AsMessageStream().Append(null);
        }

        [TestMethod]
        public void Append_ReturnsSelf_IfStreamIsEmpty()
        {
            var message = SomeMessage.AsMessageStream();

            Assert.AreSame(message, message.Append(MessageStream.Empty));
        }

        [TestMethod]
        public void Append_ReturnsNewStream_IfStreamIsNotEmpty()
        {
            var messageA = SomeMessage.AsMessageStream();
            var messageB = SomeMessage.AsMessageStream();
            var stream = messageA.Append(messageB);

            Assert.IsNotNull(stream);
            Assert.AreEqual(2, stream.Count);
            Assert.AreSame(messageA, stream[0]);
            Assert.AreSame(messageB, stream[1]);
        }

        #endregion

        #region [====== Accept ======]

        [TestMethod]
        public void Accept_DoesNothing_IfHandlerIsNull()
        {
            SomeMessage.AsMessageStream().Accept(null);
        }

        [TestMethod]
        public void Accept_LetsHandlerVisitTheMessage_IfHandlerIsNotNull()
        {
            var message = SomeMessage.AsMessageStream();
            var handler = new MessageHandlerSpy();

            message.Accept(handler);

            handler.AssertMessageCountIs(1);
            handler.AssertAreSame(message, 0);
        }

        #endregion
    }
}
