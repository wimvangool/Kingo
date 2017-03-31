using System;
using System.Collections.Generic;

namespace Kingo.Messaging.Domain
{
    /// <summary>
    /// Serves as a base-class implementation of the <see cref="ISnapshot"/> interface.
    /// </summary>
    [Serializable]
    public abstract class Snapshot : ISnapshot
    {
        ISnapshot ISnapshot.UpdateToLatestVersion() =>
            UpgradeToLatestVersion();

        /// <summary>
        /// Converts this snapshot to its latest version and returns the result. This method can be used to upgrade
        /// older versions of snapshots that have been retrieved from an event store to a version that is compatible
        /// with the latest implementation of the <see cref="IAggregateRoot"/>.
        /// </summary>
        /// <returns>The latest version of this snapshot.</returns>
        protected virtual ISnapshot UpgradeToLatestVersion() =>
            this;

        IAggregateRoot ISnapshot.RestoreAggregate(IEnumerable<IEvent> events)
        {
            var aggregate = RestoreAggregate();

            if (events != null)
            {
                aggregate.LoadFromHistory(events);
            }
            return aggregate;
        }            

        /// <summary>
        /// When implemented, restored the aggregate in the same state it had been in when this snapshot was created.
        /// </summary>
        /// <returns>A new aggregate instance in the state it had been in when this snapshot was created.</returns>
        protected abstract IAggregateRoot RestoreAggregate();

        /// <inheritdoc />
        public override string ToString() =>
            GetType().FriendlyName();
    }
}
