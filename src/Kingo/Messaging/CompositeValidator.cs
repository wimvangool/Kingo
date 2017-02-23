namespace Kingo.Messaging
{
    /// <summary>
    /// Represents a validator that is composed of two other validators.
    /// </summary>    
    public sealed class CompositeValidator : IMessageValidator
    {
        private readonly IMessageValidator _left;
        private readonly IMessageValidator _right;
        private readonly bool _haltOnFirstError;

        private CompositeValidator(IMessageValidator left, IMessageValidator right, bool haltOnFirstError)
        {
            _left = left;
            _right = right;
            _haltOnFirstError = haltOnFirstError;
        }

        /// <inheritdoc />
        public ErrorInfo Validate(object message)
        {
            var errorInfo = _left.Validate(message);
            if (errorInfo.HasErrors && _haltOnFirstError)
            {
                return errorInfo;
            }
            return ErrorInfo.Merge(errorInfo, _right.Validate(message));
        }

        /// <inheritdoc />
        public IMessageValidator MergeWith(IMessageValidator validator, bool haltOnFirstError = false)
        {
            return Merge(this, validator, haltOnFirstError);
        }

        /// <summary>
        /// Merges two validators into one.
        /// </summary>
        /// <param name="left">The first validator to merge.</param>
        /// <param name="right">The second validator to merge.</param>
        /// <param name="haltOnFirstError">
        /// Indicates whether or not the composite validator should not invoke the specified <paramref name="right"/>
        /// validator when the <paramref name="left"/> validator already detected errors on an instance.
        /// </param>
        /// <returns>A composite validator.</returns>        
        public static IMessageValidator Merge(IMessageValidator left, IMessageValidator right, bool haltOnFirstError = false)
        {
            if (left == null)
            {
                return right ?? new NullValidator();
            }
            if (left == right || right == null)
            {
                return left;
            }            
            return new CompositeValidator(left, right, haltOnFirstError);
        }        
    }
}
