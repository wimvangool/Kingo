using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging
{
    [TestClass]
    public sealed class MessageStreamTest
    {
        #region [====== Messages ======]

        private sealed class SomeMessage : Message { }

        #endregion

        #region [====== IReadOnlyList ======]

        [TestMethod]
        public void Count_IsZero_IsStreamIsEmpty()
        {
            Assert.AreEqual(0, MessageStream.Empty.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void Item_Throws_IfIndexIsNegative()
        {
            var message = MessageStream.Empty[-1];
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void Item_Throws_IfIndexIsZero_And_StreamIsEmpty()
        {
            var message = MessageStream.Empty[0];
        }

        #endregion

        #region [====== FromMessages(IEnumerable<IMessage>) ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FromMessages_Throws_IfCollectionIsNull()
        {
            MessageStream.FromMessages(null);
        }

        [TestMethod]
        public void FromMessages_ReturnsEmptyStream_IfCollectionIsEmpty()
        {
            var stream = MessageStream.FromMessages(Enumerable.Empty<IMessage>());

            Assert.IsNotNull(stream);
            Assert.AreEqual(0, stream.Count);
        }

        [TestMethod]
        public void FromMessages_ReturnsEmptyCollection_IfCollectionContainsOnlyNullElements()
        {
            var messages = new IMessage[] { null, null };
            var stream = MessageStream.FromMessages(messages);

            Assert.IsNotNull(stream);
            Assert.AreEqual(0, stream.Count);
        }

        [TestMethod]
        public void FromMessages_ReturnsExpectedException_IfCollectionContainsMessages()
        {
            var messages = new IMessage[] { new SomeMessage(), null, new EmptyMessage() };
            var stream = MessageStream.FromMessages(messages);

            Assert.IsNotNull(stream);
            Assert.AreEqual(2, stream.Count);
            Assert.AreSame(messages[0], stream[0]);
            Assert.AreSame(messages[2], stream[1]);
        }

        #endregion

        #region [====== FromMessage(IMessage) ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FromMessage_Throws_IfMessageIsNull()
        {
            MessageStream.FromMessage(null);
        }

        [TestMethod]
        public void FromMessage_ReturnsStreamWithOneElement_IfMessageIsNotNull()
        {
            IMessage message = new SomeMessage();
            var stream = MessageStream.FromMessage(message);

            Assert.IsNotNull(stream);
            Assert.AreEqual(1, stream.Count);
            Assert.AreSame(message, stream[0]);
        }

        [TestMethod]
        public void ImplicitConversionOperator_ReturnsNull_IfMessageIsNull()
        {
            Message message = null;
            MessageStream stream = message;

            Assert.IsNull(stream);
        }

        [TestMethod]
        public void ImplicitConversionOperator_ReturnsNewStream_IfMessageIsNotNull()
        {
            Message message = new SomeMessage();
            MessageStream stream = message;

            Assert.IsNotNull(stream);
            Assert.AreEqual(1, stream.Count);
            Assert.AreSame(message, stream[0]);
        }

        #endregion

        #region [====== Append(IMessage) ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AppendIMessage_Throws_IfMessageIsNull()
        {
            MessageStream.Empty.Append(null as IMessage);
        }

        [TestMethod]
        public void AppendIMessage_ReturnsNewStreamWithAppendedMessage_IfOldStreamIsEmpty()
        {
            IMessage message = new SomeMessage();
            var stream = MessageStream.Empty.Append(message);

            Assert.IsNotNull(stream);
            Assert.AreNotSame(MessageStream.Empty, stream);
            Assert.AreEqual(1, stream.Count);
            Assert.AreSame(message, stream[0]);
        }

        [TestMethod]
        public void AppendIMessage_ReturnsNewStreamWithAppendedMessage_IfOldStreamIsNotEmpty()
        {
            IMessage messageA = new SomeMessage();
            IMessage messageB = new SomeMessage();
            var streamA = MessageStream.Empty.Append(messageA);
            var streamB = streamA.Append(messageB);

            Assert.IsNotNull(streamB);
            Assert.AreNotSame(streamA, streamB);
            Assert.AreEqual(2, streamB.Count);
            Assert.AreSame(messageA, streamB[0]);
            Assert.AreSame(messageB, streamB[1]);
        }

        #endregion

        #region [====== Append<TMessage> ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AppendMessage_Throws_IfMessageIsNull()
        {
            MessageStream.Empty.Append(null as SomeMessage);
        }

        [TestMethod]
        public void AppendMessage_ReturnsNewStreamWithAppendedMessage_IfOldStreamIsEmpty()
        {
            var message = new SomeMessage();
            var stream = MessageStream.Empty.Append(message);

            Assert.IsNotNull(stream);
            Assert.AreNotSame(MessageStream.Empty, stream);
            Assert.AreEqual(1, stream.Count);
            Assert.AreSame(message, stream[0]);
        }        

        [TestMethod]
        public void AppendMessage_ReturnsNewStreamWithAppendedMessage_IfOldStreamIsNotEmpty()
        {
            var messageA = new SomeMessage();
            var messageB = new SomeMessage();
            var streamA = MessageStream.Empty.Append(messageA);
            var streamB = streamA.Append(messageB);

            Assert.IsNotNull(streamB);
            Assert.AreNotSame(streamA, streamB);
            Assert.AreEqual(2, streamB.Count);
            Assert.AreSame(messageA, streamB[0]);
            Assert.AreSame(messageB, streamB[1]);
        }

        #endregion

        #region [====== AppendStream ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AppendStream_Throws_IfStreamIsNull()
        {
            MessageStream.Empty.AppendStream(null as MessageStream);
        }

        [TestMethod]
        public void AppendStream_ReturnsEmptyStream_IfBothLeftAndRightStreamAreEmpty()
        {
            var stream = MessageStream.Empty.AppendStream(MessageStream.Empty);

            Assert.IsNotNull(stream);
            Assert.AreEqual(0, stream.Count);
        }

        [TestMethod]
        public void AppendStream_ReturnsExpectedStream_IfLeftIsEmpty_And_RightIsNotEmpty()
        {
            var message = new SomeMessage();
            var leftStream = MessageStream.Empty;
            var rightStream = MessageStream.FromMessage(message);
            var stream = leftStream.AppendStream(rightStream);

            Assert.IsNotNull(stream);
            Assert.AreEqual(1, stream.Count);
            Assert.AreSame(message, stream[0]);
        }

        [TestMethod]
        public void AppendStream_ReturnsExpectedStream_IfLeftIsNotEmpty_And_RightIsEmpty()
        {
            var message = new SomeMessage();
            var leftStream = MessageStream.FromMessage(message);
            var rightStream = MessageStream.Empty;
            var stream = leftStream.AppendStream(rightStream);

            Assert.IsNotNull(stream);
            Assert.AreEqual(1, stream.Count);
            Assert.AreSame(message, stream[0]);
        }

        [TestMethod]
        public void AppendStream_ReturnsExpectedStream_IfLeftIsNotEmpty_And_RightIsNotEmpty()
        {            
            MessageStream leftStream = new SomeMessage();
            MessageStream rightStream = new SomeMessage();
            var stream = leftStream.AppendStream(rightStream);

            Assert.IsNotNull(stream);
            Assert.AreEqual(2, stream.Count);
            Assert.AreSame(leftStream[0], stream[0]);
            Assert.AreSame(rightStream[0], stream[1]);
        }

        [TestMethod]
        public void CombineStreams_ReturnsNull_IfBothLeftAndRightAreNull()
        {
            MessageStream leftStream = null;
            MessageStream rightStream = null;
            
            Assert.IsNull(leftStream + rightStream);
        }

        [TestMethod]
        public void CombineStreams_ReturnsLeftStream_IfRightStreamIsNull()
        {
            var leftStream = MessageStream.Empty;
            var stream = leftStream + null;

            Assert.AreSame(leftStream, stream);
        }

        [TestMethod]
        public void CombineStreams_ReturnsRightStream_IfLeftStreamIsNull()
        {
            var rightStream = MessageStream.Empty;
            var stream = null + rightStream;

            Assert.AreSame(rightStream, stream);
        }

        [TestMethod]
        public void CombineStreams_ReturnsEmptyStream_IfBothLeftAndRightStreamAreEmpty()
        {
            var stream = MessageStream.Empty + MessageStream.Empty;

            Assert.IsNotNull(stream);
            Assert.AreEqual(0, stream.Count);
        }

        [TestMethod]
        public void CombineStreams_ReturnsExpectedStream_IfLeftIsEmpty_And_RightIsNotEmpty()
        {
            var message = new SomeMessage();
            var leftStream = MessageStream.Empty;
            var rightStream = MessageStream.FromMessage(message);
            var stream = leftStream + rightStream;

            Assert.IsNotNull(stream);
            Assert.AreEqual(1, stream.Count);
            Assert.AreSame(message, stream[0]);
        }

        [TestMethod]
        public void CombineStreams_ReturnsExpectedStream_IfLeftIsNotEmpty_And_RightIsEmpty()
        {
            var message = new SomeMessage();
            var leftStream = MessageStream.FromMessage(message);
            var rightStream = MessageStream.Empty;
            var stream = leftStream + rightStream;

            Assert.IsNotNull(stream);
            Assert.AreEqual(1, stream.Count);
            Assert.AreSame(message, stream[0]);
        }

        [TestMethod]
        public void CombineStreams_ReturnsExpectedStream_IfLeftIsNotEmpty_And_RightIsNotEmpty()
        {
            MessageStream leftStream = new SomeMessage();
            MessageStream rightStream = new SomeMessage();
            var stream = leftStream + rightStream;

            Assert.IsNotNull(stream);
            Assert.AreEqual(2, stream.Count);
            Assert.AreSame(leftStream[0], stream[0]);
            Assert.AreSame(rightStream[0], stream[1]);
        }

        #endregion

        #region [====== Accept ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Accept_Throws_IfVisitorIsNull()
        {
            MessageStream.Empty.Accept(null);
        }

        #endregion
    }
}
