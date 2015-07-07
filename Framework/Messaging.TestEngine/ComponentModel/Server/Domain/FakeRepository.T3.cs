using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Syztem.Threading;

namespace Syztem.ComponentModel.Server.Domain
{
    /// <summary>
    /// Represents a <see cref="Repository{K, V, A}" /> that can be used for storing aggregates in memory while running
    /// <see cref="Scenario" /> tests.
    /// </summary>
    /// <typeparam name="TKey">Type of the key that identifies an aggregate.</typeparam>
    /// <typeparam name="TVersion">Type of the version of the aggregate.</typeparam>
    /// <typeparam name="TAggregate">Type of aggregates that are managed.</typeparam>
    [SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes")]
    public abstract class FakeRepository<TAggregate, TKey, TVersion> : Repository<TAggregate, TKey, TVersion>
        where TAggregate : class, IVersionedObject<TKey, TVersion>
        where TKey : struct, IEquatable<TKey>
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>        
    {        
        /// <summary>
        /// Returns a <see cref="IDictionary{TKey,TValue}" /> that contains all stored aggregates.
        /// </summary>
        protected IDictionary<TKey, TAggregate> Aggregates
        {
            get { return ScenarioCache.Instance.GetOrAdd(GetType(), CreateAggregateDictionary); }            
        }                      

        /// <summary>
        /// Creates and returns a new <see cref="IDictionary{K, A}" /> that will be used to store all aggregates.
        /// </summary>
        /// <returns>A new <see cref="IDictionary{K, A}" />.</returns>
        protected virtual IDictionary<TKey, TAggregate> CreateAggregateDictionary()
        {
            return new Dictionary<TKey, TAggregate>();
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
        protected override Task InsertAsync(TAggregate aggregate)
        {
            return AsyncMethod.RunSynchronously(() => Aggregates.Add(aggregate.Key, aggregate));            
        }

        /// <inheritdoc />
        protected override Task UpdateAsync(TAggregate aggregate, TVersion originalVersion)
        {
            return AsyncMethod.RunSynchronously(() => Aggregates[aggregate.Key] = aggregate);
        }

        /// <inheritdoc />
        protected override Task DeleteAsync(TKey key)
        {
            return AsyncMethod.RunSynchronously(() => Aggregates.Remove(key));
        }        
    }
}
