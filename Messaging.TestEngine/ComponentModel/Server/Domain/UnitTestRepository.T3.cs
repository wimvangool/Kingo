using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace System.ComponentModel.Server.Domain
{
    /// <summary>
    /// Represents a <see cref="Repository{K, V, A}" /> that can be used for storing aggregates in memory while running
    /// <see cref="Scenario" /> tests.
    /// </summary>
    /// <typeparam name="TKey">Type of the key that identifies an aggregate.</typeparam>
    /// <typeparam name="TVersion">Type of the version of the aggregate.</typeparam>
    /// <typeparam name="TAggregate">Type of aggregates that are managed.</typeparam>
    [SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes")]
    public abstract class UnitTestRepository<TAggregate, TKey, TVersion> : Repository<TAggregate, TKey, TVersion>
        where TAggregate : class, IVersionedObject<TKey, TVersion>
        where TKey : struct, IEquatable<TKey>
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>        
    {
        private readonly IDependencyCache _cache;
        private IDependencCacheEntry<IDictionary<TKey, TAggregate>> _cacheEntry;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitTestRepository{K, V, A}" /> class.
        /// </summary>
        protected UnitTestRepository()
        {
            _cache = new ScenarioCache();
        }

        /// <summary>
        /// Returns a <see cref="IDictionary{K, A}" /> that contains all stored aggregates.
        /// </summary>
        protected IDictionary<TKey, TAggregate> Aggregates
        {
            get
            {
                IDictionary<TKey, TAggregate> aggregates;

                if (_cacheEntry == null || !_cacheEntry.TryGetValue(out aggregates))
                {
                    _cacheEntry = _cache.Add(aggregates = CreateAggregateDictionary(), OnCacheEntryRemoved);
                }                
                return aggregates;
            }
        }

        private void OnCacheEntryRemoved(IDictionary<TKey, TAggregate> aggregates)
        {
            _cacheEntry = null;
        }

        /// <summary>
        /// Creates and returns a new <see cref="IDictionary{K, A}" /> that will be used to store all aggregates.
        /// </summary>
        /// <returns>A new <see cref="IDictionary{K, A}" />.</returns>
        protected abstract IDictionary<TKey, TAggregate> CreateAggregateDictionary();

        /// <inheritdoc />
        protected override bool TrySelect(TKey key, out TAggregate aggregate)
        {
            return Aggregates.TryGetValue(key, out aggregate);
        }

        /// <inheritdoc />
        protected override void Insert(TAggregate aggregate)
        {
            Aggregates.Add(aggregate.Key, aggregate);
        }

        /// <inheritdoc />
        protected override void Update(TAggregate aggregate, TVersion originalVersion)
        {
            Aggregates[aggregate.Key] = aggregate;
        }

        /// <inheritdoc />
        protected override void Delete(TAggregate aggregate)
        {
            Aggregates.Remove(aggregate.Key);
        }        
    }
}
