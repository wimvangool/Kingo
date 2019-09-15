using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kingo.Collections.Generic;

namespace Kingo.MicroServices.Controllers
{
    /// <summary>
    /// Contains extension method for instance of type <see cref="IMicroServiceBus" />.
    /// </summary>
    public static class MicroServiceBusExtensions
    {
        #region [====== DispatchAsync ======]

        /// <summary>
        /// Sends all commands and publishes all events that are part of the specified
        /// <paramref name="messages"/> collection.
        /// </summary>
        /// <param name="bus">The bus that will dispatch the messages.</param>
        /// <param name="messages">A collection of commands and events to send and publish.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="bus"/> or <paramref name="messages"/> is <c>null</c>.
        /// </exception>
        public static Task DispatchAsync(this IMicroServiceBus bus, IEnumerable<MessageToDispatch> messages) =>
            NotNull(bus).DispatchAsync(messages.ToArray());

        private static Task DispatchAsync(this IMicroServiceBus bus, IReadOnlyCollection<MessageToDispatch> messages)
        {
            var sendTask = bus.SendAsync(messages.Where(message => message.Kind == MessageKind.Command));
            var publishTask = bus.PublishAsync(messages.Where(message => message.Kind == MessageKind.Event));
            return Task.WhenAll(sendTask, publishTask);
        }

        #endregion

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
