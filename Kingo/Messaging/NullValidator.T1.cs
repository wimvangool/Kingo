using System;

namespace Kingo.Messaging
{
    /// <summary>
    /// Represents a validator that performs no validation at all.
    /// </summary>
    /// <typeparam name="T">Type of the instance to validate.</typeparam>
    public sealed class NullValidator<T> : IValidator<T>
    {
        /// <inheritdoc />
        public ErrorInfo Validate(T instance)
        {
            if (ReferenceEquals(instance, null))
            {
                throw new ArgumentNullException("instance");
            }
            return ErrorInfo.Empty;
        }
        
        IValidator<T> IValidator<T>.Append(IValidator<T> validator, bool haltOnFirstError)
        {
            if (validator == null)
            {
                throw new ArgumentNullException("validator");
            }            
            return validator;
        }
    }
}
