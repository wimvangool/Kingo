using System;
using Kingo.Reflection;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Represents a message with a specific <see cref="MessageId" /> and a payload of type <typeparamref name="TMessage"/>.
    /// </summary>
    /// <typeparam name="TMessage">Type of the message payload.</typeparam>
    public class MessageEnvelope<TMessage> : IMessageEnvelope
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageEnvelope{TMessage}" /> class.
        /// </summary>
        /// <param name="message">Content of this message.</param>
        /// <param name="messageId">Unique identifier of this message.</param>
        /// <param name="correlationId">
        /// Identifier of the message this message to related to.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> or <paramref name="messageId"/> is <c>null</c>.
        /// </exception>
        public MessageEnvelope(TMessage message, string messageId, string correlationId = null)
        {
            if (message == null)
            {
                throw new ArgumentException(nameof(message));
            }
            MessageId = messageId ?? throw new ArgumentNullException(nameof(messageId));
            CorrelationId = correlationId;
            Content = message;
        }

        internal MessageEnvelope(MessageEnvelope<TMessage> message)
        {
            MessageId = message.MessageId;
            CorrelationId = message.CorrelationId;
            Content = message.Content;
        }

        /// <inheritdoc />
        public string MessageId
        {
            get;
        }

        /// <inheritdoc />
        public string CorrelationId
        {
            get;
        }

        object IMessageEnvelope.Content =>
            Content;

        /// <summary>
        /// Returns the contents/payload of the message.
        /// </summary>
        public TMessage Content
        {
            get;
        }

        /// <summary>
        /// Converts this message to a message of a specific <see cref="MessageKind" /> to process by a processor.
        /// </summary>
        /// <param name="kind">The kind of this message.</param>
        /// <returns>A new <see cref="MessageToProcess{TMessage}"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="kind"/> is not a valid value.
        /// </exception>
        public MessageToProcess<TMessage> ToProcess(MessageKind kind) =>
            new MessageToProcess<TMessage>(this, kind);

        internal MessageToDispatch<TMessage> ToDispatch(MessageKind kind, DateTimeOffset? deliveryTimeUtc) =>
            new MessageToDispatch<TMessage>(this, kind, deliveryTimeUtc);

        /// <inheritdoc />
        public override string ToString() =>
            $"{Content.GetType().FriendlyName()} ({nameof(MessageId)} = {MessageId})";
    }
}
