using System;
using Kingo.Reflection;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Represents a command or event that is to be sent or published on a service-bus.
    /// </summary>
    public sealed class MessageToDispatch<TMessage> : MessageEnvelope<TMessage>, IMessageToDispatch, IMessageToProcess
    {
        internal MessageToDispatch(MessageEnvelope<TMessage> message, MessageKind kind, DateTimeOffset? deliveryTimeUtc) :
            base(message)
        {
            Kind = kind;
            DeliveryTimeUtc = deliveryTimeUtc;
        }

        /// <inheritdoc />
        public MessageKind Kind
        {
            get;
        }

        /// <inheritdoc />
        public DateTimeOffset? DeliveryTimeUtc
        {
            get;
        }

        public override string ToString() =>
            MessageToDispatch.ToString(this);
    }
}
