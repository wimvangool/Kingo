using System;
using Kingo.Reflection;

namespace Kingo.Constraints
{
    /// <summary>
    /// Serves as a base-class for all constraints that support validation of a specific type.
    /// </summary>
    /// <typeparam name="TValue">The supported type.</typeparam>
    public abstract class Constraint<TValue> : Constraint
    {
        /// <inheritdoc />
        public override bool IsValid(object value) =>
            IsValid(Cast(value ?? throw new ArgumentNullException(nameof(value))));

        /// <summary>
        /// Validates the specified <paramref name="value"/> and returns the result.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <returns>
        /// <c>true</c> if the specified <paramref name="value"/> satisfies this constraint; otherwise <c>false</c>.
        /// </returns>
        public abstract bool IsValid(TValue value);

        private TValue Cast(object value)
        {
            try
            {
                return (TValue) value;
            }
            catch (InvalidCastException exception)
            {
                throw NewUnsupportedTypeException(value, exception);
            }
        }

        private Exception NewUnsupportedTypeException(object value, Exception innerException)
        {
            var messageFormat = ExceptionMessages.Constraint_UnsupportedType;
            var message = string.Format(messageFormat, value.GetType().FriendlyName(), GetType().FriendlyName(), typeof(TValue).FriendlyName());
            return new ArgumentException(message, nameof(value), innerException);
        }
    }
}
