using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Kingo.MicroServices.DataContracts;
using Kingo.Reflection;

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

        public override Message<TContent> CommitToKind(MessageKind kind) =>
            new MessageValidator<TContent>(_message.CommitToKind(kind), _expectedKind, _validationOptions);

        #endregion

        #region [====== MessageId & CorrelationId ======]

        public override string MessageId =>
            _message.MessageId;

        public override string CorrelationId =>
            _message.CorrelationId;

        public override Message<TContent> CorrelateWith(IMessage message) =>
            new MessageValidator<TContent>(_message.CorrelateWith(message), _expectedKind, _validationOptions);

        #endregion

        #region [====== Routing & Delivery ======]

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

        public override Message<TContent> Validate(IServiceProvider serviceProvider)
        {
            var message = ValidateKind(_message, _expectedKind, _validationOptions);
            if (message.MustBeValidated(_validationOptions))
            {
                return ValidateContent(message, serviceProvider);
            }
            return message;
        }

        private static Message<TContent> ValidateKind(Message<TContent> message, MessageKind expectedKind, MessageValidationOptions validationOptions)
        {
            if (message.Kind == expectedKind)
            {
                return message;
            }
            if (message.Kind == MessageKind.Undefined)
            {
                if (validationOptions.HasFlag(MessageValidationOptions.BlockUndefined))
                {
                    throw NewUndefinedMessagesBlockedException(new MessageValidator<TContent>(message, expectedKind, validationOptions));
                }
                return message.CommitToKind(expectedKind);
            }
            throw NewMessageKindMismatchException(message, expectedKind);
        }

        private static Message<TContent> ValidateContent(Message<TContent> message, IServiceProvider serviceProvider)
        {
            if (message.Content.IsNotValid(out var validationErrors, serviceProvider))
            {
                throw NewContentNotValidException(message, validationErrors);
            }
            return message;
        }

        private static Exception NewUndefinedMessagesBlockedException(IMessage<TContent> invalidMessage)
        {
            var messageFormat = ExceptionMessages.Message_UndefinedMessagesBlocked;
            var message = string.Format(messageFormat, invalidMessage.Content.GetType().FriendlyName(), MessageKind.Undefined, invalidMessage.Direction);
            return new MessageValidationFailedException(invalidMessage, Enumerable.Empty<ValidationResult>(), message);
        }

        private static Exception NewMessageKindMismatchException(IMessage<TContent> invalidMessage, MessageKind expectedKind)
        {
            var messageFormat = ExceptionMessages.Message_MessageKindMismatch;
            var message = string.Format(messageFormat, invalidMessage.Content.GetType().FriendlyName(), invalidMessage.Kind, expectedKind);
            return new MessageValidationFailedException(invalidMessage, Enumerable.Empty<ValidationResult>(), message);
        }

        private static Exception NewContentNotValidException(IMessage<TContent> invalidMessage, ICollection<ValidationResult> validationErrors)
        {
            var messageFormat = ExceptionMessages.Message_ContentNotValid;
            var message = string.Format(messageFormat, invalidMessage.Content.GetType().FriendlyName(), validationErrors.Count);
            return new MessageValidationFailedException(invalidMessage, validationErrors, message);
        }

        #endregion
    }
}
