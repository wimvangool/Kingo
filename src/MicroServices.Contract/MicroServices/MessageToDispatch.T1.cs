using System;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Represents a command or event that is to be sent or published on a service-bus.
    /// </summary>
    [Serializable]
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

        /// <inheritdoc />
        public override string ToString() =>
            MessageToDispatch.ToString(this);

        /// <summary>
        /// Converts this message to a message to process.
        /// </summary>
        /// <returns>A new message to process.</returns>
        public MessageToProcess<TMessage> ToProcess() =>
            new MessageToProcess<TMessage>(this, Kind);
    }
}
