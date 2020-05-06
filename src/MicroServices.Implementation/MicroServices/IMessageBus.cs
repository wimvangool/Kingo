using System;
using System.Collections.Generic;

namespace Kingo.MicroServices
{
    /// <summary>
    /// When implemented by a class, represent a bus to which events can be published.
    /// </summary>
    public interface IMessageBus : IReadOnlyList<IMessage>
    {
        #region [====== Send ======]

        /// <summary>
        /// Schedules a command to be sent after a specific period or delay.
        /// </summary>
        /// <param name="command">The command to send.</param>
        /// <param name="delay">
        /// The period that determines when the command should be delivered relative to the current date and time.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="command"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="delay"/> is a negative value.
        /// </exception>
        void Send(object command, TimeSpan delay);

        /// <summary>
        /// Schedules the specified <paramref name="command"/> to be sent on the service-bus.
        /// </summary>
        /// <param name="command">The command to send.</param>
        /// <param name="deliveryTime">
        /// If specified, indicates at what time the command should be sent on the service-bus.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="command"/> is <c>null</c>.
        /// </exception>
        void Send(object command, DateTimeOffset? deliveryTime = null);

        #endregion

        #region [====== Publish ======]

        /// <summary>
        /// Schedules an event to be sent after a specific period or delay.
        /// </summary>
        /// <param name="event">The event to publish.</param>
        /// <param name="delay">
        /// The period that determines when the event should be delivered relative to the current date and time.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="event"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="delay"/> is a negative value.
        /// </exception>
        void Publish(object @event, TimeSpan delay);

        /// <summary>
        /// Schedules the specified <paramref name="event"/> to be published on the service-bus.
        /// </summary>
        /// <param name="event">The event to publish.</param>
        /// <param name="deliveryTime">
        /// If specified, indicates at what time the event should be published on the service-bus.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="event"/> is <c>null</c>.
        /// </exception>
        void Publish(object @event, DateTimeOffset? deliveryTime = null);

        #endregion
    }
}
