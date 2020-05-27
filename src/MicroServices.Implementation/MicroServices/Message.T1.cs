using System;
using Kingo.Reflection;

namespace Kingo.MicroServices
{
    [Serializable]
    internal abstract class Message<TContent> : IMessage<TContent>
    {
        public override string ToString() =>
            $"{Content.GetType().FriendlyName()} ({Kind} | {Direction} | {MessageId})";

        #region [====== Kind & Direction ======]

        public abstract MessageKind Kind
        {
            get;
        }

        public abstract MessageDirection Direction
        {
            get;
        }

        public abstract Message<TContent> CommitToKind(MessageKind kind);

        #endregion

        #region [====== MessageId & CorrelationId ======]

        public abstract string MessageId
        {
            get;
        }

        public abstract string CorrelationId
        {
            get;
        }

        public abstract Message<TContent> CorrelateWith(IMessage message);

        #endregion

        #region [====== Routing & Delivery ======]

        public abstract DateTimeOffset? DeliveryTimeUtc
        {
            get;
        }

        public abstract Message<TContent> DeliverAt(DateTimeOffset? deliveryTime);

        #endregion

        #region [====== Content & Conversion ======]

        object IMessage.Content =>
            Content;

        public abstract TContent Content
        {
            get;
        }

        IMessage<TOther> IMessage.ConvertTo<TOther>() =>
            ConvertTo<TOther>();

        public Message<TOther> ConvertTo<TOther>()
        {
            if (TryConvertTo<TOther>(out var message))
            {
                return message;
            }
            throw NewConversionFailedException(typeof(TOther), Content.GetType());
        }

        bool IMessage.TryConvertTo<TOther>(out IMessage<TOther> message)
        {
            if (TryConvertTo(out Message<TOther> convertedMessage))
            {
                message = convertedMessage;
                return true;
            }
            message = null;
            return false;
        }

        public abstract bool TryConvertTo<TOther>(out Message<TOther> message);

        private static Exception NewConversionFailedException(Type expectedType, Type actualType)
        {
            var messageFormat = ExceptionMessages.Message_ConversionFailed;
            var message = string.Format(messageFormat, actualType.FriendlyName(), expectedType.FriendlyName());
            return new InvalidCastException(message);
        }

        #endregion

        #region [====== Validation ======]

        public abstract Message<TContent> Validate(IServiceProvider serviceProvider);

        public bool MustBeValidated(MessageValidationOptions options)
        {
            switch (Kind)
            {
                case MessageKind.Command:
                    return options.HasFlag(MessageValidationOptions.Commands);
                case MessageKind.Event:
                    return options.HasFlag(MessageValidationOptions.Events);
                case MessageKind.Request:
                    return options.HasFlag(MessageValidationOptions.Requests);
                case MessageKind.Response:
                    return options.HasFlag(MessageValidationOptions.Responses);
                default:
                    return false;
            }
        }

        #endregion
    }
}
