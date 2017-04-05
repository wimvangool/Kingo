using System;
using System.Collections.Generic;

namespace Kingo.Messaging.Domain
{
    /// <summary>
    /// When implemented by a class, represents the root of an aggregate.
    /// </summary>
    public interface IAggregateRoot
    {
        #region [====== LoadFromHistory & TakeSnapshot ======]

        /// <summary>
        /// Reloads the state of this aggregate by replaying all specified <paramref name="events" />.
        /// </summary>
        /// <param name="events">The events to replay.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="events"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// This aggregate does not recognise one of the events.
        /// </exception>
        void LoadFromHistory(IEnumerable<IEvent> events);

        /// <summary>
        /// Creates and returns a snapshot of the current state of this aggregate.
        /// </summary>
        /// <returns>A snapshot of the current state of this aggregate.</returns>
        ISnapshot TakeSnapshot();

        #endregion

        #region [====== EventPublished & FlushEvents() ======]

        /// <summary>
        /// This event is raised each time this aggregate publishes a new event.
        /// </summary>
        event EventHandler<EventPublishedEventArgs> EventPublished;

        /// <summary>
        /// Indicates whether or not this aggregate has published any events that haven't been flushed yet.
        /// </summary>
        bool HasPendingEvents
        {
            get;
        }

        /// <summary>
        /// Returns all events that were published by this aggregate since it was instantiated or retrieved from a data store
        /// and empties the aggregate's internal event buffer.
        /// </summary>
        IEnumerable<IEvent> FlushEvents();

        /// <summary>
        /// Notifies the aggregate that it was removed from the repository. This method can be used
        /// to publish some last minute events representing the removal of this aggregate and the end
        /// of its lifetime.
        /// </summary>
        void NotifyRemoved();

        #endregion        
    }
}
