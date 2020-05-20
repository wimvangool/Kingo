using System;
using System.Linq;
using Kingo.MicroServices.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices.TestEngine
{
    [TestClass]
    public sealed class MessageStreamTest
    {
        #region [====== Constructor ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_Throws_IfMessageCollectionIsNull()
        {
            new MessageStream(null);
        }

        #endregion

        #region [====== ToString ======]

        [TestMethod]
        public void ToString_ReturnsExpectedValue_IfStreamIsEmpty()
        {
            Assert.AreEqual("[0]", CreateEventStream().ToString());
        }

        [TestMethod]
        public void ToString_ReturnsExpectedValue_IfStreamContainsOneMessage()
        {
            Assert.AreEqual("[1] { Object }", CreateEventStream(new object()).ToString());
        }

        [TestMethod]
        public void ToString_ReturnsExpectedValue_IfStreamContainsThreeMessages()
        {
            Assert.AreEqual("[3] { Object, Int32, String }", CreateEventStream(new object(), 10, "Bla").ToString());
        }

        [TestMethod]
        public void ToString_ReturnsExpectedValue_IfStreamContainFourMessages()
        {
            Assert.AreEqual("[4] { Object, Int32, String, ... }", CreateEventStream(new object(), 10, "Bla", 'v').ToString());
        }

        [TestMethod]
        public void ToString_ReturnsExpectedValue_IfStreamContainFiveMessages()
        {
            Assert.AreEqual("[5] { Object, Int32, String, ... }", CreateEventStream(new object(), 10, "Bla", 'v', 55).ToString());
        }

        #endregion

        #region [====== AssertMessage ======]

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void AssertMessage_Throws_IfIndexIsNegative()
        {
            CreateEventStream().AssertMessage<object>(message => { }, -1);
        }

        [TestMethod]
        [ExpectedException(typeof(TestFailedException))]
        public void AssertMessage_Throws_IfStreamContainsNoMessages()
        {
            CreateEventStream().AssertMessage<object>(message => { });
        }

        [TestMethod]
        [ExpectedException(typeof(TestFailedException))]
        public void AssertMessage_Throws_IfStreamContainsNoMessagesOfTheSpecifiedType()
        {
            CreateEventStream(string.Empty).AssertMessage<int>(message => { });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AssertMessage_Throws_IfAssertionIsNull()
        {
            CreateEventStream(string.Empty).AssertMessage<object>(null);
        }

        [TestMethod]
        public void AssertMessage_AssertsExpectedMessage_IfStreamContainsExactlyOneMessageOfTheSpecifiedType()
        {
            var messageA = string.Empty;
            
            CreateEventStream(messageA).AssertMessage<object>(messageB =>
            {
                Assert.AreSame(messageA, messageB.Content);
            });
        }

        #endregion

        #region [====== GetMessage ======]

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void GetMessage_Throws_IfIndexIsNegative()
        {
            CreateEventStream().GetMessage<object>(-1);
        }

        [TestMethod]
        [ExpectedException(typeof(TestFailedException))]
        public void GetMessage_Throws_IfStreamContainsNoMessages()
        {
            CreateEventStream().GetMessage<object>();
        }

        [TestMethod]
        [ExpectedException(typeof(TestFailedException))]
        public void GetMessage_Throws_IfStreamContainsNoMessagesOfTheSpecifiedType()
        {
            CreateEventStream(string.Empty).GetMessage<int>();
        }

        [TestMethod]
        public void GetMessage_ReturnsExpectedMessage_IfStreamContainsExactlyOneMessageOfTheSpecifiedType()
        {
            var messageA = string.Empty;
            var messageB = CreateEventStream(messageA).GetMessage<object>();

            Assert.AreSame(messageA, messageB.Content);
        }

        [TestMethod]
        public void GetMessage_ReturnsExpectedMessage_IfStreamContainsTwoMessagesOfTheSpecifiedType_And_FirstOneIsSelected()
        {
            var messageA = string.Empty;
            var messageB = 4;
            var messageC = CreateEventStream(messageA, messageB).GetMessage<object>();

            Assert.AreSame(messageA, messageC.Content);
        }

        [TestMethod]
        public void GetMessage_ReturnsExpectedMessage_IfStreamContainsTwoMessagesOfTheSpecifiedType_And_SecondOneIsSelected()
        {
            var messageA = string.Empty;
            var messageB = 4;
            var messageC = CreateEventStream(messageA, messageB).GetMessage<object>(1);

            Assert.AreEqual(messageB, messageC.Content);
        }

        #endregion

        #region [====== TryGetMessage ======]

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void TryGetMessage_Throws_IfIndexIsNegative()
        {
            CreateEventStream().TryGetMessage<object>(-1, out _);
        }

        [TestMethod]
        public void TryGetMessage_ReturnsFalse_IfStreamContainsNoMessages()
        {
            Assert.IsFalse(CreateEventStream().TryGetMessage<object>(0, out var message));
            Assert.IsNull(message);
        }

        [TestMethod]
        public void TryGetMessage_ReturnsFalse_IfStreamContainsNoMessagesOfTheSpecifiedType()
        {
            Assert.IsFalse(CreateEventStream(string.Empty).TryGetMessage<int>(0, out var message));
            Assert.IsNull(message);
        }

        [TestMethod]
        public void TryGetMessage_ReturnsTrue_IfStreamContainsExactlyOneMessageOfTheSpecifiedType()
        {
            var messageA = string.Empty;

            Assert.IsTrue(CreateEventStream(messageA).TryGetMessage<object>(0, out var messageB));
            Assert.AreSame(messageA, messageB.Content);
        }

        [TestMethod]
        public void TryGetMessage_ReturnsTrue_IfStreamContainsTwoMessagesOfTheSpecifiedType_And_FirstOneIsSelected()
        {
            var messageA = string.Empty;
            var messageB = 4;
            
            Assert.IsTrue(CreateEventStream(messageA, messageB).TryGetMessage<object>(0, out var messageC));
            Assert.AreSame(messageA, messageC.Content);
        }

        [TestMethod]
        public void TryGetMessage_ReturnsTrue_IfStreamContainsTwoMessagesOfTheSpecifiedType_And_SecondOneIsSelected()
        {
            var messageA = string.Empty;
            var messageB = 4;

            Assert.IsTrue(CreateEventStream(messageA, messageB).TryGetMessage<object>(1, out var messageC));
            Assert.AreEqual(messageB, messageC.Content);
        }

        #endregion

        private static readonly MessageFactory _MessageFactory = new MessageCollection().BuildMessageFactory();

        private static MessageStream CreateEventStream(params object[] events) =>
            new MessageStream(events.Select(CreateEvent));

        private static Message<object> CreateEvent(object @event) =>
            _MessageFactory.CreateEvent(MessageDirection.Output, MessageHeader.Unspecified, @event);
    }
}
