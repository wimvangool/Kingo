using System;
using System.Threading.Tasks;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Represents a handler of different types of messages.
    /// </summary>
    public interface IMessageHandler
    {
        /// <summary>
        /// Handles the specified <paramref name="message"/> asynchronously and returns a stream of events
        /// as a result of the operation.
        /// </summary>
        /// <param name="message">The message to handle.</param>           
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        Task<MessageStream> HandleAsync<TMessage>(TMessage message);
    }
}
