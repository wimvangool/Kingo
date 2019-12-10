using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices.Controllers
{
    [TestClass]
    public sealed class MessageStreamTest
    {
        #region [====== MessageStreamStub ======]

        private sealed class MessageStreamStub : MessageStream
        {
            public MessageStreamStub(MessageStream stream) :
                base(stream) { }

            public new TMessage GetMessage<TMessage>(int index = 0) =>
                base.GetMessage<TMessage>(index).Content;

            public bool TryGetMessage<TMessage>(int index, out TMessage message)
            {
                if (base.TryGetMessage<TMessage>(index, out var messageToDispatch))
                {
                    message = messageToDispatch.Content;
                    return true;
                }
                message = default;
                return false;
            }
        }

        #endregion

        #region [====== Constructor ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_Throws_IfMessageCollectionIsNull()
        {
            new MessageStream(null);
        }

        #endregion

        #region [====== GetMessage ======]

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void GetMessage_Throws_IfIndexIsNegative()
        {
            CreateEventStreamStub().GetMessage<object>(-1);
        }

        [TestMethod]
        [ExpectedException(typeof(TestFailedException))]
        public void GetMessage_Throws_IfStreamContainsNoMessages()
        {
            CreateEventStreamStub().GetMessage<object>();
        }

        [TestMethod]
        [ExpectedException(typeof(TestFailedException))]
        public void GetMessage_Throws_IfStreamContainsNoMessagesOfTheSpecifiedType()
        {
            CreateEventStreamStub(string.Empty).GetMessage<int>();
        }

        [TestMethod]
        public void GetMessage_ReturnsExpectedMessage_IfStreamContainsExactlyOneMessageOfTheSpecifiedType()
        {
            var messageA = string.Empty;
            var messageB = CreateEventStreamStub(messageA).GetMessage<object>();

            Assert.AreSame(messageA, messageB);
        }

        [TestMethod]
        public void GetMessage_ReturnsExpectedMessage_IfStreamContainsTwoMessagesOfTheSpecifiedType_And_FirstOneIsSelected()
        {
            var messageA = string.Empty;
            var messageB = 4;
            var messageC = CreateEventStreamStub(messageA, messageB).GetMessage<object>();

            Assert.AreSame(messageA, messageC);
        }

        [TestMethod]
        public void GetMessage_ReturnsExpectedMessage_IfStreamContainsTwoMessagesOfTheSpecifiedType_And_SecondOneIsSelected()
        {
            var messageA = string.Empty;
            var messageB = 4;
            var messageC = CreateEventStreamStub(messageA, messageB).GetMessage<object>(1);

            Assert.AreEqual(messageB, messageC);
        }

        #endregion

        #region [====== TryGetMessage ======]

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void TryGetMessage_Throws_IfIndexIsNegative()
        {
            CreateEventStreamStub().TryGetMessage<object>(-1, out _);
        }

        [TestMethod]
        public void TryGetMessage_ReturnsFalse_IfStreamContainsNoMessages()
        {
            Assert.IsFalse(CreateEventStreamStub().TryGetMessage<object>(0, out var message));
            Assert.IsNull(message);
        }

        [TestMethod]
        public void TryGetMessage_ReturnsFalse_IfStreamContainsNoMessagesOfTheSpecifiedType()
        {
            Assert.IsFalse(CreateEventStreamStub(string.Empty).TryGetMessage<int>(0, out var message));
            Assert.AreEqual(0, message);
        }

        [TestMethod]
        public void TryGetMessage_ReturnsTrue_IfStreamContainsExactlyOneMessageOfTheSpecifiedType()
        {
            var messageA = string.Empty;

            Assert.IsTrue(CreateEventStreamStub(messageA).TryGetMessage<object>(0, out var messageB));
            Assert.AreSame(messageA, messageB);
        }

        [TestMethod]
        public void TryGetMessage_ReturnsTrue_IfStreamContainsTwoMessagesOfTheSpecifiedType_And_FirstOneIsSelected()
        {
            var messageA = string.Empty;
            var messageB = 4;
            
            Assert.IsTrue(CreateEventStreamStub(messageA, messageB).TryGetMessage<object>(0, out var messageC));
            Assert.AreSame(messageA, messageC);
        }

        [TestMethod]
        public void TryGetMessage_ReturnsTrue_IfStreamContainsTwoMessagesOfTheSpecifiedType_And_SecondOneIsSelected()
        {
            var messageA = string.Empty;
            var messageB = 4;

            Assert.IsTrue(CreateEventStreamStub(messageA, messageB).TryGetMessage<object>(1, out var messageC));
            Assert.AreEqual(messageB, messageC);
        }

        #endregion

        private static MessageStreamStub CreateEventStreamStub(params object[] events) =>
            new MessageStreamStub(CreateEventStream(events));

        private static MessageStream CreateEventStream(params object[] events) =>
            new MessageStream(events.Select(@event => @event.ToEvent()));
    }
}
