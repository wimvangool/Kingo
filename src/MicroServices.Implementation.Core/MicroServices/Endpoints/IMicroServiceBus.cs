using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kingo.MicroServices.Endpoints
{
    /// <summary>
    /// When implemented by a class, represents a service bus that can publish messages.
    /// </summary>
    public interface IMicroServiceBus
    {
        /// <summary>
        /// Publishes all specified <paramref name="events" />.
        /// </summary>
        /// <param name="events">A collection of events.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="events"/> is <c>null</c>.
        /// </exception>
        Task PublishAsync(IEnumerable<object> events);

        /// <summary>
        /// Publishes the specified <paramref name="event" />.
        /// </summary>
        /// <param name="event">An event.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="event"/> is <c>null</c>.
        /// </exception> 
        Task PublishAsync(object @event);        
    }
}
