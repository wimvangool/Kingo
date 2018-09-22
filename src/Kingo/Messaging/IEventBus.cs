using System;

namespace Kingo.Messaging
{
    /// <summary>
    /// Represents an event bus that can be used to publish events.
    /// </summary>
    public interface IEventBus
    {
        /// <summary>
        /// Publishes the specified <paramref name="event"/> to the bus.
        /// </summary>
        /// <typeparam name="TEvent">Type of event to publish.</typeparam>
        /// <param name="event">The event to publish.</param>       
        /// <exception cref="ArgumentNullException">
        /// <paramref name="event"/> is <c>null</c>.
        /// </exception>
        void Publish<TEvent>(TEvent @event);
    }
}
