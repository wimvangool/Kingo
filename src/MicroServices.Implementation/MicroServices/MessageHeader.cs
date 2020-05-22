using System;
using System.Text;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Represents the header of a <see cref="IMessage{TContent}"/>, which contains its
    /// <see cref="IMessage{TContent}.Id"/> and <see cref="IMessage{TContent}.CorrelationId" />.
    /// </summary>
    [Serializable]
    public readonly struct MessageHeader : IEquatable<MessageHeader>
    {
        /// <summary>
        /// Represents an empty header.
        /// </summary>
        public static readonly MessageHeader Unspecified = new MessageHeader();

        /// <summary>
        /// Generates and returns a new <see cref="MessageHeader" /> where the <see cref="MessageHeader.Id"/>
        /// has been set to a random <see cref="Guid"/>.
        /// </summary>
        /// <returns>A new header.</returns>
        public static MessageHeader NewHeader() =>
            new MessageHeader(Guid.NewGuid().ToString());

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageHeader" /> class.
        /// </summary>
        /// <param name="id">Identifier of the message.</param>
        public MessageHeader(string id) :
            this(id, null) { }

        private MessageHeader(string id, string correlationId)
        {
            Id = id;
            CorrelationId = correlationId;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            if (Id == null)
            {
                return nameof(Unspecified);
            }
            var header = new StringBuilder($"{nameof(Id)} = {Id}");

            if (CorrelationId != null)
            {
                header = header.Append($", {nameof(CorrelationId)} = {CorrelationId}");
            }
            return header.ToString();
        }

        #region [====== Equals & GetHashCode ======]

        /// <inheritdoc />
        public override bool Equals(object obj) =>
            obj is MessageHeader other && Equals(other);

        /// <inheritdoc />
        public bool Equals(MessageHeader other) =>
            Id == other.Id && CorrelationId == other.CorrelationId;

        /// <inheritdoc />
        public override int GetHashCode() =>
            HashCode.Combine(GetType(), Id, CorrelationId);

        #endregion

        #region [====== Id & Correlation ======]

        /// <summary>
        /// Identifier of the message.
        /// </summary>
        public string Id
        {
            get;
        }

        /// <summary>
        /// Identifier of the correlated message.
        /// </summary>
        public string CorrelationId
        {
            get;
        }

        internal MessageHeader WithId(IMessageIdGenerator messageIdGenerator, object content)
        {
            if (Id == null)
            {
                return new MessageHeader(messageIdGenerator.GenerateMessageId(content));
            }
            return this;
        }

        /// <summary>
        /// Assigns the specified <paramref name="correlationId"/> to this header.
        /// </summary>
        /// <param name="correlationId">The correlation-id to assign.</param>
        /// <returns>A new header where the <see cref="CorrelationId"/> has been set to the specified <paramref name="correlationId"/>.</returns>
        /// <exception cref="InvalidOperationException">
        /// The <see cref="Id"/> of this header is unspecified.
        /// </exception>
        public MessageHeader WithCorrelationId(string correlationId)
        {
            if (Id == null)
            {
                throw NewIdRequiredException();
            }
            return new MessageHeader(Id, correlationId);
        }

        private static Exception NewIdRequiredException() =>
            new InvalidOperationException(ExceptionMessages.MessageHeader_IdNotSpecified);

        #endregion
    }
}
