using System;

namespace Kingo
{
    /// <summary>
    /// Represents a validator that performs no validation at all.
    /// </summary>    
    public sealed class NullValidator : IValidator
    {
        /// <inheritdoc />
        public ErrorInfo Validate(object instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }
            return ErrorInfo.Empty;
        }

        /// <inheritdoc />
        public IValidator MergeWith(IValidator validator, bool haltOnFirstError = false)
        {
            return validator ?? this;
        }
    }
}
