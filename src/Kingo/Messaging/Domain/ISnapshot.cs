using System;
using System.Collections.Generic;

namespace Kingo.Messaging.Domain
{
    /// <summary>
    /// When implemented by a class, represents a snapshot of the state of an <see cref="IAggregateRoot" /> and serves as
    /// a factory to restore the aggregate in this state.
    /// </summary>
    public interface ISnapshot
    {
        /// <summary>
        /// Converts this snapshot to its latest version and returns the result. This method can be used to upgrade
        /// older versions of snapshots that have been retrieved from an event store to a version that is compatible
        /// with the latest implementation of the <see cref="IAggregateRoot"/>.
        /// </summary>
        /// <returns>The latest version of this snapshot.</returns>
        ISnapshot UpdateToLatestVersion();

        /// <summary>
        /// Restores and returns the aggregate based on this snapshot.
        /// </summary>
        /// <param name="events">
        /// If specified, this collection is used to replay all events on the restored aggregate.
        /// </param>
        /// <returns>The restored aggregate.</returns>
        /// <exception cref="ArgumentException">
        /// The restored aggregate does not recognise one of the specified events.
        /// </exception>
        IAggregateRoot RestoreAggregate(IEnumerable<IEvent> events = null);
    }
}
