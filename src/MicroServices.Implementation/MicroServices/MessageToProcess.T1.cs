using System;
using Kingo.Reflection;

namespace Kingo.MicroServices
{        
    internal sealed class MessageToProcess<TMessage> : IMessageToProcess<TMessage>
    {
        public MessageToProcess(TMessage message, MessageKind kind)
        {                       
            if (ReferenceEquals(message, null))
            {
                throw new ArgumentNullException(nameof(message));
            }
            Content = Content;
            Kind = kind;
        }
        object IMessageEnvelope.Content =>
            Content;
        
        public TMessage Content
        {
            get;
        }

        public MessageKind Kind
        {
            get;
        }

        public override string ToString() =>
            $"{Content.GetType().FriendlyName()} ({nameof(Kind)} = {Kind})";
    }
}
