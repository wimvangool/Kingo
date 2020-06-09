using System;
using System.IO;

namespace Kingo.MicroServices.Controllers
{
    /// <summary>
    /// When implemented by a class, represents a serialized version of a <see cref="IMessage"/> that can be
    /// transported over a <see cref="IMicroServiceBus" />.
    /// </summary>
    public interface IMicroServiceBusMessage
    {
        #region [====== MessageId & CorrelationId ======]

        /// <summary>
        /// Gets the unique identifier of this message.
        /// </summary>
        public string MessageId
        {
            get;
        }

        /// <summary>
        /// Gets the <see cref="MessageId"/> of the message that this message is correlated with.
        /// </summary>
        public string CorrelationId
        {
            get;
        }

        #endregion

        #region [====== Routing & Delivery ======]

        /// <summary>
        /// Indicates at what (UTC) time the message should be delivered.
        /// </summary>
        public DateTimeOffset? DeliveryTimeUtc
        {
            get;
        }

        #endregion

        #region [====== ContentType & Content ======]

        /// <summary>
        /// Identifies the type or schema of the <see cref="Content"/> of this message.
        /// </summary>
        public string ContentType
        {
            get;
        }

        /// <summary>
        /// The (serialized) content of the message.
        /// </summary>
        public MemoryStream Content
        {
            get;
        }

        #endregion
    }
}
