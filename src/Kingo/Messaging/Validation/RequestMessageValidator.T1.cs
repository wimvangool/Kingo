using System;

namespace Kingo.Messaging.Validation
{
    internal sealed class RequestRequestMessageValidator<TMessage> : IRequestMessageValidator<TMessage>, IRequestMessageValidator
    {
        private readonly IRequestMessageValidator<TMessage> _validator;

        public RequestRequestMessageValidator(IRequestMessageValidator<TMessage> validator)
        {            
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        public override string ToString() =>
            _validator.GetType().FriendlyName();

        public IRequestMessageValidator Append<TOther>(IRequestMessageValidator<TOther> validator) =>
            new CompositeValidator(this, new RequestRequestMessageValidator<TOther>(validator));

        public ErrorInfo Validate(object message, bool haltOnFirstError = false) =>
            Validate(ConvertToSupportedType(message), haltOnFirstError);

        public ErrorInfo Validate(TMessage message, bool haltOnFirstError = false) =>
            _validator.Validate(message, haltOnFirstError);
        
        private static TMessage ConvertToSupportedType(object message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }
            try
            {
                return (TMessage) message;
            }
            catch (InvalidCastException)
            {
                throw NewUnsupportedMessageTypeException(message, typeof(TMessage));
            }
        }

        private static Exception NewUnsupportedMessageTypeException(object messageType, Type supportedType)
        {
            var messageFormat = ExceptionMessages.MessageValidator_UnsupportedMessageType;
            var message = string.Format(messageFormat, messageType.GetType().FriendlyName(), supportedType.FriendlyName());
            return new ArgumentException(message, nameof(message));
        }
    }
}
