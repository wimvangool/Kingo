using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Kingo.MicroServices.DataContracts;

namespace Kingo.MicroServices.Controllers
{
    internal sealed class MessageBlob : IMicroServiceBusMessage
    {
        private readonly MessageHeader _header;
        private readonly DateTimeOffset? _deliveryTimeUtc;
        private readonly DataContractBlob _blob;

        public MessageBlob(IMessage message, DataContractBlob blob)
        {
            _header = new MessageHeader(message.MessageId).WithCorrelationId(message.CorrelationId);
            _deliveryTimeUtc = message.DeliveryTimeUtc;
            _blob = blob;
        }

        #region [====== ToString ======]

        /// <inheritdoc />
        public override string ToString() =>
            MicroServiceBusMessage.ToString(this);

        #endregion

        #region [====== MessageId & CorrelationId ======]

        public string MessageId =>
            _header.MessageId;

        public string CorrelationId =>
            _header.CorrelationId;

        #endregion

        #region [====== Routing & Delivery ======]

        public DateTimeOffset? DeliveryTimeUtc =>
            _deliveryTimeUtc;

        #endregion

        #region [====== ContentType & Content ======]

        public string ContentType =>
            _blob.ContentType.ToString();

        public MemoryStream Content =>
            _blob.Content;

        #endregion
    }
}
