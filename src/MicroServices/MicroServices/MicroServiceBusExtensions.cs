using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kingo.MicroServices
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
            var sendTask = bus.SendCommandsAsync(messages.Where(message => message.Kind == MessageKind.Command));
            var publishTask = bus.PublishEventsAsync(messages.Where(message => message.Kind == MessageKind.Event));
            return Task.WhenAll(sendTask, publishTask);
        }

        #endregion

        private static IMicroServiceBus NotNull(IMicroServiceBus bus) =>
            bus ?? throw new ArgumentNullException(nameof(bus));
    }
}
