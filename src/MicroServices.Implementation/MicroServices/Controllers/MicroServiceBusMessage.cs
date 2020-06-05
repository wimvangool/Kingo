using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using Kingo.MicroServices.DataContracts;

namespace Kingo.MicroServices.Controllers
{
    /// <summary>
    /// Represents a <see cref="IMicroServiceBusMessage"/> that can be stored inside a <see cref="MicroServiceBusOutbox"/>.
    /// </summary>
    [Serializable]
    public sealed class MicroServiceBusMessage : IMicroServiceBusMessage
    {
        private readonly IMicroServiceBusMessage _message;
        private readonly MessageKind _kind;

        internal MicroServiceBusMessage(IMicroServiceBusMessage message, MessageKind kind)
        {
            _message = message;
            _kind = kind;
        }

        #region [====== ToString() ======]

        /// <inheritdoc />
        public override string ToString() =>
            ToString(this);

        internal static string ToString(IMicroServiceBusMessage message) =>
            DataContractBlob.ToString(message.ContentType, message.Content.Length);

        #endregion

        #region [====== Kind ======]

        /// <summary>
        /// Gets the kind of this message.
        /// </summary>
        public MessageKind Kind =>
            _kind;

        #endregion

        #region [====== MessageId & CorrelationId ======]

        /// <inheritdoc />
        public string MessageId =>
            _message.MessageId;

        /// <inheritdoc />
        public string CorrelationId =>
            _message.CorrelationId;

        #endregion

        #region [====== Routing & Delivery ======]

        /// <inheritdoc />
        public DateTimeOffset? DeliveryTimeUtc =>
            _message.DeliveryTimeUtc;

        #endregion

        #region [====== ContentType & Content ======]

        /// <inheritdoc />
        public string ContentType =>
            _message.ContentType;

        /// <inheritdoc />
        public MemoryStream Content =>
            _message.Content;

        #endregion
    }
}
