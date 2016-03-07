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
        private readonly ConcurrentDictionary<TKey, IMemento<TKey, TVersion>> _snapshots;

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryRepository{T, S, R}" /> class.
        /// </summary>
        protected InMemoryRepository()
        {
            _snapshots = new ConcurrentDictionary<TKey, IMemento<TKey, TVersion>>();
        }

        /// <inheritdoc />
        protected override ITypeToContractMap TypeToContractMap
        {
            get { return Messaging.TypeToContractMap.FullyQualifiedName; }
        }

        /// <summary>
        /// Returns a thread-safe dictionary of aggregates.
        /// </summary>
        protected IDictionary<TKey, IMemento<TKey, TVersion>> Snapshots
        {
            get { return _snapshots; }
        }

        /// <inheritdoc />
        protected override Task<IMemento<TKey, TVersion>> SelectSnapshotByKeyAsync(TKey key, ITypeToContractMap map)
        {
            return AsyncMethod.RunSynchronously(() =>
            {
                IMemento<TKey, TVersion> snapshot;

                if (Snapshots.TryGetValue(key, out snapshot))
                {
                    return snapshot;
                }
                return null;              
            });           
        }

        /// <inheritdoc />
        protected override Task<bool> UpdateAsync(SnapshotToSave<TKey, TVersion> snapshotToSave, TVersion originalVersion)
        {
            return AsyncMethod.RunSynchronously(() =>
            {
                Snapshots[snapshotToSave.Value.Key] = snapshotToSave.Value;
                return true;
            });               
        }

        /// <inheritdoc />
        protected override Task InsertAsync(SnapshotToSave<TKey, TVersion> snapshotToSave)
        {
            return AsyncMethod.RunSynchronously(() =>
            {                
                try
                {
                    Snapshots.Add(snapshotToSave.Value.Key, snapshotToSave.Value);
                }
                catch (ArgumentException)
                {
                    throw NewDuplicateKeyException(snapshotToSave.Value.Key);
                }                
            });
        }        
    }
}
