using System;
using System.Collections.Generic;
using System.Text;

namespace Kingo.MicroServices
{
    internal static class MessageKindExtensions
    {
        public static MessageKind Validate(this MessageKind kind)
        {
            switch (kind)
            {
                case MessageKind.Command:
                case MessageKind.Event:
                case MessageKind.QueryRequest:
                case MessageKind.QueryResponse:
                    return kind;
                default:
                    throw NewUnsupportedMessageKindException(kind);
            }
        }

        private static Exception NewUnsupportedMessageKindException(MessageKind kind)
        {
            var messageFormat = ExceptionMessages.MessageKindExtensions_MessageKindNotSupported;
            var message = string.Format(messageFormat, kind);
            return new ArgumentOutOfRangeException(nameof(kind), message);
        }
    }
}
