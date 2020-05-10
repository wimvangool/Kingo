using System;
using System.Collections.Generic;
using System.Text;

namespace Kingo.MicroServices
{
    public sealed class FixedMessageKindResolver : IMessageKindResolver
    {
        private readonly MessageKind _messageKind;

        public FixedMessageKindResolver(MessageKind messageKind)
        {
            _messageKind = messageKind;
        }

        public MessageKind ResolveMessageKind(Type messageType) =>
            _messageKind;
    }
}
