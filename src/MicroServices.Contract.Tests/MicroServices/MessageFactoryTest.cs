using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices
{
    [TestClass]
    public sealed class MessageFactoryTest
    {
        #region [====== Messages ======]

        [Message(MessageKind.Command, "[{0}]", nameof(PropertyA))]
        private class MessageA
        {
            public MessageA(int propertyA)
            {
                PropertyA = propertyA;
            }

            public int PropertyA
            {
                get;
            }
        }

        [Message(MessageKind.Event, "[{0}-{1}]", nameof(PropertyA), nameof(PropertyB))]
        private class MessageB : MessageA
        {
            public MessageB(int propertyA, int propertyB) : base(propertyA)
            {
                PropertyB = propertyB;
            }

            public int PropertyB
            {
                get;
            }
        }

        private class MessageC : MessageB
        {
            public MessageC(int propertyA, int propertyB, int propertyC) : base(propertyA, propertyB)
            {
                PropertyC = propertyC;
            }

            public int PropertyC
            {
                get;
            }
        }

        #endregion

        private readonly MessageFactoryBuilder _builder;

        public MessageFactoryTest()
        {
            _builder = new MessageFactoryBuilder();
        }

        #region [====== Builder ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Add_Throws_IfMessageTypeIsNull()
        {
            _builder.Add(null, new MessageAttribute(MessageKind.Command));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Add_Throws_IfMessageAttributeNull()
        {
            _builder.Add(typeof(object), null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Add_Throws_IfMessageAttributeWasAlreadyAddedForSpecifiedType()
        {
            _builder.Add<object>(MessageKind.Command);
            _builder.Add<object>(MessageKind.Event);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetMessageKindResolver_Throws_IfResolverIsNull()
        {
            _builder.MessageKindResolver = null;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetMessageIdGenerator_Throws_IfGeneratorIsNull()
        {
            _builder.MessageIdGenerator = null;
        }

        #endregion

        #region [====== CreateMessage (Basic) ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreateMessage_Throws_IfContentIsNull()
        {
            CreateMessage(null);
        }

        [TestMethod]
        public void CreateMessage_ReturnsExpectedMessage_IfContentTypeDeclaresNoMessageAttribute_And_NoAttributeWasAddedForType()
        {
            var message = CreateMessage(new object());

            Assert.AreSame(typeof(Message<object>), message.GetType());
            Assert.AreEqual(MessageKind.Undefined, message.Kind);
            Assert.AreNotEqual(Guid.Empty, Guid.Parse(message.Id));
            Assert.IsNull(message.CorrelationId);
            Assert.IsNull(message.DeliveryTimeUtc);
        }

        [TestMethod]
        public void CreateMessage_ReturnsExpectedMessage_IfCustomMessageKindResolverWasSpecified()
        {
            _builder.MessageKindResolver = new FixedMessageKindResolver(MessageKind.Command);

            var message = CreateMessage(new object());

            Assert.AreSame(typeof(Message<object>), message.GetType());
            Assert.AreEqual(MessageKind.Command, message.Kind);
            Assert.AreNotEqual(Guid.Empty, Guid.Parse(message.Id));
            Assert.IsNull(message.CorrelationId);
            Assert.IsNull(message.DeliveryTimeUtc);
        }

        [TestMethod]
        public void CreateMessage_ReturnsExpectedMessage_IfCustomMessageIdGeneratorWasSpecified()
        {
            var messageId = Guid.NewGuid().ToString();

            _builder.MessageIdGenerator = new FixedMessageIdGenerator(messageId);

            var message = CreateMessage(new object());

            Assert.AreSame(typeof(Message<object>), message.GetType());
            Assert.AreEqual(MessageKind.Undefined, message.Kind);
            Assert.AreEqual(messageId, message.Id);
            Assert.IsNull(message.CorrelationId);
            Assert.IsNull(message.DeliveryTimeUtc);
        }

        [TestMethod]
        public void CreateMessage_ReturnsExpectedMessage_IfDeliveryTimeWasSpecified()
        {
            var deliveryTime = DateTimeOffset.UtcNow;
            var message = CreateMessage(new object(), deliveryTime);

            Assert.AreSame(typeof(Message<object>), message.GetType());
            Assert.AreEqual(MessageKind.Undefined, message.Kind);
            Assert.AreNotEqual(Guid.Empty, Guid.Parse(message.Id));
            Assert.IsNull(message.CorrelationId);
            Assert.AreEqual(deliveryTime, message.DeliveryTimeUtc);
        }

        #endregion

        #region [====== CreateMessage (MessageAttributes) ======]

        [TestMethod]
        public void CreateMessage_ReturnsExpectedMessage_IfContentTypeDeclaresNoMessageAttribute_But_MessageAttributeWasAddedForType()
        {
            _builder.Add<string>(MessageKind.Command, "Id-{0}", nameof(string.Length));

            var message = CreateMessage(Guid.NewGuid().ToString());

            Assert.AreSame(typeof(Message<string>), message.GetType());
            Assert.AreEqual(MessageKind.Command, message.Kind);
            Assert.AreEqual("Id-36", message.Id);
            Assert.IsNull(message.CorrelationId);
            Assert.IsNull(message.DeliveryTimeUtc);
        }

        [TestMethod]
        public void CreateMessage_ReturnsExpectedMessage_IfContentTypeDeclaresMessageAttribute_And_NoAttributeWasAddedForType()
        {
            var message = CreateMessage(new MessageA(10));

            Assert.AreSame(typeof(Message<MessageA>), message.GetType());
            Assert.AreEqual(MessageKind.Command, message.Kind);
            Assert.AreEqual("[10]", message.Id);
            Assert.IsNull(message.CorrelationId);
            Assert.IsNull(message.DeliveryTimeUtc);
        }

        [TestMethod]
        public void CreateMessage_ReturnsExpectedMessage_IfContentTypeDeclaresMessageAttribute_But_MessageAttributeWasAddedForType()
        {
            _builder.Add<MessageA>(MessageKind.Event, "({0})", nameof(MessageA.PropertyA));

            var message = CreateMessage(new MessageA(10));

            Assert.AreSame(typeof(Message<MessageA>), message.GetType());
            Assert.AreEqual(MessageKind.Event, message.Kind);
            Assert.AreEqual("(10)", message.Id);
            Assert.IsNull(message.CorrelationId);
            Assert.IsNull(message.DeliveryTimeUtc);
        }

        [TestMethod]
        public void CreateMessage_ReturnsExpectedMessage_IfContentTypeDeclaresNewMessageAttribute_And_NoAttributeWasAddedForType()
        {
            var message = CreateMessage(new MessageB(10, 20));

            Assert.AreSame(typeof(Message<MessageB>), message.GetType());
            Assert.AreEqual(MessageKind.Event, message.Kind);
            Assert.AreEqual("[10-20]", message.Id);
            Assert.IsNull(message.CorrelationId);
            Assert.IsNull(message.DeliveryTimeUtc);
        }

        [TestMethod]
        public void CreateMessage_ReturnsExpectedMessage_IfContentTypeDeclaresNewMessageAttribute_But_MessageAttributeWasAddedForType()
        {
            _builder.Add<MessageB>(MessageKind.Request, "({0}-{1})", nameof(MessageA.PropertyA), nameof(MessageB.PropertyB));

            var message = CreateMessage(new MessageB(10, 20));

            Assert.AreSame(typeof(Message<MessageB>), message.GetType());
            Assert.AreEqual(MessageKind.Request, message.Kind);
            Assert.AreEqual("(10-20)", message.Id);
            Assert.IsNull(message.CorrelationId);
            Assert.IsNull(message.DeliveryTimeUtc);
        }

        [TestMethod]
        public void CreateMessage_ReturnsExpectedMessage_IfContentTypeInheritsMessageAttribute_And_NoAttributeWasAddedForType()
        {
            var message = CreateMessage(new MessageC(10, 20, 30));

            Assert.AreSame(typeof(Message<MessageC>), message.GetType());
            Assert.AreEqual(MessageKind.Event, message.Kind);
            Assert.AreEqual("[10-20]", message.Id);
            Assert.IsNull(message.CorrelationId);
            Assert.IsNull(message.DeliveryTimeUtc);
        }

        [TestMethod]
        public void CreateMessage_ReturnsExpectedMessage_IfContentTypeInheritsMessageAttribute_But_MessageAttributeWasAddedForBaseType()
        {
            _builder.Add<MessageB>(MessageKind.Request, "({0}-{1})", nameof(MessageA.PropertyA), nameof(MessageB.PropertyB));

            var message = CreateMessage(new MessageC(10, 20, 30));

            Assert.AreSame(typeof(Message<MessageC>), message.GetType());
            Assert.AreEqual(MessageKind.Request, message.Kind);
            Assert.AreEqual("(10-20)", message.Id);
            Assert.IsNull(message.CorrelationId);
            Assert.IsNull(message.DeliveryTimeUtc);
        }

        [TestMethod]
        public void CreateMessage_ReturnsExpectedMessage_IfContentTypeInheritsMessageAttribute_But_MessageAttributeWasAddedForType()
        {
            _builder.Add<MessageC>(MessageKind.Request, "({0}-{1}-{2})", nameof(MessageA.PropertyA), nameof(MessageB.PropertyB), nameof(MessageC.PropertyC));

            var message = CreateMessage(new MessageC(10, 20, 30));

            Assert.AreSame(typeof(Message<MessageC>), message.GetType());
            Assert.AreEqual(MessageKind.Request, message.Kind);
            Assert.AreEqual("(10-20-30)", message.Id);
            Assert.IsNull(message.CorrelationId);
            Assert.IsNull(message.DeliveryTimeUtc);
        }

        #endregion

        private IMessage CreateMessage(object content, DateTimeOffset? deliveryTime = null) =>
            _builder.BuildMessageFactory().CreateMessage(content, deliveryTime);
    }
}
