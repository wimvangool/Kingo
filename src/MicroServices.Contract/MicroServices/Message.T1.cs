using System;
using System.Xml.Schema;
using System.Xml.Xsl;
using Kingo.Reflection;
using static Kingo.Ensure;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Represents a message with a specific <see cref="Id" /> and a payload of type <typeparamref name="TContent"/>.
    /// </summary>
    /// <typeparam name="TContent">Type of the message payload.</typeparam>
    [Serializable]
    public sealed class Message<TContent> : IMessage
    {
        private Message(Message<TContent> message, MessageKind kind)
        {
            Kind = kind;
            Id = message.Id;
            CorrelationId = message.CorrelationId;
            DeliveryTimeUtc = message.DeliveryTimeUtc;
            Content = message.Content;
        }

        private Message(Message<TContent> message, string correlationId)
        {
            Kind = message.Kind;
            Id = message.Id;
            CorrelationId = correlationId;
            DeliveryTimeUtc = message.DeliveryTimeUtc;
            Content = message.Content;
        }

        private Message(Message<TContent> message, DateTimeOffset deliveryTime)
        {
            Kind = message.Kind;
            Id = message.Id;
            CorrelationId = message.CorrelationId;
            DeliveryTimeUtc = deliveryTime.ToUniversalTime();
            Content = message.Content;
        }

        private Message(IMessage message, TContent content)
        {
            Kind = message.Kind;
            Id = message.Id;
            CorrelationId = message.CorrelationId;
            DeliveryTimeUtc = message.DeliveryTimeUtc;
            Content = content;
        }

        internal Message(MessageKind kind, string id, TContent content)
        {
            Kind = kind;
            Id = id;
            Content = content;
        }

        #region [====== Kind ======]

        /// <inheritdoc />
        public MessageKind Kind
        {
            get;
        }

        #endregion

        #region [====== Id & Correlation ======]

        /// <inheritdoc />
        public string Id
        {
            get;
        }

        /// <inheritdoc />
        public string CorrelationId
        {
            get;
        }

        IMessage IMessage.CorrelateWith(IMessage message) =>
            CorrelateWith(message);

        /// <summary>
        /// Correlates this message with the specified <paramref name="message"/> by setting the <see cref="CorrelationId"/>
        /// of this message to the <see cref="Id"/> of the specified <paramref name="message"/>.
        /// </summary>
        /// <param name="message">Another message.</param>
        /// <returns>A new message with the updated <see cref="CorrelationId"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message" /> is <c>null</c>.
        /// </exception>
        public Message<TContent> CorrelateWith(IMessage message) =>
            new Message<TContent>(this, IsNotNull(message, nameof(message)).Id);

        #endregion

        #region [====== DeliveryTime ======]

        /// <inheritdoc />
        public DateTimeOffset? DeliveryTimeUtc
        {
            get;
        }

        IMessage IMessage.DeliverAt(DateTimeOffset deliveryTime) =>
            DeliverAt(deliveryTime);

        /// <summary>
        /// Schedules this message to be delivered at the specified <see cref="deliveryTime"/>.
        /// </summary>
        /// <param name="deliveryTime">The desired delivery time.</param>
        /// <returns>This message with the specified <paramref name="deliveryTime"/>.</returns>
        public Message<TContent> DeliverAt(DateTimeOffset deliveryTime) =>
            new Message<TContent>(this, deliveryTime);

        #endregion

        #region [====== Content & Conversion ======]

        object IMessage.Content =>
            Content;

        /// <summary>
        /// Returns the contents/payload of the message.
        /// </summary>
        public TContent Content
        {
            get;
        }

        /// <inheritdoc />
        public Message<TOther> ConvertTo<TOther>()
        {
            if (TryConvertTo<TOther>(out var message))
            {
                return message;
            }
            throw NewInvalidCastException(typeof(TOther), Content.GetType());
        }

        /// <inheritdoc />
        public bool TryConvertTo<TOther>(out Message<TOther> message)
        {
            if (Content is TOther content)
            {
                message = new Message<TOther>(this, content);
                return true;
            }
            message = null;
            return false;
        }

        /// <inheritdoc />
        public override string ToString() =>
            $"{Content.GetType().FriendlyName()} ({Kind}, {Id})";

        private static Exception NewInvalidCastException(Type expectedType, Type actualType)
        {
            var messageFormat = ExceptionMessages.Message_IsValidCast;
            var message = string.Format(messageFormat, actualType.FriendlyName(), expectedType.FriendlyName());
            return new InvalidCastException(message);
        }

        #endregion
    }
}
