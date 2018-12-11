using System;
using System.Collections.Generic;

namespace Kingo.MicroServices.Domain
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
        /// This aggregate does not recognize one of the events.
        /// </exception>
        void LoadFromHistory(IEnumerable<IEvent> events);

        /// <summary>
        /// Creates and returns a snapshot of the current state of this aggregate.
        /// </summary>
        /// <returns>A snapshot of the current state of this aggregate.</returns>
        ISnapshot TakeSnapshot();

        #endregion

        #region [====== EventPublished & Commit ======]

        /// <summary>
        /// This event is raised when a new event is published on this bus.
        /// </summary>
        event EventHandler<EventPublishedEventArgs> EventPublished;

        /// <summary>
        /// Returns all events that have been published by this aggregate since the last commit.
        /// </summary>
        IReadOnlyList<IEvent> Events
        {
            get;
        }

        /// <summary>
        /// Commits all changes and returns all events that were published since the last commit.
        /// </summary>
        /// <returns></returns>
        IReadOnlyList<IEvent> Commit();

        #endregion

        #region [====== NotifyRemoved ======]

        /// <summary>
        /// Notifies the aggregate that it was removed from the repository. This method can be used
        /// to publish some last minute events representing the removal of this aggregate and the end
        /// of its lifetime.
        /// </summary>
        void NotifyRemoved();

        #endregion
    }
}
