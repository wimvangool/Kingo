using System;
using System.Collections.Generic;
using System.Text;

namespace Kingo.MicroServices
{
    internal sealed class MessageValidator<TContent> : Message<TContent>
    {
        private readonly Message<TContent> _message;
        private readonly MessageKind _expectedKind;
        private readonly MessageValidationOptions _validationOptions;

        public MessageValidator(Message<TContent> message, MessageKind expectedKind, MessageValidationOptions validationOptions)
        {
            _message = message;
            _expectedKind = expectedKind;
            _validationOptions = validationOptions;
        }

        #region [====== Kind & Direction ======]

        public override MessageKind Kind =>
            _expectedKind;

        public override MessageDirection Direction =>
            _message.Direction;

        #endregion

        #region [====== Id & Correlation ======]

        public override string Id =>
            _message.Id;

        public override string CorrelationId =>
            _message.CorrelationId;

        public override Message<TContent> CorrelateWith(IMessage message) =>
            new MessageValidator<TContent>(_message.CorrelateWith(message), _expectedKind, _validationOptions);

        #endregion

        #region [====== DeliveryTime ======]

        public override DateTimeOffset? DeliveryTimeUtc =>
            _message.DeliveryTimeUtc;

        public override Message<TContent> DeliverAt(DateTimeOffset? deliveryTime) =>
            new MessageValidator<TContent>(_message.DeliverAt(deliveryTime), _expectedKind, _validationOptions);

        #endregion

        #region [====== Content & Conversion ======]

        public override TContent Content =>
            _message.Content;

        public override bool TryConvertTo<TOther>(out Message<TOther> message)
        {
            if (_message.TryConvertTo(out Message<TOther> convertedMessage))
            {
                message = new MessageValidator<TOther>(convertedMessage, _expectedKind, _validationOptions);
                return true;
            }
            message = null;
            return false;
        }

        #endregion

        #region [====== Validation ======]

        public override Message<TContent> Validate(IServiceProvider serviceProvider) =>
            throw new NotImplementedException();

        #endregion
    }
}
