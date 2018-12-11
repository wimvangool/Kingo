using System;

namespace Kingo.MicroServices.Domain
{
    /// <summary>
    /// Serves as a base-class implementation of the <see cref="ISnapshot"/> interface.
    /// </summary>
    /// <typeparam name="TKey">Type of the identifier of the aggregate.</typeparam>
    /// <typeparam name="TVersion">Type of the version of the aggregate.</typeparam>
    [Serializable]
    public abstract class Snapshot<TKey, TVersion> : AggregateDataObject<TKey, TVersion>, ISnapshot<TKey, TVersion>
        where TKey : struct, IEquatable<TKey>
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Snapshot{T, S}" /> class.
        /// </summary>
        /// <param name="id">Identifier of the aggregate.</param>
        /// <param name="version">Version of the aggregate.</param>
        protected Snapshot(TKey id = default(TKey), TVersion version = default(TVersion)) :
            base(id, version) { }        

        ISnapshot ISnapshot.UpdateToLatestVersion() =>
            UpdateToLatestVersion();

        /// <summary>
        /// Converts this snapshot to its latest version and returns the result. This method can be used to upgrade
        /// older versions of snapshots that have been retrieved from an event store to a version that is compatible
        /// with the latest implementation of the <see cref="IAggregateRoot"/>.
        /// </summary>
        /// <returns>The latest version of this snapshot.</returns>
        protected virtual ISnapshot UpdateToLatestVersion() =>
            this;

        internal override object RestoreAggregate(Type aggregateType) =>
            AggregateRootFactory.RestoreAggregateFromSnapshot(aggregateType, this);
    }
}
