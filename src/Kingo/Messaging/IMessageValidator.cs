namespace Kingo.Messaging
{
    /// <summary>
    /// When implemented by a class, represents a validator for a specific instance.
    /// </summary>
    public interface IMessageValidator
    {
        /// <summary>
        /// Validates all values of the specified <paramref name="message"/> and returns all the validation-errors, if any.
        /// </summary>   
        /// <param name="message">The instance to validate.</param>             
        /// <returns>
        /// A <see cref="ErrorInfo" /> instance that contains all validation errors, if any. If <paramref name="message"/>
        /// is <c>null</c> or this validator does not support the specified <paramref name="message"/>,
        /// an empty <see cref="ErrorInfo" /> is returned.
        /// </returns>          
        ErrorInfo Validate(object message);

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
        IMessageValidator MergeWith(IMessageValidator validator, bool haltOnFirstError = false);
    }
}
