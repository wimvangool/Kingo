using System;
using System.Collections.Generic;
using System.Text;

namespace Kingo.MicroServices
{
    internal sealed class MessageFactory : IMessageFactory
    {
        private readonly IMessageIdFactory _messageIdFactory;

        public MessageFactory(IMessageIdFactory messageIdFactory)
        {
            _messageIdFactory = messageIdFactory;
        }

        public MessageBuilder ToBuilder() =>
            new MessageBuilder(_messageIdFactory);

        public Message CreateMessage(object content) =>
            new Message(content, _messageIdFactory.GenerateMessageIdFor(content));

        public Message<TMessage> CreateMessage<TMessage>(TMessage content) =>
            new Message<TMessage>(content, _messageIdFactory.GenerateMessageIdFor(content));
    }
}
