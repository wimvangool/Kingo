using System;
using System.Collections.Generic;
using System.Text;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Represents a command or event that is to be sent or published on a service-bus.
    /// </summary>
    public sealed class MessageToDispatch<TMessage> : IMessageToDispatch, IMessageToProcess<TMessage>
    {
        private MessageToDispatch(TMessage content, MessageKind kind, DateTimeOffset? deliveryTimeUtc)
        {
            Content = content;
            Kind = kind;
            DeliveryTimeUtc = deliveryTimeUtc;
        }

        object IMessageEnvelope.Content =>
            Content;

        /// <summary>
        /// Returns the contents of the message.
        /// </summary>
        public TMessage Content
        {
            get;
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

        internal static bool IsOfExpectedType(MessageToDispatch message, MessageKind kind, out MessageToDispatch<TMessage> messageOfExpectedType)
        {
            if (message.Kind == kind && message.Content is TMessage content)
            {
                messageOfExpectedType = new MessageToDispatch<TMessage>(content, message.Kind, message.DeliveryTimeUtc);
                return true;
            }
            messageOfExpectedType = null;
            return false;
        }
    }
}
