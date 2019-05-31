using System;
using System.Threading.Tasks;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Represents a processor of different types of messages.
    /// </summary>
    public interface IMessageProcessor
    {
        /// <summary>
        /// Processes the specified message asynchronously.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message.</typeparam>        
        /// <param name="message">Message to handle.</param>        
        /// <returns>
        /// The result of the operation, which includes all published events and the number of message handlers that were invoked.
        /// </returns> 
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="MicroProcessorException">
        /// Something went wrong while handling the message.
        /// </exception> 
        Task<HandleAsyncResult> HandleAsync<TMessage>(TMessage message);
    }
}
