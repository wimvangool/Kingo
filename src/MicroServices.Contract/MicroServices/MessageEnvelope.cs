using System;
using Kingo.Reflection;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Represents the envelope of a message carrying its payload and metadata.
    /// </summary>
    [Serializable]
    public class MessageEnvelope : IMessageEnvelope
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageEnvelope" /> class.
        /// </summary>
        /// <param name="content">Content of this message.</param>
        /// <param name="messageId">Unique identifier of this message.</param>
        /// <param name="correlationId">
        /// Identifier of the message this message to related to.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="content"/> or <paramref name="messageId"/> is <c>null</c>.
        /// </exception>
        public MessageEnvelope(object content, string messageId, string correlationId = null)
        {
            MessageId = messageId ?? throw new ArgumentNullException(nameof(messageId));
            CorrelationId = correlationId;
            Content = content ?? throw new ArgumentNullException(nameof(content));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageEnvelope" /> class.
        /// </summary>
        /// <param name="message">The message to copy.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        public MessageEnvelope(IMessageEnvelope message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }
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
