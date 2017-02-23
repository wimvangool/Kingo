using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging
{
    [TestClass]
    public sealed class MessageStreamTest
    {
        #region [====== Messages ======]

        private sealed class SomeMessage : Message { }

        #endregion        

        #region [====== Append(IMessageStream) ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Append_Throws_IfMessageIsNull()
        {
            CreateStream().Append(null);
        }        

        [TestMethod]
        public void Append_ReturnsSelf_ISpecifiedStreamIsEmpty()
        {
            var stream = CreateStream();

            Assert.AreSame(stream, stream.Append(MessageStream.Empty));
        }

        [TestMethod]
        public void Append_ReturnsNewStream_IfSpecifiedStreamIsMessage()
        {
            var message = new SomeMessage();
            var streamA = CreateStream();
            var streamB = streamA.Append(message);

            Assert.IsNotNull(streamB);
            Assert.AreEqual(3, streamB.Count);
            Assert.AreSame(streamA[0], streamB[0]);
            Assert.AreSame(streamA[1], streamB[1]);
            Assert.AreSame(message, streamB[2]);
        }

        [TestMethod]
        public void Append_ReturnsNewStream_IfSpecifiedStreamIsOtherStream()
        {
            var streamA = CreateStream();
            var streamB = CreateStream(3);
            var streamC = streamA.Append(streamB);

            Assert.IsNotNull(streamC);
            Assert.AreEqual(5, streamC.Count);
            Assert.AreSame(streamA[0], streamC[0]);
            Assert.AreSame(streamA[1], streamC[1]);
            Assert.AreSame(streamB[0], streamC[2]);
            Assert.AreSame(streamB[1], streamC[3]);
            Assert.AreSame(streamB[2], streamC[4]);
        }

        #endregion

        #region [====== Accept ======]

        [TestMethod]
        public void Accept_DoesNothing_IfHandlerIsNull()
        {
            CreateStream().Accept(null);
        }

        [TestMethod]
        public void Accept_LetsHandlerVisitAllMessages_IfHandleIsNotNull()
        {
            var stream = CreateStream();
            var handler = new MessageHandlerSpy();

            stream.Accept(handler);

            handler.AssertMessageCountIs(2);
            handler.AssertVisitedAll(stream);
        }

        #endregion

        private static IMessageStream CreateStream(int count = 2)
        {
            var stream = MessageStream.Empty;

            for (int index = 0; index < count; index++)
            {
                stream = stream.Append(new SomeMessage());
            }
            return stream;
        }
    }
}
