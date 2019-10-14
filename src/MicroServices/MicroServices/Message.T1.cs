using System;
using System.Collections.Generic;
using System.Text;
using Kingo.Reflection;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Represents a message with a specific <see cref="MessageId" /> and a payload of type <typeparamref name="TMessage"/>.
    /// </summary>
    /// <typeparam name="TMessage">Type of the message payload.</typeparam>
    public class Message<TMessage> : IMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Message{TMessage}" /> class.
        /// </summary>
        /// <param name="content">Content of this message.</param>
        /// <param name="messageId">Unique identifier of this message.</param>
        /// <param name="correlationId">
        /// Identifier of the message this message to related to.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="content"/> or <paramref name="messageId"/> is <c>null</c>.
        /// </exception>
        public Message(TMessage content, string messageId, string correlationId = null)
        {
            if (ReferenceEquals(content, null))
            {
                throw new ArgumentException(nameof(content));
            }
            MessageId = messageId ?? throw new ArgumentNullException(nameof(messageId));
            CorrelationId = correlationId;
            Content = content;
        }

        internal Message(Message<TMessage> message)
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

        object IMessage.Content =>
            Content;

        /// <summary>
        /// Returns the contents/payload of the message.
        /// </summary>
        public TMessage Content
        {
            get;
        }

        /// <summary>
        /// Converts this message to a message of a specific <see cref="MessageKind" /> to process by a <see cref="IMicroProcessor" />.
        /// </summary>
        /// <param name="kind">The kind of this message.</param>
        /// <returns>A new <see cref="MessageToProcess{TMessage}"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="kind"/> is not a valid value.
        /// </exception>
        public MessageToProcess<TMessage> ToProcess(MessageKind kind) =>
            new MessageToProcess<TMessage>(this, kind);

        /// <inheritdoc />
        public override string ToString() =>
            $"{Content.GetType().FriendlyName()} ({nameof(MessageId)} = {MessageId})";
    }
}
