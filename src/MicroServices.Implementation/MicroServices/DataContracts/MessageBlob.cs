using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Kingo.MicroServices.DataContracts
{
    /// <summary>
    /// Represents a message of which the contents are serialized into a blob.
    /// </summary>
    [Serializable]
    [DataContract]
    public sealed class MessageBlob 
    {
        #region [====== MessageId & Correlation ======]

        /// <summary>
        /// Gets the unique identifier of this message.
        /// </summary>
        [DataMember]
        public string MessageId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the <see cref="MessageId"/> of the message this message is correlated with.
        /// </summary>
        [DataMember]
        public string CorrelationId
        {
            get;
            set;
        }

        #endregion

        #region [====== Routing & Delivery ======]

        /// <summary>
        /// Indicates at what (UTC) time the message should be delivered.
        /// </summary>
        [DataMember]
        public DateTimeOffset? DeliveryTimeUtc
        {
            get;
            set;
        }

        #endregion

        #region [====== ContentType & Content ======]

        /// <summary>
        /// Identifies the type or schema of the <see cref="Content"/> of this message.
        /// </summary>
        [DataMember]
        public string ContentType
        {
            get;
            set;
        }

        /// <summary>
        /// The (serialized) content of the message.
        /// </summary>
        [DataMember]
        public IReadOnlyList<byte> Content
        {
            get;
            set;
        }

        #endregion
    }
}
