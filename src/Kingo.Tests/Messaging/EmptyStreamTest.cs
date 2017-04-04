using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging
{
    [TestClass]
    public sealed class EmptyStreamTest
    {
        #region [====== SomeMessage ======]

        private sealed class SomeMessage { }

        #endregion

        #region [====== IReadOnlyList<object> ======]

        [TestMethod]
        public void Count_IsZero()
        {
            Assert.AreEqual(0, MessageStream.Empty.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void Item_Throws_IfIndexIsNegative()
        {
            MessageStream.Empty[-1].IgnoreValue();
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void Item_Throws_IfIndexIsZero()
        {
            MessageStream.Empty[0].IgnoreValue();
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void Item_Throws_IfIndexIsOne()
        {
            MessageStream.Empty[1].IgnoreValue();
        }

        #endregion

        #region [====== AppendStream(IMessageStream) ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AppendStream_Throws_IfStreamIsNull()
        {
            MessageStream.Empty.AppendStream(null);
        }

        [TestMethod]
        public void AppendStream_ReturnsStream_IfStreamIsNotNull()
        {
            var stream = MessageStream.CreateStream(new SomeMessage());

            Assert.AreSame(stream, MessageStream.Empty.AppendStream(stream));
        }

        #endregion        

        #region [====== HandleMessagesWithAsync ======]

        [TestMethod]        
        public void HandleMessagesWithAsync_DoesNothing_IfHandlerIsNull()
        {
            MessageStream.Empty.HandleMessagesWithAsync(null);
        }

        [TestMethod]
        public async Task HandleMessagesWithAsync_DoesNothing_IfHandlerIsNotNull()
        {
            var handler = new MessageHandlerSpy();

            await MessageStream.Empty.HandleMessagesWithAsync(handler);            

            handler.AssertMessageCountIs(0);
        }

        #endregion
    }
}
