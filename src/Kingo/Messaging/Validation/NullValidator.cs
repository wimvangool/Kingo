namespace Kingo.Messaging.Validation
{
    /// <summary>
    /// Represents a validator that performs no validation at all.
    /// </summary>    
    public sealed class NullValidator : IMessageValidator<object>
    {
        /// <inheritdoc />
        public ErrorInfo Validate(object message, bool haltOnFirstError = false) =>
            ErrorInfo.Empty;        
    }
}
