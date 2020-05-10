using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using static Kingo.Ensure;

namespace Kingo.MicroServices
{
    internal sealed class MessageIdGenerator : MessageAttributeConsumer, IMessageIdGenerator
    {
        private readonly IMessageIdGenerator _generator;
        private readonly ConcurrentDictionary<Type, MessageIdFormatter> _formatters;

        public MessageIdGenerator(IDictionary<Type, MessageAttribute> attributes, IMessageIdGenerator generator) : base(attributes)
        {
            _generator = generator;
            _formatters = new ConcurrentDictionary<Type, MessageIdFormatter>();
        }

        public string GenerateMessageId(object content)
        {
            if (TryGetMessageIdFormatter(IsNotNull(content, nameof(content)).GetType(), out var formatter))
            {
                return formatter.FormatMessageId(content);
            }
            return _generator.GenerateMessageId(content);
        }

        private bool TryGetMessageIdFormatter(Type contentType, out MessageIdFormatter formatter) =>
            (formatter = GetMessageIdFormatter(contentType)) != null;

        private MessageIdFormatter GetMessageIdFormatter(Type contentType) =>
            _formatters.GetOrAdd(contentType, CreateMessageIdFormatter);

        private MessageIdFormatter CreateMessageIdFormatter(Type contentType)
        {
            if (TryGetMessageAttribute(contentType, out var attribute) && attribute.MessageIdFormat != null)
            {
                return MessageIdFormatter.FromContentType(contentType, attribute.MessageIdFormat, attribute.MessageIdProperties);
            }
            return null;
        }
    }
}
