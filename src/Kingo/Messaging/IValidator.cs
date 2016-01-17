namespace Kingo.Messaging
{
    /// <summary>
    /// When implemented by a class, represents a validator for a specific instance.
    /// </summary>
    public interface IValidator
    {
        /// <summary>
        /// Validates all values of the specified <paramref name="instance"/> and returns all the validation-errors, if any.
        /// </summary>   
        /// <param name="instance">The instance to validate.</param>             
        /// <returns>
        /// A <see cref="ErrorInfo" /> instance that contains all validation errors, if any. If <paramref name="instance"/>
        /// is <c>null</c> or this validator does not support the specified <paramref name="instance"/>,
        /// an empty <see cref="ErrorInfo" /> is returned.
        /// </returns>          
        ErrorInfo Validate(object instance);

        /// <summary>
        /// Merges this validator with another validator.
        /// </summary>
        /// <param name="validator">Another validator.</param>
        /// <param name="haltOnFirstError">
        /// Indicates whether or not the composite validator should not invoke the specified <paramref name="validator"/>
        /// validator when the current validator already detected errors on an instance.
        /// </param>
        /// <returns>
        /// A validator that contains the validation-logic of both the current and the specified <paramref name="validator"/>.
        /// </returns>
        IValidator MergeWith(IValidator validator, bool haltOnFirstError = false);
    }
}
