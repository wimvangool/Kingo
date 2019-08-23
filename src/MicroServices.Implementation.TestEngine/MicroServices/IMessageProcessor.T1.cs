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
        /// Executes a command with a specific message handler.
        /// </summary>
        /// <param name="messageHandler">Message handler that will handle the specified <paramref name="message"/>.</param>
        /// <param name="message">The command to execute.</param>        
        /// <exception cref="ArgumentNullException">
        /// <paramref name="messageHandler"/> or <paramref name="message"/> is <c>null</c>.
        /// </exception>
        Task ExecuteCommandAsync(IMessageHandler<TMessage> messageHandler, TMessage message);

        /// <summary>
        /// Handles an event with a specific message handler.
        /// </summary>
        /// <param name="messageHandler">Message handler that will handle the specified <paramref name="message"/>.</param>
        /// <param name="message">The event to handle.</param>        
        /// <exception cref="ArgumentNullException">
        /// <paramref name="messageHandler"/> or <paramref name="message"/> is <c>null</c>.
        /// </exception>
        Task HandleEventAsync(IMessageHandler<TMessage> messageHandler, TMessage message);
    }
}
