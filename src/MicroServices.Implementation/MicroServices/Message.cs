using System;
using Kingo.Reflection;
using static Kingo.Ensure;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Represents a message that can be processed by a <see cref="IMicroProcessor"/>.
    /// </summary>
    public sealed class Message : IMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Message" /> class.
        /// </summary>
        /// <param name="content">The content of this message.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="content"/> is <c>null</c>.
        /// </exception>
        public Message(object content)
        {
            Content = IsNotNull(content, nameof(content));
        }

        #region [====== ToString ======]

        /// <inheritdoc />
        public override string ToString() =>
            ToString(this);

        internal static string ToString(IMessage message) =>
            $"{message.Content.GetType().FriendlyName()} ({message.Kind} | {message.Direction})";

        #endregion

        #region [====== Kind & Direction ======]

        /// <inheritdoc />
        public MessageKind Kind
        {
            get;
            set;
        }

        /// <inheritdoc />
        public MessageDirection Direction
        {
            get;
            set;
        }

        #endregion

        #region [====== MessageId & CorrelationId ======]

        private MessageHeader _header;

        /// <inheritdoc />
        public string MessageId
        {
            get => _header.MessageId;
            set => _header = new MessageHeader(value).WithCorrelationId(CorrelationId);
        }

        /// <inheritdoc />
        public string CorrelationId
        {
            get => _header.CorrelationId;
            set => _header = _header.WithCorrelationId(value);
        }

        #endregion

        #region [====== Routing & Delivery ======]

        private DateTimeOffset? _deliveryTimeUtc;

        /// <inheritdoc />
        public DateTimeOffset? DeliveryTimeUtc
        {
            get => _deliveryTimeUtc;
            set => _deliveryTimeUtc = value?.ToUniversalTime();
        }

        #endregion

        #region [====== Content & Conversion ======]

        /// <inheritdoc />
        public object Content
        {
            get;
        }

        /// <inheritdoc />
        public IMessage<TContent> ConvertTo<TContent>()
        {
            if (TryConvertTo<TContent>(out var message))
            {
                return message;
            }
            throw NewConversionFailedException(typeof(TContent), Content.GetType());
        }

        /// <inheritdoc />
        public bool TryConvertTo<TContent>(out IMessage<TContent> message)
        {
            if (Content is TContent content)
            {
                message = new MessageImplementation<TContent>(Kind, Direction, _header, content, _deliveryTimeUtc);
                return true;
            }
            message = null;
            return false;
        }

        internal static Exception NewConversionFailedException(Type expectedType, Type actualType)
        {
            var messageFormat = ExceptionMessages.Message_ConversionFailed;
            var message = string.Format(messageFormat, actualType.FriendlyName(), expectedType.FriendlyName());
            return new InvalidCastException(message);
        }

        #endregion
    }
}
