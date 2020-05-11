using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices
{
    [TestClass]
    public sealed class DefaultIdGeneratorTest
    {
        private readonly DefaultMessageIdGenerator _generator;

        public DefaultIdGeneratorTest()
        {
            _generator = new DefaultMessageIdGenerator();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GenerateMessageId_Throws_IfContentIsNull()
        {
            _generator.GenerateMessageId(null);
        }

        [TestMethod]
        public void GenerateMessageId_GeneratesNonEmptyGuid_IfContentIsNotNull()
        {
            Assert.AreNotEqual(Guid.Empty, _generator.GenerateMessageId(new object()));
        }

        [TestMethod]
        public void GenerateMessageId_GeneratesSimilarIds_IfContentIsSameInstance()
        {
            var content = new object();

            var messageId1 = _generator.GenerateMessageId(content);
            var messageId2 = _generator.GenerateMessageId(content);

            Assert.AreNotEqual(messageId1, messageId2);
            Assert.AreEqual(FirstPartOf(messageId1), FirstPartOf(messageId2));
        }

        [TestMethod]
        public void GenerateMessageId_GeneratesDifferentIds_IfContentIsNotSameInstance()
        {
            var messageId1 = _generator.GenerateMessageId(new object());
            var messageId2 = _generator.GenerateMessageId(new object());

            Assert.AreNotEqual(messageId1, messageId2);
            Assert.AreNotEqual(FirstPartOf(messageId1), FirstPartOf(messageId2));
        }

        private static string FirstPartOf(string messageId) =>
            messageId.Substring(0, 18);
    }
}
