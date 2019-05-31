using System;

namespace Kingo.MicroServices.Contracts
{
    /// <summary>
    /// When implemented by a class, represents a validator for a specific message.
    /// </summary>
    /// <typeparam name="TMessage">Type of the message this validator can validate.</typeparam>
    public interface IRequestMessageValidator<in TMessage>
    {
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
        ErrorInfo Validate(TMessage message, bool haltOnFirstError = false);        
    }
}
