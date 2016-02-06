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
        #region [====== Insert ======]

        internal override async Task InsertAsync(TAggregate aggregate, IWritableEventStream<TKey, TVersion> domainEventStream)
        {
            await InsertAsync(new Snapshot<TKey, TVersion>(TypeToContractMap, aggregate.CreateSnapshot()));

            aggregate.WriteTo(domainEventStream);            
        }

        /// <summary>
        /// Inserts a new snapshot into this repository.
        /// </summary>
        /// <param name="snapshot">Snapshot of the aggregate to insert.</param>                         
        protected abstract Task InsertAsync(Snapshot<TKey, TVersion> snapshot);

        #endregion        

        #region [====== Update ======]

        internal override async Task<bool> UpdateAsync(TAggregate aggregate, TVersion originalVersion, IWritableEventStream<TKey, TVersion> domainEventStream)
        {
            var snapshot = new Snapshot<TKey, TVersion>(TypeToContractMap, aggregate.CreateSnapshot());

            var updateSucceeded = await UpdateAsync(snapshot, originalVersion);
            if (updateSucceeded)
            {
                aggregate.WriteTo(domainEventStream);
                return true;
            }
            return false;
        }

        /// <summary>
        /// When overridden, updates / overwrites a previous version with a new version.
        /// </summary>
        /// <param name="snapshot">Snapshot of the aggregate to update.</param>        
        /// <param name="originalVersion">
        /// The version of the aggregate before it was updated.
        /// </param>        
        /// <returns>
        /// A <see cref="Task{T}" /> representing the update operation. This task should return
        /// <c>true</c> if the update succeeded or <c>false</c> if a concurrency conflict was detected.
        /// </returns>
        protected virtual Task<bool> UpdateAsync(Snapshot<TKey, TVersion> snapshot, TVersion originalVersion)
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
