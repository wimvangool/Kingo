using System;
using System.Threading.Tasks;

namespace Kingo.Messaging
{
    /// <summary>
    /// Represents a handler of different message types.
    /// </summary>
    public interface IMessageHandler
    {
        /// <summary>
        /// Handles the specified <paramref name="message"/> asynchronously.
        /// If <paramref name="handler"/> is specified, this handler is expected to handle the message with this handler.
        /// </summary>
        /// <param name="message">The message to handle.</param>
        /// <param name="handler">Optional handler with which the message is to be handled.</param>        
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        Task HandleAsync<TMessage>(TMessage message, IMessageHandler<TMessage> handler = null);
    }
}
