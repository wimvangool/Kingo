using System;
using Kingo.Reflection;

namespace Kingo.MicroServices
{        
    public sealed class MessageToProcess<TMessage> : Message<TMessage>, IMessageToProcess
    {
        internal MessageToProcess(Message<TMessage> message, MessageKind kind) :
            base(message)
        {
            Kind = kind.Validate();
        }

        public MessageKind Kind
        {
            get;
        }

        public override string ToString() =>
            $"{Content.GetType().FriendlyName()} ({nameof(Kind)} = {Kind})";
    }
}
