using System;
using System.Collections.Generic;
using System.Text;

namespace Kingo.MicroServices
{
    internal sealed class MessageKindResolver : MessageAttributeConsumer, IMessageKindResolver
    {
        private readonly IMessageKindResolver _resolver;

        public MessageKindResolver(IDictionary<Type, MessageAttribute> attributes, IMessageKindResolver resolver) : base(attributes)
        {
            _resolver = resolver;
        }

        public MessageKind ResolveMessageKind(Type contentType)
        {
            if (TryGetMessageAttribute(contentType, out var attribute))
            {
                return attribute.MessageKind;
            }
            return _resolver.ResolveMessageKind(contentType);
        }
    }
}
