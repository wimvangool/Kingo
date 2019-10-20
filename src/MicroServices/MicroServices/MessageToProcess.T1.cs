using System;
using Kingo.Reflection;

namespace Kingo.MicroServices
{        
    public sealed class MessageToProcess<TMessage> : MessageEnvelope<TMessage>, IMessageToProcess
    {
        internal MessageToProcess(MessageEnvelope<TMessage> message, MessageKind kind) :
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
