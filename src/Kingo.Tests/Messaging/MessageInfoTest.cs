using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging
{
    [TestClass]
    public sealed class MessageInfoTest
    {
        #region [====== Creation ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FromInputStream_Throws_IfMessageIsNull()
        {
            MessageInfo.FromInputStream(null);
        }

        [TestMethod]
        public void FromInputStream_ReturnsExpectedInstance_IsMessageIsNotNull()
        {
            var message = new object();
            var messageInfo = MessageInfo.FromInputStream(message);

            Assert.IsNotNull(messageInfo);
            Assert.AreSame(message, messageInfo.Message);
            Assert.AreEqual(MessageSources.InputStream, messageInfo.Source);
            Assert.AreEqual("System.Object (InputStream)", messageInfo.ToString());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FromOutputStream_Throws_IfMessageIsNull()
        {
            MessageInfo.FromOutputStream(null);
        }

        [TestMethod]
        public void FromOutputStream_ReturnsExpectedInstance_IsMessageIsNotNull()
        {
            var message = new object();
            var messageInfo = MessageInfo.FromOutputStream(message);

            Assert.IsNotNull(messageInfo);
            Assert.AreSame(message, messageInfo.Message);
            Assert.AreEqual(MessageSources.OutputStream, messageInfo.Source);
            Assert.AreEqual("System.Object (OutputStream)", messageInfo.ToString());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FromMetadataStream_Throws_IfMessageIsNull()
        {
            MessageInfo.FromMetadataStream(null);
        }

        [TestMethod]
        public void FromMetadataStream_ReturnsExpectedInstance_IsMessageIsNotNull()
        {
            var message = new object();
            var messageInfo = MessageInfo.FromMetadataStream(message);

            Assert.IsNotNull(messageInfo);
            Assert.AreSame(message, messageInfo.Message);
            Assert.AreEqual(MessageSources.MetadataStream, messageInfo.Source);
            Assert.AreEqual("System.Object (MetadataStream)", messageInfo.ToString());
        }

        #endregion

        #region [====== IsInstanceOfType ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void IsInstanceOfType_Throws_IfTypeIsNull()
        {
            var messageInfo = CreateMessageInfo(new object());

            messageInfo.IsInstanceOf(null);
        }

        [TestMethod]        
        public void IsInstanceOfType_ReturnsFalse_IfMessageIsNotOfTheSpecifiedType()
        {
            var messageInfo = CreateMessageInfo(new object());

            Assert.IsFalse(messageInfo.IsInstanceOf(typeof(string)));
        }

        [TestMethod]
        public void IsInstanceOfType_ReturnsTrue_IfMessageIsNotOfTheSpecifiedType()
        {
            var messageInfo = CreateMessageInfo(string.Empty);

            Assert.IsTrue(messageInfo.IsInstanceOf(typeof(string)));
        }

        #endregion

        #region [====== IsInstanceOf<T> ======]        

        [TestMethod]
        public void IsInstanceOf_ReturnsFalse_IfMessageIsNotOfTheSpecifiedType()
        {
            var messageInfo = CreateMessageInfo(new object());

            Assert.IsFalse(messageInfo.IsInstanceOf<string>());
        }

        [TestMethod]
        public void IsInstanceOf_ReturnsTrue_IfMessageIsNotOfTheSpecifiedType()
        {
            var messageInfo = CreateMessageInfo(string.Empty);

            Assert.IsTrue(messageInfo.IsInstanceOf<string>());
        }

        #endregion

        #region [====== IsInstanceOf<T>(out T) ======]        

        [TestMethod]
        public void IsInstanceOf_Out_ReturnsFalse_IfMessageIsNotOfTheSpecifiedType()
        {
            var messageInfo = CreateMessageInfo(new object());
            string message;

            Assert.IsFalse(messageInfo.IsInstanceOf(out message));
            Assert.IsNull(message);
        }

        [TestMethod]
        public void IsInstanceOf_Out_ReturnsTrue_IfMessageIsNotOfTheSpecifiedType()
        {
            var messageInfo = CreateMessageInfo(string.Empty);
            string message;

            Assert.IsTrue(messageInfo.IsInstanceOf(out message));
            Assert.AreEqual(string.Empty, message);
        }

        #endregion

        private static MessageInfo CreateMessageInfo(object message) =>
            MessageInfo.FromInputStream(message);
    }
}
