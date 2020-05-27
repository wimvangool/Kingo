using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices
{
    [TestClass]
    public sealed class MessageHeaderTest
    {
        #region [====== WithId ======]

        [TestMethod]
        public void WithId_ReturnsExpectedHeader_IfIdWasNotYetSpecified()
        {
            var headerA = MessageHeader.Unspecified;
            var headerB = headerA.WithMessageId(new DefaultMessageIdGenerator(), new object());

            Assert.AreEqual(36, headerB.MessageId.Length);
        }

        [TestMethod]
        public void WithId_ReturnsExpectedHeader_IfIdWasAlreadySpecified()
        {
            var headerA = new MessageHeader(string.Empty);
            var headerB = headerA.WithMessageId(new DefaultMessageIdGenerator(), new object());

            Assert.AreEqual(string.Empty, headerB.MessageId);
        }

        #endregion

        #region [====== WithCorrelationId ======]

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void WithCorrelationId_Throws_IfIdWasNotYetSpecified()
        {
            MessageHeader.Unspecified.WithCorrelationId(string.Empty);
        }

        [TestMethod]
        public void WithCorrelationId_ReturnsExpectedHeader_IfIdWasAlreadySpecified()
        {
            var headerA = new MessageHeader(string.Empty);
            var headerB = headerA.WithCorrelationId(string.Empty);

            Assert.AreEqual(string.Empty, headerB.CorrelationId);
        }

        #endregion
    }
}
