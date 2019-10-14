using System;
using System.Collections.Generic;
using System.Text;
using Kingo.Reflection;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Represents a message with a specific <see cref="MessageId" /> and <see cref="CorrelationId" />.
    /// </summary>
    public class Message : IMessage
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
        public Message(object content, string messageId, string correlationId = null)
        {
            MessageId = messageId ?? throw new ArgumentNullException(nameof(messageId));
            CorrelationId = correlationId;
            Content = content ?? throw new ArgumentNullException(nameof(content));
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
