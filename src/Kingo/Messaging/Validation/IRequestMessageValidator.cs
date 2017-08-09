using System;

namespace Kingo.Messaging.Validation
{
    /// <summary>
    /// When implemented by a class, represents a validator of a specific message type.
    /// </summary>
    public interface IRequestMessageValidator
    {       
        /// <summary>
        /// Appends the specified <paramref name="validator"/> to this validator and returns the composite validator.
        /// </summary>      
        /// <param name="validator">The validator to append.</param>
        /// <returns>The composite validator.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="validator"/> is <c>null</c>.
        /// </exception>
        IRequestMessageValidator Append<TMessage>(IRequestMessageValidator<TMessage> validator);

        /// <summary>
        /// Validates all properties and/or fields of the specified <paramref name="message"/> and returns all the validation-errors, if any.
        /// </summary>   
        /// <param name="message">The message to validate.</param>   
        /// <param name="haltOnFirstError">
        /// Indicates whether or not this method should return as soon as the first validation error is found.
        /// </param>          
        /// <returns>
        /// A collection of validation errors. If <paramref name="haltOnFirstError"/> is <c>true</c>, the returned
        /// collection contains a minimum of validation errors. If <paramref name="haltOnFirstError" /> is <c>false</c>,
        /// all validation errors are returned.
        /// </returns>  
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The type of <paramref name="message"/> is not supported by this validator.
        /// </exception>
        ErrorInfo Validate(object message, bool haltOnFirstError = false);
    }
}
