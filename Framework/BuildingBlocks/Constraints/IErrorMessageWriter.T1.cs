using System;

namespace Kingo.BuildingBlocks.Constraints
{
    /// <summary>
    /// When implemented by a class, represents a producer of error messages.
    /// </summary>
    /// <typeparam name="TMessage">Type of the instances for which the error messages are produced.</typeparam>
    public interface IErrorMessageWriter<in TMessage>
    {
        /// <summary>
        /// Validates the specified <paramref name="message"/> and writes all error messages to the specified <paramref name="reader"/>.
        /// </summary>
        /// <param name="message">The instance that is validated.</param>
        /// <param name="reader">A reader or consumer of all error messages.</param> 
        /// <returns><c>true</c> if any errors were detected; otherwise <c>false</c>.</returns>      
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> or <paramref name="reader"/> is <c>null</c>.
        /// </exception>        
        bool WriteErrorMessages(TMessage message, IErrorMessageReader reader);
    }
}
