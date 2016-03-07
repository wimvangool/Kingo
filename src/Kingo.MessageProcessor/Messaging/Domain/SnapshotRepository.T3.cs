using System;
using System.Threading.Tasks;
using Kingo.Resources;

namespace Kingo.Messaging.Domain
{
    /// <summary>
    /// Represents a repository that stores its aggregates as snapshots.
    /// </summary>
    /// <typeparam name="TKey">Type of the key that identifies an aggregate.</typeparam>
    /// <typeparam name="TVersion">Type of the version of the aggregate.</typeparam>
    /// <typeparam name="TAggregate">Type of aggregates that are managed.</typeparam>
    public abstract class SnapshotRepository<TKey, TVersion, TAggregate> : Repository<TKey, TVersion, TAggregate>        
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>
        where TAggregate : class, IAggregateRoot<TKey, TVersion>
    {
        #region [====== Select ======]

        internal override async Task<TAggregate> SelectByKeyAsync(TKey key)
        {
            var snapshot = await SelectSnapshotByKeyAsync(key, TypeToContractMap);
            if (snapshot == null)
            {
                return null;
            }
            return (TAggregate) snapshot.RestoreAggregate();
        }

        /// <summary>
        /// Loads a snapshot from the repository.
        /// </summary>
        /// <param name="key">Key of the aggregate.</param>
        /// <param name="map">
        /// The mapping from each contract to a specific type that can be used to deserialize the retrieved data to its correct type.
        /// </param>
        /// <returns>
        /// A <see cref="Task{T}" /> representing the load operation. The task should return <c>null</c> if the aggregate was not found.
        /// </returns>
        protected abstract Task<IMemento<TKey, TVersion>> SelectSnapshotByKeyAsync(TKey key, ITypeToContractMap map);

        #endregion

        #region [====== Insert ======]

        internal override async Task InsertAsync(TAggregate aggregate, IDomainEventBus<TKey, TVersion> eventBus)
        {
            await InsertAsync(new SnapshotToSave<TKey, TVersion>(TypeToContractMap, aggregate.CreateSnapshot()));

            aggregate.Commit(eventBus);         
        }

        /// <summary>
        /// Inserts a new snapshot into this repository.
        /// </summary>
        /// <param name="snapshotToSave">Snapshot of the aggregate to insert.</param>                         
        protected abstract Task InsertAsync(SnapshotToSave<TKey, TVersion> snapshotToSave);

        #endregion        

        #region [====== Update ======]

        internal override async Task<bool> UpdateAsync(TAggregate aggregate, TVersion originalVersion, IDomainEventBus<TKey, TVersion> eventBus)
        {
            var snapshot = new SnapshotToSave<TKey, TVersion>(TypeToContractMap, aggregate.CreateSnapshot());

            var updateSucceeded = await UpdateAsync(snapshot, originalVersion);
            if (updateSucceeded)
            {
                aggregate.Commit(eventBus);
                return true;
            }
            return false;
        }

        /// <summary>
        /// When overridden, updates / overwrites a previous version with a new version.
        /// </summary>
        /// <param name="snapshotToSave">Snapshot of the aggregate to update.</param>        
        /// <param name="originalVersion">
        /// The version of the aggregate before it was updated.
        /// </param>        
        /// <returns>
        /// A <see cref="Task{T}" /> representing the update operation. This task should return
        /// <c>true</c> if the update succeeded or <c>false</c> if a concurrency conflict was detected.
        /// </returns>
        protected virtual Task<bool> UpdateAsync(SnapshotToSave<TKey, TVersion> snapshotToSave, TVersion originalVersion)
        {
            throw NewUpdateNotSupportedException();
        }

        private Exception NewUpdateNotSupportedException()
        {
            var messageFormat = ExceptionMessages.Repository_UpdateNotSupported;
            var message = string.Format(messageFormat, typeof(TAggregate), GetType());
            return new NotSupportedException(message);
        }

        #endregion       
    }
}
