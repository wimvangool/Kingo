using System;
using System.Collections.Generic;
using System.Text;

namespace Kingo.MicroServices
{
    internal sealed class MessageEnvelopeFactory : IMessageEnvelopeFactory
    {
        private readonly IMessageIdFactory _messageIdFactory;

        public MessageEnvelopeFactory(IMessageIdFactory messageIdFactory)
        {
            _messageIdFactory = messageIdFactory;
        }

        public MessageEnvelopeBuilder ToBuilder() =>
            new MessageEnvelopeBuilder(_messageIdFactory);

        public MessageEnvelope Wrap(object message) =>
            new MessageEnvelope(message, _messageIdFactory.GenerateMessageIdFor(message));

        public MessageEnvelope<TMessage> Wrap<TMessage>(TMessage message) =>
            new MessageEnvelope<TMessage>(message, _messageIdFactory.GenerateMessageIdFor(message));
    }
}
