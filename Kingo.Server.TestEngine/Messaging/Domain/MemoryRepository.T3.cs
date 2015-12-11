using System;
using System.Collections.Concurrent;
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
        private readonly ConcurrentDictionary<TKey, TAggregate> _aggregates;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryRepository{T, S, R}" /> class.
        /// </summary>
        protected MemoryRepository()
        {
            _aggregates = new ConcurrentDictionary<TKey, TAggregate>();
        }        

        /// <summary>
        /// Returns a thread-safe dictionary of aggregates.
        /// </summary>
        protected IDictionary<TKey, TAggregate> Aggregates
        {
            get { return _aggregates; }
        }

        /// <inheritdoc />
        protected override Task<TAggregate> SelectByKeyAsync(TKey key)
        {
            return AsyncMethod.RunSynchronously(() =>
            {
                TAggregate aggregate;

                if (Aggregates.TryGetValue(key, out aggregate))
                {
                    return aggregate;
                }
                return null;              
            });           
        }

        /// <inheritdoc />
        protected override Task UpdateAsync(TAggregate aggregate, TVersion originalVersion)
        {
            return AsyncMethod.RunSynchronously(() =>
            {
                Aggregates[aggregate.Key] = aggregate;
            });               
        }

        /// <inheritdoc />
        protected override Task InsertAsync(TAggregate aggregate)
        {
            return AsyncMethod.RunSynchronously(() =>
            {                
                try
                {
                    Aggregates.Add(aggregate.Key, aggregate);
                }
                catch (ArgumentException)
                {
                    throw NewDuplicateKeyException(aggregate.Key);
                }                
            });
        }        
    }
}
