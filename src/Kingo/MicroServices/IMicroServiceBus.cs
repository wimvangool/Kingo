using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kingo.MicroServices
{
    /// <summary>
    /// When implemented by a class, represents a service bus to which messages can be published by a <see cref="MicroProcessor" />.
    /// </summary>
    public interface IMicroServiceBus
    {
        /// <summary>
        /// Publishes all specified <paramref name="messages" />.
        /// </summary>
        /// <param name="messages">The messages to publish.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="messages"/> is <c>null</c>.
        /// </exception>
        Task PublishAsync(IEnumerable<object> messages);

        /// <summary>
        /// Publishes the specified <paramref name="message" />.
        /// </summary>
        /// <param name="message">The message to publish.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception> 
        Task PublishAsync(object message);        
    }
}
