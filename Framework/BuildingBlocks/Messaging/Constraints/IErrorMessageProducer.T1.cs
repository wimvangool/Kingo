using System;

namespace Kingo.BuildingBlocks.Messaging.Constraints
{
    /// <summary>
    /// When implemented by a class, represents a producer of error messages.
    /// </summary>
    /// <typeparam name="TMessage">Type of the object the error messages are produced for.</typeparam>
    public interface IErrorMessageProducer<in TMessage>
    {
        /// <summary>
        /// Determines whether <paramref name="message"/> is in error and adds all error messages to the specified <paramref name="consumer"/>.
        /// </summary>
        /// <param name="message">The instance for which the error messages are produced.</param>
        /// <param name="consumer">A consumer of error messages.</param>   
        /// <param name="formatProvider">A <see cref="IFormatProvider" /> that is used for placeholders that define a specific format.</param>
        /// <returns><c>true</c> if any error messages were added to the specified <paramref name="consumer"/>; otherwise <c>false</c>.</returns>    
        /// <exception cref="ArgumentNullException">
        /// <paramref name="consumer"/> is <c>null</c>.
        /// </exception>
        bool HasErrors(TMessage message, IErrorMessageConsumer consumer, IFormatProvider formatProvider = null);
    }
}
