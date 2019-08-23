using System;
using Kingo.Reflection;

namespace Kingo.MicroServices
{
    internal sealed class MessageKindResolver : IMessageKindResolver
    {
        public override string ToString() =>
            GetType().FriendlyName();

        public MessageKind ResolveMessageKind(Type messageType)
        {
            if (NameOf(messageType).EndsWith("Command"))
            {
                return MessageKind.Command;
            }
            return MessageKind.Event;
        }

        private static string NameOf(Type messageType) =>
            messageType.IsGenericType ? messageType.Name.RemoveTypeParameterCount() : messageType.Name;
    }
}
