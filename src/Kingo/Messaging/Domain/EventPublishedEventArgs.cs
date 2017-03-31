using System;

namespace Kingo.Messaging.Domain
{
    /// <summary>
    /// Represents the argument of the <see cref="IAggregateRoot.EventPublished" /> event.
    /// </summary>
    public abstract class EventPublishedEventArgs : EventArgs
    {
        /// <summary>
        /// Publishes the events that was published by the aggregate to the specified <paramref name="stream"/>.
        /// </summary>
        /// <param name="stream">The stream to publish to event to.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="stream"/> is <c>null</c>.
        /// </exception>
        public abstract void PublishEvent(IEventStream stream);
    }
}
