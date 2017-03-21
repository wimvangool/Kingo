using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging
{
    [TestClass]
    public sealed class MessageStreamTest
    {
        #region [====== Messages ======]

        private sealed class SomeMessage
        {
            private static int _InstanceCount = 0;
            private readonly int _instance;

            public SomeMessage()
            {
                _instance = Interlocked.Increment(ref _InstanceCount);
            }

            public override string ToString()
            {
                return _instance.ToString();
            }
        }

        #endregion        

        #region [====== AppendStream(IMessageStream) ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AppendStream_Throws_IfMessageIsNull()
        {
            CreateStream().AppendStream(null);
        }        

        [TestMethod]
        public void AppendStream_ReturnsSelf_ISpecifiedStreamIsEmpty()
        {
            var stream = CreateStream();

            Assert.AreSame(stream, stream.AppendStream(MessageStream.Empty));
        }        

        [TestMethod]
        public void AppendStream_ReturnsNewStream_IfSpecifiedStreamIsNotEmpty()
        {
            var streamA = CreateStream();
            var streamB = CreateStream(3);
            var streamC = streamA.AppendStream(streamB);

            Assert.IsNotNull(streamC);
            Assert.AreEqual(5, streamC.Count);
            Assert.AreSame(streamA[0], streamC[0]);
            Assert.AreSame(streamA[1], streamC[1]);
            Assert.AreSame(streamB[0], streamC[2]);
            Assert.AreSame(streamB[1], streamC[3]);
            Assert.AreSame(streamB[2], streamC[4]);
        }

        #endregion

        #region [====== HandleMessagesWithAsync ======]

        [TestMethod]
        public void HandleMessagesWithAsync_DoesNothing_IfHandlerIsNull()
        {
            CreateStream().HandleMessagesWithAsync(null);
        }

        [TestMethod]
        public async Task HandleMessagesWithAsync_LetsHandlerVisitAllMessages_IfHandleIsNotNull()
        {
            var stream = CreateStream();
            var handler = new MessageHandlerSpy();

            await stream.HandleMessagesWithAsync(handler);            

            handler.AssertMessageCountIs(2);
            handler.AssertVisitedAll(stream);
        }

        #endregion

        #region [====== Concat ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Concat_Throws_IfStreamsIsNull()
        {
            MessageStream.Concat(null);
        }

        [TestMethod]
        public void Concat_ReturnsEmptyStream_IfStreamsContainsOnlyNullElements()
        {
            var stream = MessageStream.Concat(new IMessageStream[2]);

            Assert.IsNotNull(stream);
            Assert.AreEqual(0, stream.Count);
        }

        [TestMethod]
        public void Concat_ReturnsSpecifiedStream_IfStreamsContainsOnlyOneStream()
        {
            var stream = MessageStream.Concat(new[] { CreateStream() });

            Assert.IsNotNull(stream);
            Assert.AreEqual(2, stream.Count);
        }

        [TestMethod]
        public void Concat_ReturnsSpecifiedStream_IfStreamsContainsMultipleStreams()
        {
            var streamA = CreateStream();
            var streamB = CreateStream();
            var streamC = CreateStream();
            var streamD = MessageStream.Concat(new[]
            {
                streamA,
                null,
                streamB,
                null,
                streamC
            });

            Assert.IsNotNull(streamD);
            Assert.AreEqual(6, streamD.Count);

            Assert.AreSame(streamA[0], streamD[0]);
            Assert.AreSame(streamA[1], streamD[1]);

            Assert.AreSame(streamB[0], streamD[2]);
            Assert.AreSame(streamB[1], streamD[3]);

            Assert.AreSame(streamC[0], streamD[4]);
            Assert.AreSame(streamC[1], streamD[5]);
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
