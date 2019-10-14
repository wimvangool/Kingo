using System;
using System.Collections.Generic;
using System.Text;

namespace Kingo.MicroServices
{
    public static class ObjectExtensions
    {
        public static MessageToDispatch ToCommand(this object content) =>
            content.ToMessage().ToDispatch(MessageKind.Command);

        public static MessageToDispatch ToEvent(this object content) =>
            content.ToMessage().ToDispatch(MessageKind.Event);

        public static Message ToMessage(this object content) =>
            new Message(content, NewMessageId());

        private static string NewMessageId() =>
            Guid.NewGuid().ToString();
    }
}
