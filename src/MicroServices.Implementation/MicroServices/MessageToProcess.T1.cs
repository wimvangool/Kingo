using System;
using Kingo.Reflection;

namespace Kingo.MicroServices
{        
    internal sealed class MessageToProcess<TMessage> : IMessageToProcess<TMessage>
    {
        public MessageToProcess(TMessage content, MessageKind kind)
        {                       
            if (ReferenceEquals(content, null))
            {
                throw new ArgumentNullException(nameof(content));
            }
            Content = content;
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
