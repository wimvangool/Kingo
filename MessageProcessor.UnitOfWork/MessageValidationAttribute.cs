using System;
using System.ComponentModel.DataAnnotations;

namespace YellowFlare.MessageProcessing
{
    /// <summary>
    /// Serves as a bass-class for all <see cref="ValidationAttribute">ValidationAttributes</see> that are
    /// used on messages.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The most prevalent added functionality to the basic <see cref="ValidationAttribute" /> class is that
    /// this attribute supports searching for Label-properties that can both be used as a means to display
    /// in the UI as well as use in the error-messages produced by the validation-attribute(s).
    /// </para>
    /// <para>
    /// To use this feature, a message should declare an extra string typed property with the same name as the
    /// property that is being validated, but suffixed with the word 'Label'. For example, suppose we have
    /// a property called <c>MyProperty</c>, then this attribute will look for the <c>MyPropertyLabel</c>-property
    /// to get the appropriate label.
    /// </para>
    /// </remarks>
    public abstract class MessageValidationAttribute : ValidationAttribute
    {
        /// <inheritdoc />
        protected sealed override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (validationContext == null)
            {
                return IsValid(value, null, null);
            }
            var propertyMapping = MessagePropertyLabelCollection.For(validationContext.ObjectInstance);
            var labelName = propertyMapping.FindLabelFor(validationContext.MemberName);

            return IsValid(value, validationContext, labelName);
        }

        /// <summary>
        /// Validates the specified value and returns a <see cref="ValidationResult" />.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="validationContext">The validation-context.</param>
        /// <param name="labelName">
        /// The name of the property as revoled by this attribute that can be used in the error-message.
        /// </param>
        /// <returns>The validation-result.</returns>
        protected abstract ValidationResult IsValid(object value, ValidationContext validationContext, string labelName);

        /// <summary>
        /// A validation-result that signals that validation had succeeded.
        /// </summary>
        protected static readonly ValidationResult Success = ValidationResult.Success;

        /// <summary>
        /// Resolves a service of the specified type using the <see cref="IServiceProvider" /> of the <paramref name="validationContext"/>.
        /// </summary>
        /// <typeparam name="TService">Type of service to resolve.</typeparam>
        /// <param name="validationContext">The context that is used to resolve the service.</param>
        /// <returns>
        /// The resolved service, or <c>null</c> if no service of the specified type was found.
        /// </returns>
        protected static TService Resolve<TService>(ValidationContext validationContext) where TService : class
        {
            return validationContext == null ? null : validationContext.GetService(typeof(TService)) as TService;
        }

        /// <summary>
        /// Creates and returns a new <see cref="ValidationResult" /> for the specified <paramref name="propertyName"/>
        /// containing the specified <paramref name="errorMessage"/>.
        /// </summary>
        /// <param name="errorMessage">The error-message.</param>
        /// <param name="propertyName">The property for which the validation failed.</param>
        /// <returns>
        /// A new <see cref="ValidationResult" /> for the specified <paramref name="propertyName"/>
        /// containing the specified <paramref name="errorMessage"/>.
        /// </returns>
        protected static ValidationResult CreateValidationError(string errorMessage, string propertyName)
        {
            return new ValidationResult(errorMessage, new[] { propertyName });
        }
    }
}
