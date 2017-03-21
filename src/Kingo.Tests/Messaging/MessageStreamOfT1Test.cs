using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging
{
    [TestClass]
    public sealed class MessageStreamOfT1Test
    {
        #region [====== Messages ======]

        private sealed class SomeMessage { }

        #endregion

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_Throws_IfMessageIsNull()
        {
            CreateStream(null, null);
        }

        #region [====== IReadOnlyList<object> ======]        

        [TestMethod]
        public void Count_ReturnsOne()
        {
            Assert.AreEqual(1, CreateStream().Count);
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void Item_Throws_IfIndexIsNegative()
        {
            CreateStream()[-1].IgnoreValue();
        }

        [TestMethod]
        public void Item_ReturnsMessage_IfIndexIsZero()
        {
            var message = new SomeMessage();
            var stream = CreateStream(message, null);

            Assert.AreSame(message, stream[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void Item_Throws_IfIndexIsGreaterThanZero()
        {
            CreateStream()[1].IgnoreValue();
        }

        #endregion

        #region [====== AppendStream(IMessageStream) ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AppendStream_Throws_IfStreamIsNull()
        {
            CreateStream().AppendStream(null);
        }

        [TestMethod]
        public void AppendStream_ReturnsSelf_IfStreamIsEmpty()
        {
            var message = CreateStream();

            Assert.AreSame(message, message.AppendStream(MessageStream.Empty));
        }

        [TestMethod]
        public void AppendStream_ReturnsNewStream_IfStreamIsNotEmpty()
        {            
            var streamA = CreateStream();
            var streamB = CreateStream();
            var stream = streamA.AppendStream(streamB);

            Assert.IsNotNull(stream);
            Assert.AreEqual(2, stream.Count);
            Assert.AreSame(streamA[0], stream[0]);
            Assert.AreSame(streamB[0], stream[1]);
        }

        #endregion

        #region [====== HandleMessagesWithAsync ======]

        [TestMethod]
        public async Task HandleMessagesWithAsync_DoesNothing_IfHandlerIsNull()
        {
            await CreateStream().HandleMessagesWithAsync(null);            
        }

        [TestMethod]
        public async Task HandleMessagesWithAsync_LetsHandlerVisitTheMessage_IfHandlerIsNotNull()
        {
            var message = new SomeMessage();
            var stream = CreateStream(message, null);
            var handler = new MessageHandlerSpy();

            await stream.HandleMessagesWithAsync(handler);            

            handler.AssertMessageCountIs(1);
            handler.AssertAreSame(message, 0);
        }

        #endregion

        private static IMessageStream CreateStream()
        {
            return CreateStream(new SomeMessage(), null);
        }

        private static IMessageStream CreateStream(SomeMessage message, IMessageHandler<SomeMessage> handler)
        {
            return new MessageStream<SomeMessage>(message, handler);
        }
    }
}
