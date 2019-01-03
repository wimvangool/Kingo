using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Kingo.MicroServices.Domain
{
    /// <summary>
    /// When implemented by a class, represents the root of an aggregate with a specific id and version.
    /// </summary>
    /// <typeparam name="TKey">Type of the identifier of this aggregate.</typeparam>
    /// <typeparam name="TVersion">Type of the version of this aggregate.</typeparam>
    public interface IAggregateRoot<TKey, TVersion> : IAggregateRoot<TKey>
        where TKey : struct, IEquatable<TKey>
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>
    {
        /// <summary>
        /// Returns the current version of the aggregate.
        /// </summary>
        TVersion Version
        {
            get;
        }

        #region [====== Events ======]

        /// <summary>
        /// Reloads the state of this aggregate by replaying all specified <paramref name="events" />.
        /// The events are already assumed to be up-to-date and sorted by version in ascending order.
        /// </summary>
        /// <param name="events">The events to replay.</param>        
        /// <exception cref="ArgumentNullException">
        /// <paramref name="events"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// This aggregate does not recognize one of the events.
        /// </exception>
        void LoadFromHistory(IEnumerable<ISnapshotOrEvent<TKey, TVersion>> events);        

        #endregion

        #region [====== TakeSnapshot ======]               

        /// <summary>
        /// Creates and returns a snapshot of the current state of this aggregate.
        /// </summary>
        /// <returns>A snapshot of the current state of this aggregate.</returns>
        /// <exception cref="NotSupportedException">
        /// This aggregate does not support snapshots.
        /// </exception>
        ISnapshotOrEvent<TKey, TVersion> TakeSnapshot();

        /// <summary>
        /// Commits all changes of this aggregate and returns all events that were published by this aggregate
        /// since the last commit.
        /// </summary>
        /// <returns>All events that were published since the last commit.</returns>
        IReadOnlyList<ISnapshotOrEvent<TKey, TVersion>> Commit();

        #endregion
    }
}
