using System;

namespace Kingo.Messaging.Domain
{
    /// <summary>
    /// Contains the event that was published by an <see cref="IAggregateRoot" />.
    /// </summary>
    public sealed class EventPublishedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventPublishedEventArgs" /> class.
        /// </summary>
        /// <param name="event">The event that was published.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="@event"/> is <c>null</c>.
        /// </exception>
        public EventPublishedEventArgs(IEvent @event)
        {            
            Event = @event ?? throw new ArgumentNullException(nameof(@event));
        }

        /// <summary>
        /// The event that was published.
        /// </summary>
        public IEvent Event
        {
            get;
        }
    }
}
