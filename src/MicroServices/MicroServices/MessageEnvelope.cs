using System;
using Kingo.Reflection;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Represents the envelope of a message carrying its payload and metadata.
    /// </summary>
    public class MessageEnvelope : IMessageEnvelope
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
        public MessageEnvelope(object message, string messageId, string correlationId = null)
        {
            MessageId = messageId ?? throw new ArgumentNullException(nameof(messageId));
            CorrelationId = correlationId;
            Content = message ?? throw new ArgumentNullException(nameof(message));
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

        /// <summary>
        /// Returns the contents/payload of the message.
        /// </summary>
        public object Content
        {
            get;
        }

        /// <inheritdoc />
        public override string ToString() =>
            Content.GetType().FriendlyName();
    }
}
