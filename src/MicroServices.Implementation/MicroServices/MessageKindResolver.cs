using System;
using Kingo.Reflection;

namespace Kingo.MicroServices
{
    internal sealed class MessageKindResolver : IMessageKindResolver
    {
        public override string ToString() =>
            GetType().FriendlyName();

        public MessageKind ResolveMessageKind(Type messageType) =>
            ResolveMessageKind(NameOf(messageType));

        private static MessageKind ResolveMessageKind(string messageTypeName)
        {
            if (messageTypeName.EndsWith("Command"))
            {
                return MessageKind.Command;
            }
            if (messageTypeName.EndsWith("Event"))
            {
                return MessageKind.Event;
            }
            if (messageTypeName.EndsWith("Request"))
            {
                return MessageKind.QueryRequest;
            }
            if (messageTypeName.EndsWith("Response"))
            {
                return MessageKind.QueryResponse;
            }
            return MessageKind.Unspecified;
        }

        private static string NameOf(Type messageType) =>
            messageType.FriendlyName(false, false);
    }
}
