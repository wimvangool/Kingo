using System;

namespace Kingo.Messaging.Domain
{
    /// <summary>
    /// Represents the argument of the <see cref="IAggregateRoot.EventPublished" /> event.
    /// </summary>
    /// <typeparam name="TEvent">Type of the event that was published.</typeparam>
    public sealed class EventPublishedEventArgs<TEvent> : EventPublishedEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventPublishedEventArgs" /> class.
        /// </summary>
        /// <param name="event">The event that was published.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="event"/> is <c>null</c>.
        /// </exception>
        public EventPublishedEventArgs(TEvent @event)
        {
            if (ReferenceEquals(@event, null))
            {
                throw new ArgumentNullException(nameof(@event));
            }
            Event = @event;
        }

        /// <summary>
        /// The event that was published.
        /// </summary>
        public TEvent Event
        {
            get;
        }

        /// <inheritdoc />
        public override void PublishEvent(IEventStream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }
            stream.Publish(Event);
        }
    }
}
