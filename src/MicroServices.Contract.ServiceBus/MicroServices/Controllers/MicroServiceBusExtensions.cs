using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kingo.Collections.Generic;

namespace Kingo.MicroServices.Controllers
{
    /// <summary>
    /// Contains extension method for instance of type <see cref="IMicroServiceBus" />.
    /// </summary>
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
        public static Task SendAsync(this IMicroServiceBus bus, params object[] commands) =>
            bus.SendAsync(commands.AsEnumerable());

        /// <summary>
        /// Sends all specified commands to the appropriate service(s).
        /// </summary>
        /// <param name="bus">The bus that will send the commands.</param>
        /// <param name="commands">A collection of commands to send.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="bus"/> or <paramref name="commands"/> is <c>null</c>.
        /// </exception>
        public static Task SendAsync(this IMicroServiceBus bus, IEnumerable<object> commands) =>
            NotNull(bus).SendAsync(commands.WhereNotNull().Select(command => MessageToDispatch.CreateCommand(command)));

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
        public static Task PublishAsync(this IMicroServiceBus bus, params object[] events) =>
            bus.PublishAsync(events.AsEnumerable());

        /// <summary>
        /// Publishes all specified events.
        /// </summary>
        /// <param name="bus">The bus that will publish the events.</param>
        /// <param name="events">A collection of events to publish.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="bus"/> or <paramref name="events" /> is <c>null</c>.
        /// </exception>
        public static Task PublishAsync(this IMicroServiceBus bus, IEnumerable<object> events) =>
            NotNull(bus).PublishAsync(events.WhereNotNull().Select(@event => MessageToDispatch.CreateEvent(@event)));

        #endregion

        private static IMicroServiceBus NotNull(IMicroServiceBus bus) =>
            bus ?? throw new ArgumentNullException(nameof(bus));
    }
}
