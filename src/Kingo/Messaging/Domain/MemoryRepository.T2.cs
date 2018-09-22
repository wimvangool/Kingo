using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Kingo.Threading.AsyncMethod;

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
        private readonly Dictionary<TKey, AggregateDataSet<TKey>> _aggregates;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryRepository{T, S}" /> class.
        /// </summary>              
        /// <param name="serializationStrategy">Specifies the serialization strategy of this repository.</param>
        /// <param name="aggregates">
        /// A collection of aggregates that are initially present in this repository.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="serializationStrategy" /> does not specify a valid serialization strategy.
        /// </exception>
        public MemoryRepository(AggregateSerializationStrategy serializationStrategy, IEnumerable<TAggregate> aggregates = null) :
            base(serializationStrategy)
        {
            _aggregates = InitializeAggregates(serializationStrategy, aggregates ?? Enumerable.Empty<TAggregate>());
        }

        private static Dictionary<TKey, AggregateDataSet<TKey>> InitializeAggregates(AggregateSerializationStrategy serializationStrategy, IEnumerable<TAggregate> aggregatesToAdd)
        {
            var changeSet = new ChangeSet<TKey>(serializationStrategy);

            foreach (var aggregate in aggregatesToAdd)
            {
                changeSet.AddAggregateToInsert(aggregate);
            }
            var aggregates = new Dictionary<TKey, AggregateDataSet<TKey>>();

            foreach (var aggregate in changeSet.AggregatesToInsert)
            {
                aggregates.Add(aggregate.Id, aggregate);
            }
            return aggregates;
        }

        /// <inheritdoc />
        public override string ToString() =>
            base.ToString() + $" (Count = {Count})";

        #region [====== IReadOnlyCollection<TAggregate> ======]

        /// <inheritdoc />
        public int Count =>
            _aggregates.Count;

        /// <inheritdoc />
        public IEnumerator<TAggregate> GetEnumerator()
        {
            foreach (var key in _aggregates.Keys)
            {
                yield return GetByIdAsync(key).Await();
            }
        }

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();

        #endregion

        #region [====== Read Operations ======]

        /// <inheritdoc />
        protected internal override Task<AggregateDataSet<TKey>> SelectByIdAsync(TKey id) => Run(() =>
        {
            if (_aggregates.TryGetValue(id, out var aggregate))
            {
                return aggregate;
            }
            return null;
        });

        #endregion

        #region [====== Write Operations ======]

        /// <inheritdoc />
        public override Task FlushAsync() =>
            FlushAsync(false);

        /// <inheritdoc />
        protected internal override async Task FlushAsync(IChangeSet<TKey> changeSet)
        {
            foreach (var data in changeSet.AggregatesToInsert)
            {
                Insert(data);
            }
            foreach (var data in changeSet.AggregatesToUpdate)
            {
                Update(data);
            }
            foreach (var id in changeSet.AggregatesToDelete)
            {
                Delete(id);
            }
        }

        private void Insert(AggregateDataSet<TKey> data) =>
            _aggregates.Add(data.Id, data);

        private void Update(AggregateDataSet<TKey> data) =>
            _aggregates[data.Id] = _aggregates[data.Id].Append(data);

        private void Delete(TKey id) =>
            _aggregates.Remove(id);

        #endregion
    }
}
