using System;

namespace Kingo.MicroServices
{
    [Serializable]
    internal sealed class MessageImplementation<TContent> : Message<TContent>
    {
        private readonly MessageKind _kind;
        private readonly MessageDirection _direction;
        private readonly MessageHeader _header;
        private readonly TContent _content;
        private readonly DateTimeOffset? _deliveryTimeUtc;

        public MessageImplementation(MessageKind kind, MessageDirection direction, MessageHeader header, TContent content, DateTimeOffset? deliveryTime = null)
        {
            _kind = kind;
            _direction = direction;
            _header = header;
            _content = content;
            _deliveryTimeUtc = deliveryTime?.ToUniversalTime();
        }

        #region [====== Kind & Direction ======]

        public override MessageKind Kind =>
            _kind;

        public override MessageDirection Direction =>
            _direction;

        public override Message<TContent> CommitToKind(MessageKind kind) =>
            new MessageImplementation<TContent>(kind, _direction, _header, _content, _deliveryTimeUtc);

        #endregion

        #region [====== Id & Correlation ======]

        public override string Id =>
            _header.Id;

        public override string CorrelationId =>
            _header.CorrelationId;

        public override Message<TContent> CorrelateWith(IMessage message) =>
            new MessageImplementation<TContent>(_kind, _direction, _header.WithCorrelationId(message?.Id), _content, _deliveryTimeUtc);

        #endregion

        #region [====== DeliveryTime ======]

        public override DateTimeOffset? DeliveryTimeUtc =>
            _deliveryTimeUtc;

        public override Message<TContent> DeliverAt(DateTimeOffset? deliveryTime) =>
            new MessageImplementation<TContent>(_kind, _direction, _header, _content, deliveryTime);

        #endregion

        #region [====== Content & Conversion ======]

        public override TContent Content =>
            _content;

        public override bool TryConvertTo<TOther>(out Message<TOther> message)
        {
            if (Content is TOther content)
            {
                message = new MessageImplementation<TOther>(_kind, _direction, _header, content, _deliveryTimeUtc);
                return true;
            }
            message = null;
            return false;
        }

        #endregion

        #region [====== Validation ======]

        public override Message<TContent> Validate(IServiceProvider serviceProvider) =>
            this;

        #endregion
    }
}
