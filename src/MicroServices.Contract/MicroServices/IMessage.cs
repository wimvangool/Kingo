using System;

namespace Kingo.MicroServices
{
    /// <summary>
    /// When implemented by a class, represents (the envelope of) a message, carrying its payload and metadata.
    /// </summary>
    public interface IMessage
    {
        #region [====== Kind & Direction ======]

        /// <summary>
        /// Indicates what kind of message this is.
        /// </summary>
        MessageKind Kind
        {
            get;
        }

        /// <summary>
        /// Indicates what the direction of this message is.
        /// </summary>
        MessageDirection Direction
        {
            get;
        }

        #endregion

        #region [====== MessageId & CorrelationId ======]

        /// <summary>
        /// Gets the unique identifier of this message.
        /// </summary>
        string MessageId
        {
            get;
        }

        /// <summary>
        /// If specified, returns the message-id of the message this message is correlated with
        /// (which is typically the message that triggered this message to be dispatched or processed).
        /// </summary>
        string CorrelationId
        {
            get;
        }

        #endregion

        #region [====== Routing & Delivery ======]

        /// <summary>
        /// If specified, indicates at what (UTC) time the message should be delivered.
        /// </summary>
        DateTimeOffset? DeliveryTimeUtc
        {
            get;
        }

        #endregion

        #region [====== Content ======]

        /// <summary>
        /// Returns the contents/payload of the message.
        /// </summary>
        object Content
        {
            get;
        }

        /// <summary>
        /// Converts this message to an instance of type <see cref="IMessage{TContent}"/>.
        /// </summary>
        /// <typeparam name="TContent">(Expected) type of the content of this message.</typeparam>
        /// <returns>The converted message.</returns>
        /// <exception cref="InvalidCastException">
        /// The <see cref="Content"/> of this message could not be converted to the specified type.
        /// </exception>
        IMessage<TContent> ConvertTo<TContent>();

        /// <summary>
        /// Attempts to convert this message to an instance of type <see cref="IMessage{TContent}"/>.
        /// </summary>
        /// <typeparam name="TContent">(Expected) type of the content of this message.</typeparam>
        /// <param name="message">
        /// If the conversion succeeds, this parameter will refer to the converted message;
        /// otherwise it will be <c>null</c>.
        /// </param>
        /// <returns><c>true</c> if the conversion succeeds; otherwise <c>false</c>.</returns>
        bool TryConvertTo<TContent>(out IMessage<TContent> message);

        #endregion
    }
}
