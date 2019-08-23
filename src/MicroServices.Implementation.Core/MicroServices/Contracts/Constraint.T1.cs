using System;
using System.ComponentModel.DataAnnotations;
using Kingo.Reflection;

namespace Kingo.MicroServices.Contracts
{
    /// <summary>
    /// Serves as a base-class for all constraints that support validation of a specific type.
    /// </summary>
    /// <typeparam name="TValue">The supported type.</typeparam>
    public abstract class Constraint<TValue> : Constraint
    {
        /// <inheritdoc />
        public override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            if (validationContext == null)
            {
                throw new ArgumentNullException(nameof(validationContext));
            }
            return IsValid(Cast(value), validationContext);
        }

        /// <summary>
        /// Validates the specified <paramref name="value"/> and returns the result.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="validationContext">Context of the value to validate.</param>
        /// <returns>
        /// <see cref="ValidationResult.Success"/> if <paramref name="value"/> is valid; otherwise
        /// it will return a result with the appropriate error message.
        /// </returns>
        public abstract ValidationResult IsValid(TValue value, ValidationContext validationContext);

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
