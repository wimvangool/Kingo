using System;

namespace Kingo.MicroServices.Validation
{      
    internal sealed class NullValidator : IRequestMessageValidator
    {
        public override string ToString() =>
            string.Empty;

        public IRequestMessageValidator Append<TMessage>(Func<TMessage, bool, ErrorInfo> validator) =>
            Append(new DelegateValidator<TMessage>(validator));

        public IRequestMessageValidator Append<TMessage>(IRequestMessageValidator<TMessage> validator) =>
            new RequestRequestMessageValidator<TMessage>(validator);

        /// <inheritdoc />
        public ErrorInfo Validate(object message, bool haltOnFirstError = false) =>
            ErrorInfo.Empty;        
    }
}
