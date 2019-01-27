using System;
using System.Threading.Tasks;

namespace Kingo.MicroServices
{
    /// <summary>
    /// When implemented by a class, represents a processor that can process a specific message.
    /// </summary>
    /// <typeparam name="TMessage">Type of the message that can be processed.</typeparam>
    public interface IMessageProcessor<TMessage>
    {        
        /// <summary>
        /// Processes the specified <paramref name="message" />.
        /// </summary>
        /// <param name="message">The message to handle.</param>
        /// <param name="handler">Optional handler to handle the message inside the processor.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        Task HandleAsync(TMessage message, IMessageHandler<TMessage> handler = null);
    }
}
