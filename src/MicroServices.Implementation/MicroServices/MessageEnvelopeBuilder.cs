using System;
using System.Collections.Generic;
using System.Text;

namespace Kingo.MicroServices
{
    internal sealed class MessageEnvelopeBuilder : IMessageEnvelopeBuilder
    {
        private readonly IMessageIdFactory _messageIdFactory;

        public MessageEnvelopeBuilder(IMessageIdFactory messageIdFactory)
        {
            _messageIdFactory = messageIdFactory;
        }

        public string MessageId
        {
            get;
            set;
        }

        
        public string CorrelationId
        {
            get;
            set;
        }

        public MessageEnvelope Wrap(object message) =>
            new MessageEnvelope(message, DetermineMessageIdFor(message), CorrelationId);

        public MessageEnvelope<TMessage> Wrap<TMessage>(TMessage message) =>
            new MessageEnvelope<TMessage>(message, DetermineMessageIdFor(message), CorrelationId);

        private string DetermineMessageIdFor(object message) =>
            MessageId ?? _messageIdFactory.GenerateMessageIdFor(message);
    }
}
