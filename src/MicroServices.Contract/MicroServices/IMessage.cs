using System;

namespace Kingo.MicroServices
{
    /// <summary>
    /// When implemented by a class, represents (the envelope of) a message, carrying its payload and metadata.
    /// </summary>
    public interface IMessage
    {
        #region [====== Kind ======]

        /// <summary>
        /// Indicates what kind of message this is.
        /// </summary>
        MessageKind Kind
        {
            get;
        }

        #endregion

        #region [====== Id & Correlation ======]

        /// <summary>
        /// Returns the unique identifier of this message.
        /// </summary>
        string Id
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

        /// <summary>
        /// Correlates this message with the specified <paramref name="message"/> by setting the <see cref="CorrelationId"/>
        /// of this message to the <see cref="Id"/> of the specified <paramref name="message"/>.
        /// </summary>
        /// <param name="message">Another message.</param>
        /// <returns>A new message with the updated <see cref="CorrelationId"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message" /> is <c>null</c>.
        /// </exception>
        IMessage CorrelateWith(IMessage message);

        #endregion

        #region [====== DeliveryTime ======]

        /// <summary>
        /// If specified, indicates at what (UTC) time the message should be delivered.
        /// </summary>
        DateTimeOffset? DeliveryTimeUtc
        {
            get;
        }

        /// <summary>
        /// Schedules this message to be delivered at the specified <see cref="deliveryTime"/>.
        /// </summary>
        /// <param name="deliveryTime">The desired delivery time.</param>
        /// <returns>This message with the specified <paramref name="deliveryTime"/>.</returns>
        IMessage DeliverAt(DateTimeOffset deliveryTime);

        #endregion

        #region [====== Content & Conversion ======]

        /// <summary>
        /// Returns the contents/payload of the message.
        /// </summary>
        object Content
        {
            get;
        }

        /// <summary>
        /// Converts this message to an instance of type <see cref="Message{TMessage}"/>.
        /// </summary>
        /// <typeparam name="TContent">Type of the content of the message.</typeparam>
        /// <returns>The converted message.</returns>
        /// <exception cref="InvalidCastException">
        /// The <see cref="Content"/> of this message is not an instance of type <typeparamref name="TContent"/>.
        /// </exception>
        Message<TContent> ConvertTo<TContent>();

        /// <summary>
        /// Attempts to convert this message to an instance of type <see cref="Message{TMessage}"/>.
        /// </summary>
        /// <typeparam name="TContent">Type of the content of the message.</typeparam>
        /// <param name="message">
        /// If the conversion succeeds, this parameter will be set to the converted value;
        /// otherwise it will be set to <c>null</c>.
        /// </param>
        /// <returns><c>true</c> if the conversion succeeds, otherwise <c>false</c>.</returns>
        bool TryConvertTo<TContent>(out Message<TContent> message);

        #endregion
    }
}
