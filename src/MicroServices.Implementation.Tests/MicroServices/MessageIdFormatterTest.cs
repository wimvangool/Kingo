using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices
{
    [TestClass]
    public sealed class MessageIdFormatterTest
    {
        #region [====== Messages ======]

        private class SomeMessage
        {
            public SomeMessage(string id, string name)
            {
                Id = id;
                Name = name;
            }

            public string Id
            {
                get;
                set;
            }

            public string Name
            {
                get;
                set;
            }
        }

        private class SomeOtherMessage : SomeMessage
        {
            public SomeOtherMessage(string id, string name) :
                base(id, name) { }

            public new int Id
            {
                get => int.Parse(base.Id);
                set => base.Id = value.ToString();
            }
        }

        #endregion

        #region [====== FromContentType ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void FromContentType_Throws_IfSpecifiedPropertyIsNotFound()
        {
            FromContentType(typeof(object), string.Empty, nameof(SomeMessage.Id));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void FromContentType_Throws_IfSpecifiedPropertyIsAmbiguous()
        {
            FromContentType(typeof(SomeOtherMessage), string.Empty, nameof(SomeMessage.Id));
        }

        #endregion

        #region [====== FormatMessageId ======]

        [TestMethod]
        public void FromContentType_ReturnsExpectedFormatter_IfNoPropertiesAreSpecified()
        {
            var content = new object();
            var messageIdFormat = Guid.NewGuid().ToString();
            var formatter = FromContent(content, messageIdFormat);

            Assert.AreEqual(messageIdFormat, formatter.FormatMessageId(content));
        }

        [TestMethod]
        public void FormatMessageId_ReturnsExpectedMessageId_IfSpecifiedPropertiesAreFound()
        {
            var content = new SomeMessage(nameof(SomeMessage.Id), nameof(SomeMessage.Name));
            var formatter = FromContent(content, "{0}-{1}", nameof(SomeMessage.Id), nameof(SomeMessage.Name));

            Assert.AreEqual("Id-Name", formatter.FormatMessageId(content));
        }

        #endregion

        private static MessageIdFormatter FromContent(object content, string messageIdFormat, params string[] messageIdProperties) =>
            FromContentType(content.GetType(), messageIdFormat, messageIdProperties);

        private static MessageIdFormatter FromContentType(Type contentType, string messageIdFormat, params string[] messageIdProperties) =>
            MessageIdFormatter.FromContentType(contentType, messageIdFormat, messageIdProperties);
    }
}
