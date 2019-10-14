using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kingo.MicroServices
{
    /// <summary>
    /// When implemented by a class, represents a service bus that can publish messages.
    /// </summary>
    public interface IMicroServiceBus
    {
        /// <summary>
        /// Sends all specified <paramref name="commands" /> to the appropriate service(s).
        /// </summary>
        /// <param name="commands">A collection of commands.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="commands"/> is <c>null</c>.
        /// </exception>
        Task SendCommandsAsync(IEnumerable<IMessageToDispatch> commands);

        /// <summary>
        /// Publishes all specified <paramref name="events" />.
        /// </summary>
        /// <param name="events">A collection of events.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="events"/> is <c>null</c>.
        /// </exception>
        Task PublishEventsAsync(IEnumerable<IMessageToDispatch> events);
    }
}
