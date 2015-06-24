namespace System.ComponentModel.DataAnnotations
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
    public abstract class MemberValidationAttribute : ValidationAttribute
    {
        /// <inheritdoc />
        protected virtual ValidationResult InvalidResult(ValidationContext validationContext, string errorMessage)
        {
            if (validationContext == null)
            {
                return new ValidationResult(errorMessage);
            }
            var labelProvider = RequestMessageLabelProvider.For(validationContext.ObjectInstance);
            var label = labelProvider.FindLabelFor(validationContext.MemberName);

            return new ValidationResult(string.Format(errorMessage, label), new [] { validationContext.MemberName });
        }

        /// <summary>
        /// The <see cref="ValidationResult" /> that indicates validation succeeded.
        /// </summary>
        protected static readonly ValidationResult ValidResult = ValidationResult.Success;

        /// <summary>
        /// Attempts to resolve a particular service using the specified <paramref name="serviceProvider"/>.
        /// </summary>
        /// <typeparam name="TService">Type of the service to resolve.</typeparam>
        /// <param name="serviceProvider">The provider that is used to resolve the service.</param>
        /// <param name="service">
        /// When the service was resolved, this parameter will refer to that service when the method has completed.
        /// </param>
        /// <returns>
        /// <c>true</c> is the service was resolved; otherwise <c>false</c>.
        /// </returns>
        protected static bool TryResolve<TService>(IServiceProvider serviceProvider, out TService service) where TService : class
        {
            return (service = Resolve<TService>(serviceProvider)) != null;
        }

        /// <summary>
        /// Resolves a service of the specified type using the <see cref="IServiceProvider" /> of the <paramref name="serviceProvider"/>.
        /// </summary>
        /// <typeparam name="TService">Type of the service to resolve.</typeparam>
        /// <param name="serviceProvider">The provider that is used to resolve the service.</param>
        /// <returns>
        /// The resolved service, or <c>null</c> if no service of the specified type was found.
        /// </returns>
        protected static TService Resolve<TService>(IServiceProvider serviceProvider) where TService : class
        {
            return serviceProvider == null ? null : serviceProvider.GetService(typeof(TService)) as TService;
        }        
    }
}
