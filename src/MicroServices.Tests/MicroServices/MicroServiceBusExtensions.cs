using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kingo.Collections.Generic;

namespace Kingo.MicroServices
{
    public static class MicroServiceBusExtensions
    {
        #region [====== SendAsync ======]

        /// <summary>
        /// Sends all specified commands to the appropriate service(s).
        /// </summary>
        /// <param name="bus">The bus that will send the commands.</param>
        /// <param name="commands">A collection of commands to send.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="bus"/> or <paramref name="commands"/> is <c>null</c>.
        /// </exception>
        public static Task SendCommandsAsync(this IMicroServiceBus bus, params object[] commands) =>
            bus.SendCommandsAsync(commands.AsEnumerable());

        /// <summary>
        /// Sends all specified commands to the appropriate service(s).
        /// </summary>
        /// <param name="bus">The bus that will send the commands.</param>
        /// <param name="commands">A collection of commands to send.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="bus"/> or <paramref name="commands"/> is <c>null</c>.
        /// </exception>
        public static Task SendCommandsAsync(this IMicroServiceBus bus, IEnumerable<object> commands) =>
            bus.SendCommandsAsync(commands.WhereNotNull().Select(command => command.ToCommand()));

        #endregion

        #region [====== PublishAsync ======]

        /// <summary>
        /// Publishes all specified events.
        /// </summary>
        /// <param name="bus">The bus that will publish the events.</param>
        /// <param name="events">A collection of events to publish.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="bus"/> or <paramref name="events" /> is <c>null</c>.
        /// </exception>
        public static Task PublishEventsAsync(this IMicroServiceBus bus, params object[] events) =>
            bus.PublishEventsAsync(events.AsEnumerable());

        /// <summary>
        /// Publishes all specified events.
        /// </summary>
        /// <param name="bus">The bus that will publish the events.</param>
        /// <param name="events">A collection of events to publish.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="bus"/> or <paramref name="events" /> is <c>null</c>.
        /// </exception>
        public static Task PublishEventsAsync(this IMicroServiceBus bus, IEnumerable<object> events) =>
            bus.PublishEventsAsync(events.WhereNotNull().Select(@event => @event.ToEvent()));

        #endregion
    }
}
