using System;

namespace Kingo.Messaging
{
    /// <summary>
    /// Represents a validator that performs no validation at all.
    /// </summary>    
    public sealed class NullValidator : IMessageValidator
    {
        /// <inheritdoc />
        public ErrorInfo Validate(object message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }
            return ErrorInfo.Empty;
        }

        /// <inheritdoc />
        public IMessageValidator MergeWith(IMessageValidator validator, bool haltOnFirstError = false)
        {
            return validator ?? this;
        }
    }
}
