using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging
{
    [TestClass]
    public sealed class EmptyStreamTest
    {
        #region [====== SomeMessage ======]

        private sealed class SomeMessage : Message { }

        #endregion

        #region [====== IReadOnlyList<IMessage> ======]

        [TestMethod]
        public void Count_IsZero()
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
        public void Item_Throws_IfIndexIsZero()
        {
            var message = MessageStream.Empty[0];
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void Item_Throws_IfIndexIsOne()
        {
            var message = MessageStream.Empty[1];
        }

        #endregion

        #region [====== Append(IMessageStream) ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Append_Throws_IfStreamIsNull()
        {
            MessageStream.Empty.Append(null);
        }

        [TestMethod]
        public void Append_ReturnsStream_IfStreamIsNotNull()
        {
            var stream = new SomeMessage();

            Assert.AreSame(stream, MessageStream.Empty.Append(stream));
        }

        #endregion        

        #region [====== Accept ======]

        [TestMethod]        
        public void Accept_DoesNothing_IfHandlerIsNull()
        {
            MessageStream.Empty.Accept(null);
        }

        [TestMethod]
        public void Accept_DoesNothing_IfHandlerIsNotNull()
        {
            var handler = new MessageHandlerSpy();

            MessageStream.Empty.Accept(handler);

            handler.AssertMessageCountIs(0);
        }

        #endregion
    }
}
