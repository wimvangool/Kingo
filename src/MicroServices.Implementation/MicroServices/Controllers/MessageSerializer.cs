using System;
using System.Collections.Generic;
using System.Text;
using Kingo.MicroServices.DataContracts;
using static Kingo.Ensure;

namespace Kingo.MicroServices.Controllers
{
    internal sealed class MessageSerializer : IMessageSerializer
    {
        private readonly IDataContractSerializer _serializer;

        public MessageSerializer(IDataContractSerializer serializer = null)
        {
            _serializer = serializer ?? new DataContractSerializer();
        }

        #region [====== Serialize ======]

        public IMicroServiceBusMessage Serialize(IMessage message) =>
            new MessageBlob(message, _serializer.Serialize(IsNotNull(message, nameof(message)).Content));

        #endregion

        #region [====== Deserialize ======]

        public IMessage DeserializeInput(IMicroServiceBusMessage message, MessageKind kind) =>
            Deserialize(IsNotNull(message, nameof(message)), kind, MessageDirection.Input);

        public IMessage DeserializeOutput(IMicroServiceBusMessage message, MessageKind kind) =>
            Deserialize(IsNotNull(message, nameof(message)), kind, MessageDirection.Output);

        private IMessage Deserialize(IMicroServiceBusMessage message, MessageKind kind, MessageDirection direction)
        {
            return new Message(_serializer.Deserialize(message.Content.ToArray(), message.ContentType))
            {
                Kind = kind,
                Direction = direction,
                MessageId = message.MessageId,
                CorrelationId = message.CorrelationId,
                DeliveryTimeUtc = message.DeliveryTimeUtc
            };
        }

        #endregion
    }
}
