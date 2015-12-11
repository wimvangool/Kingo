using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kingo.Threading;

namespace Kingo.Messaging.Domain
{
    /// <summary>
    /// Represents an in-memory repository of aggregates that can be used in combination with
    /// <see cref="WriteOnlyScenario{T}">WriteOnlyScenarios</see> to temporarily store data
    /// when testing business logic.
    /// </summary>
    /// <typeparam name="TKey">Type of the key that identifies an aggregate.</typeparam>
    /// <typeparam name="TVersion">Type of the version of the aggregate.</typeparam>
    /// <typeparam name="TAggregate">Type of aggregates that are managed.</typeparam>
    public class MemoryRepository<TKey, TVersion, TAggregate> : SnapshotRepository<TKey, TVersion, TAggregate>
        where TKey : struct, IEquatable<TKey>
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>
        where TAggregate : class, IVersionedObject<TKey, TVersion>, IReadableEventStream<TKey, TVersion>
    {
        private readonly Dictionary<TKey, TAggregate> _aggregates;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryRepository{T, S, R}" /> class.
        /// </summary>
        protected MemoryRepository()
        {
            _aggregates = new Dictionary<TKey, TAggregate>();
        }

        protected override Task<TAggregate> SelectByKeyAsync(TKey key)
        {
            return AsyncMethod.RunSynchronously(() =>
            {               
                lock (_aggregates)
                {
                    TAggregate aggregate;

                    if (_aggregates.TryGetValue(key, out aggregate))
                    {
                        return aggregate;
                    }
                }
                return null;
            });           
        }

        protected override Task UpdateAsync(TAggregate aggregate, TVersion originalVersion)
        {
            return AsyncMethod.RunSynchronously(() =>
            {
                lock (_aggregates)
                {
                    _aggregates[aggregate.Key] = aggregate;
                }
            });               
        }

        protected override Task InsertAsync(TAggregate aggregate)
        {
            return AsyncMethod.RunSynchronously(() =>
            {
                try
                {
                    lock (_aggregates)
                    {
                        _aggregates.Add(aggregate.Key, aggregate);
                    }
                }
                catch (ArgumentException)
                {
                    throw NewDuplicateKeyException(aggregate.Key);
                }                
            });
        }        
    }
}
