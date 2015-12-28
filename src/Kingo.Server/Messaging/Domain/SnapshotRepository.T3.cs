using System;
using System.Threading.Tasks;

namespace Kingo.Messaging.Domain
{
    /// <summary>
    /// Represents a <see cref="Repository{T, K, S}" /> that stores its aggregates as snapshots.
    /// </summary>
    /// <typeparam name="TKey">Type of the key that identifies an aggregate.</typeparam>
    /// <typeparam name="TVersion">Type of the version of the aggregate.</typeparam>
    /// <typeparam name="TAggregate">Type of aggregates that are managed.</typeparam>
    public abstract class SnapshotRepository<TKey, TVersion, TAggregate> : Repository<TKey, TVersion, TAggregate>        
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>
        where TAggregate : class, IVersionedObject<TKey, TVersion>, IReadableEventStream<TKey, TVersion>
    {
        #region [====== Getting & Updating ======]

        internal override Task UpdateAsync(TAggregate aggregate, TVersion originalVersion, IWritableEventStream<TKey, TVersion> domainEventStream)
        {
            aggregate.WriteTo(domainEventStream);

            return UpdateAsync(aggregate, originalVersion);
        }

        /// <summary>
        /// Updates / overwrites a previous version with a new version.
        /// </summary>
        /// <param name="aggregate">The instance to update.</param>        
        /// <param name="originalVersion">
        /// The version of the aggregate before it was updated.
        /// </param> 
        protected abstract Task UpdateAsync(TAggregate aggregate, TVersion originalVersion);

        #endregion

        #region [====== Adding & Inserting ======]

        internal override Task InsertAsync(TAggregate aggregate, IWritableEventStream<TKey, TVersion> domainEventStream)
        {
            aggregate.WriteTo(domainEventStream);

            return InsertAsync(aggregate);
        }

        /// <summary>
        /// Inserts a new aggregate into the underlying Data Store.
        /// </summary>
        /// <param name="aggregate">The instance to update.</param>                 
        protected abstract Task InsertAsync(TAggregate aggregate);

        #endregion        
    }
}
