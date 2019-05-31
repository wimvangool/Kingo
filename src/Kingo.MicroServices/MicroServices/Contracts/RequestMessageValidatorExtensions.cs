using System;

namespace Kingo.MicroServices.Contracts
{
    /// <summary>
    /// Contains extension methods for the <see cref="IRequestMessageValidator" /> interface.
    /// </summary>
    public static class RequestMessageValidatorExtensions
    {
        /// <summary>
        /// Appends the specified <paramref name="validationMethod"/> to this validator and returns the composite validator.
        /// </summary>      
        /// <param name="validator">A message validator.</param>
        /// <param name="validationMethod">The validation delegate to append.</param>
        /// <returns>The composite validator.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="validationMethod"/> is <c>null</c>.
        /// </exception>
        public static IRequestMessageValidator Append<TMessage>(this IRequestMessageValidator validator, Func<TMessage, bool, ErrorInfo> validationMethod) =>
            validator.Append(new DelegateValidator<TMessage>(validationMethod));
    }
}
