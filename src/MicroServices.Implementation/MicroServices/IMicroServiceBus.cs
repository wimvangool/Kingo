using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kingo.MicroServices
{
    /// <summary>
    /// When implemented by a class, represents a service bus that can route/transmit messages
    /// to all services in the service-landscape.
    /// </summary>
    public interface IMicroServiceBus
    {
        /// <summary>
        /// Sends out all specified <paramref name="messages" />.
        /// </summary>
        /// <param name="messages">A collection of messages to send.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="messages"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The bus is currently not able to send any messages.
        /// </exception>
        Task SendAsync(params IMessage[] messages) =>
            SendAsync(messages.AsEnumerable());

        /// <summary>
        /// Sends out all specified <paramref name="messages" />.
        /// </summary>
        /// <param name="messages">A collection of messages to send.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="messages"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The bus is currently not able to send any messages.
        /// </exception>
        Task SendAsync(IEnumerable<IMessage> messages);
    }
}
