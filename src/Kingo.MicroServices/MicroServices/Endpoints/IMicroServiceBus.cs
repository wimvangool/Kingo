using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Kingo.MicroServices.Endpoints
{
    /// <summary>
    /// When implemented by a class, represents a service bus that can publish messages.
    /// </summary>
    public interface IMicroServiceBus : IHostedService
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
