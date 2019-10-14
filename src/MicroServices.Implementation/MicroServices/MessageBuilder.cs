using System;
using System.Collections.Generic;
using System.Text;

namespace Kingo.MicroServices
{
    internal sealed class MessageBuilder : IMessageBuilder
    {
        private readonly IMessageIdFactory _messageIdFactory;

        public MessageBuilder(IMessageIdFactory messageIdFactory)
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

        public Message CreateMessage(object content) =>
            new Message(content, DetermineMessageIdFor(content), CorrelationId);

        public Message<TMessage> CreateMessage<TMessage>(TMessage content) =>
            new Message<TMessage>(content, DetermineMessageIdFor(content), CorrelationId);

        private string DetermineMessageIdFor(object content) =>
            MessageId ?? _messageIdFactory.GenerateMessageIdFor(content);
    }
}
