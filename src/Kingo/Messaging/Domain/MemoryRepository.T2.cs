using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kingo.Threading;

namespace Kingo.Messaging.Domain
{
    /// <summary>
    /// Represents a repository where all aggregates are stored in memory.
    /// </summary>
    /// <typeparam name="TKey">Type of the identifier of the aggregate.</typeparam>
    /// <typeparam name="TAggregate">Type of the aggregate that is managed by this repository.</typeparam>
    public class MemoryRepository<TKey, TAggregate> : Repository<TKey, TAggregate>, IReadOnlyCollection<TAggregate>
        where TKey : struct, IEquatable<TKey>
        where TAggregate : class, IAggregateRoot<TKey>
    {
        private readonly Dictionary<TKey, AggregateData<TKey>> _aggregates;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryRepository{T, S}" /> class.
        /// </summary>
        /// <param name="aggregates">
        /// A collection of aggregates that are initially present in this repository.
        /// </param>
        public MemoryRepository(IEnumerable<TAggregate> aggregates = null)
        {
            _aggregates = CreateAggregateData(aggregates);
        }

        /// <inheritdoc />
        public override string ToString() =>
            $"[Count: {_aggregates.Count}] - " + base.ToString();

        private static Dictionary<TKey, AggregateData<TKey>> CreateAggregateData(IEnumerable<TAggregate> aggregates)
        {
            var aggregateData = new Dictionary<TKey, AggregateData<TKey>>();

            foreach (var aggregate in Serialize(aggregates))
            {
                aggregateData.Add(aggregate.Key, aggregate.Value);
            }
            return aggregateData;
        }

        private static IEnumerable<KeyValuePair<TKey, AggregateData<TKey>>> Serialize(IEnumerable<TAggregate> aggregates)
        {
            if (aggregates == null)
            {
                return Enumerable.Empty<KeyValuePair<TKey, AggregateData<TKey>>>();
            }
            return
                from aggregate in aggregates.WhereNotNull()
                let id = aggregate.Id
                let snapshot = aggregate.TakeSnapshot()
                select new KeyValuePair<TKey, AggregateData<TKey>>(id, new AggregateData<TKey>(id, snapshot));
        }

        #region [====== IReadOnlyCollection<TAggregate> ======]

        /// <inheritdoc />
        public int Count =>
            _aggregates.Count;

        /// <inheritdoc />
        public IEnumerator<TAggregate> GetEnumerator()
        {
            return _aggregates.Values
                .Select(data => data.Snapshot.RestoreAggregate(data.Events))
                .Cast<TAggregate>()
                .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();

        #endregion

        #region [====== Read Operations ======]

        protected internal override Task<AggregateData<TKey>> SelectByIdAsync(TKey id)
        {
            return AsyncMethod.RunSynchronously(() =>
            {
                AggregateData<TKey> aggregate;

                if (_aggregates.TryGetValue(id, out aggregate))
                {
                    return aggregate;
                }
                return null;
            });
        }

        #endregion

        #region [====== Write Operations ======]

        protected internal override Task FlushAsync(IChangeSet<TKey> changeSet)
        {
            return AsyncMethod.RunSynchronously(() =>
            {
                foreach (var aggregate in changeSet.AggregatesToInsert)
                {
                    _aggregates.Add(aggregate.Id, aggregate.GetSnapshotOnly());
                }                
                foreach (var aggregate in changeSet.AggregatesToUpdate)
                {
                    _aggregates[aggregate.Id] = aggregate.GetSnapshotOnly();
                }
                foreach (var aggregate in changeSet.AggregatesToDelete)
                {
                    _aggregates.Remove(aggregate);
                }
            });
        }

        #endregion
    }
}
