using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kingo.Threading;

namespace Kingo.Messaging.Domain
{
    /// <summary>
    /// Represents an in-memory repository of aggregates that can be used in combination with
    /// write-only scenarios to temporarily store data when testing business logic.
    /// </summary>
    /// <typeparam name="TKey">Type of the key that identifies an aggregate.</typeparam>
    /// <typeparam name="TVersion">Type of the version of the aggregate.</typeparam>
    /// <typeparam name="TAggregate">Type of aggregates that are managed.</typeparam>
    public class InMemoryRepository<TKey, TVersion, TAggregate> : SnapshotRepository<TKey, TVersion, TAggregate>
        where TKey : struct, IEquatable<TKey>
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>
        where TAggregate : class, IAggregateRoot<TKey, TVersion>
    {
        private readonly ConcurrentDictionary<TKey, ISnapshot<TKey, TVersion>> _snapshots;

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryRepository{T, S, R}" /> class.
        /// </summary>
        protected InMemoryRepository()
        {
            _snapshots = new ConcurrentDictionary<TKey, ISnapshot<TKey, TVersion>>();
        }

        /// <inheritdoc />
        protected override ITypeToContractMap TypeToContractMap
        {
            get { return Domain.TypeToContractMap.FullyQualifiedName; }
        }

        /// <summary>
        /// Returns a thread-safe dictionary of aggregates.
        /// </summary>
        protected IDictionary<TKey, ISnapshot<TKey, TVersion>> Snapshots
        {
            get { return _snapshots; }
        }

        /// <inheritdoc />
        protected override Task<ISnapshot<TKey, TVersion>> SelectByKeyAsync(TKey key, ITypeToContractMap map)
        {
            return AsyncMethod.RunSynchronously(() =>
            {
                ISnapshot<TKey, TVersion> snapshot;

                if (Snapshots.TryGetValue(key, out snapshot))
                {
                    return snapshot;
                }
                return null;              
            });           
        }

        /// <inheritdoc />
        protected override Task<bool> UpdateAsync(Snapshot<TKey, TVersion> snapshot, TVersion originalVersion)
        {
            return AsyncMethod.RunSynchronously(() =>
            {
                Snapshots[snapshot.Value.Key] = snapshot.Value;
                return true;
            });               
        }

        /// <inheritdoc />
        protected override Task InsertAsync(Snapshot<TKey, TVersion> snapshot)
        {
            return AsyncMethod.RunSynchronously(() =>
            {                
                try
                {
                    Snapshots.Add(snapshot.Value.Key, snapshot.Value);
                }
                catch (ArgumentException)
                {
                    throw NewDuplicateKeyException(snapshot.Value.Key);
                }                
            });
        }        
    }
}
