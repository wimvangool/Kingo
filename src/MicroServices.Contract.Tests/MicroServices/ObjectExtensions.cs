using System;

namespace Kingo.MicroServices
{
    public static class ObjectExtensions
    {
        public static MessageToDispatch ToCommand(this object message) =>
            message.ToMessage().ToDispatch(MessageKind.Command);

        public static MessageToDispatch ToEvent(this object message) =>
            message.ToMessage().ToDispatch(MessageKind.Event);

        public static MessageEnvelope ToMessage(this object message) =>
            new MessageEnvelope(message, NewMessageId());

        private static string NewMessageId() =>
            Guid.NewGuid().ToString();
    }
}
