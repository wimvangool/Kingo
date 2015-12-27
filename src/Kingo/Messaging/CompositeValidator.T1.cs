using System;

namespace Kingo.Messaging
{
    /// <summary>
    /// Represents a validator that is composed of two other validators.
    /// </summary>
    /// <typeparam name="T">Type of the instance to validate.</typeparam>
    public sealed class CompositeValidator<T> : IValidator<T>
    {
        private readonly IValidator<T> _left;
        private readonly IValidator<T> _right;
        private readonly bool _haltOnFirstError;

        private CompositeValidator(IValidator<T> left, IValidator<T> right, bool haltOnFirstError)
        {
            _left = left;
            _right = right;
            _haltOnFirstError = haltOnFirstError;
        }

        /// <inheritdoc />
        public ErrorInfo Validate(T instance)
        {
            var errorInfo = _left.Validate(instance);
            if (errorInfo.HasErrors && _haltOnFirstError)
            {
                return errorInfo;
            }
            return ErrorInfo.Merge(errorInfo, _right.Validate(instance));
        }

        IValidator<T> IValidator<T>.Append(IValidator<T> validator, bool haltOnFirstError)
        {
            return Append(this, validator, haltOnFirstError);
        }

        /// <summary>
        /// Combines the two specified validators into a single validator.
        /// </summary>
        /// <param name="left">The first validator to use.</param>
        /// <param name="right">The second validator to use.</param>
        /// <param name="haltOnFirstError">
        /// Indicates whether or not the composite validator should not invoke the specified <paramref name="right"/>
        /// validator when the <paramref name="left"/> validator already detected errors on an instance.
        /// </param>
        /// <returns>A composite validator.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="left"/> or <paramref name="right"/> is <c>null</c>.
        /// </exception>
        public static IValidator<T> Append(IValidator<T> left, IValidator<T> right, bool haltOnFirstError = false)
        {
            if (left == null)
            {
                throw new ArgumentNullException("left");
            }
            if (right == null)
            {
                throw new ArgumentNullException("right");
            }
            if (left == right)
            {
                return left;
            }
            return new CompositeValidator<T>(left, right, haltOnFirstError);
        }
    }
}
