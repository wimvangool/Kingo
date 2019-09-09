using System;
using System.Collections.Generic;
using System.Text;
using Kingo.Clocks;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Contains extension-methods for instances of type <see cref="IMessageBus" />.
    /// </summary>
    public static class MessageBusExtensions
    {
        /// <summary>
        /// Schedules a command for a specific time relative to the current date and time.
        /// </summary>
        /// <param name="bus">A message bus.</param>
        /// <param name="command">The command to send.</param>
        /// <param name="delta">
        /// The period that determines when the command should be delivered relative to the current date and time.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="bus"/> or <paramref name="command"/> is <c>null</c>.
        /// </exception>
        public static void Send(this IMessageBus bus, object command, TimeSpan delta) =>
            NotNull(bus).Send(command, CalculateDeliveryTimeUtc(delta));

        /// <summary>
        /// Schedules an event for a specific time relative to the current date and time.
        /// </summary>
        /// <param name="bus">A message bus.</param>
        /// <param name="event">The event to publish.</param>
        /// <param name="delta">
        /// The period that determines when the event should be delivered relative to the current date and time.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="bus"/> or <paramref name="event"/> is <c>null</c>.
        /// </exception>
        public static void Publish(this IMessageBus bus, object @event, TimeSpan delta) =>
            NotNull(bus).Publish(@event, CalculateDeliveryTimeUtc(delta));

        private static IMessageBus NotNull(IMessageBus bus) =>
            bus ?? throw new ArgumentNullException(nameof(bus));

        private static DateTimeOffset CalculateDeliveryTimeUtc(TimeSpan delta) =>
            Clock.Current.UtcDateAndTime().Add(delta);
    }
}
