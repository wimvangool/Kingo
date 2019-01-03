using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kingo.Threading;

namespace Kingo.MicroServices.Domain
{
    /// <summary>
    /// Represents a repository where all aggregates are stored in memory.
    /// </summary>
    /// <typeparam name="TKey">Type of the identifier of the aggregate.</typeparam>
    /// <typeparam name="TVersion">Type of the version of the aggregate.</typeparam>
    /// <typeparam name="TAggregate">Type of the aggregate that is managed by this repository.</typeparam>
    public class MemoryRepository<TKey, TVersion, TAggregate> : Repository<TKey, TVersion, TAggregate>, IReadOnlyCollection<TAggregate>
        where TKey : struct, IEquatable<TKey>
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>
        where TAggregate : class, IAggregateRoot<TKey, TVersion>
    {
        #region [====== DataSet ======]

        private sealed class AggregateDataSet
        {
            private readonly ISnapshotOrEvent<TKey, TVersion> _snapshot;
            private readonly ISnapshotOrEvent<TKey, TVersion>[] _events;
            private readonly TVersion _version;

            public AggregateDataSet(AggregateDataSet<TKey, TVersion> dataSet)
            {
                _snapshot = dataSet.Snapshot;
                _events = dataSet.Events.ToArray();
                _version = dataSet.NewVersion;
            }            

            private AggregateDataSet(ISnapshotOrEvent<TKey, TVersion> snapshot, IEnumerable<ISnapshotOrEvent<TKey, TVersion>> events, TVersion version)
            {
                _snapshot = snapshot;
                _events = events.ToArray();
                _version = version;
            }

            public AggregateDataSet<TKey> ToDataSet(TKey id) =>
                new AggregateDataSet<TKey>(id, _snapshot, _events);

            public AggregateDataSet Append(AggregateDataSet<TKey, TVersion> dataSet)
            {
                if (_version.Equals(dataSet.OldVersion))
                {
                    return new AggregateDataSet(dataSet.Snapshot ?? _snapshot, _events.Concat(dataSet.Events), dataSet.NewVersion);
                }
                throw NewConcurrencyException(dataSet.OldVersion, _version);
            }

            private static Exception NewConcurrencyException(TVersion expectedVersion, TVersion actualVersion)
            {
                var messageFormat = ExceptionMessages.MemoryRepository_ConcurrencyConflict;
                var message = string.Format(messageFormat, typeof(TAggregate).FriendlyName(), actualVersion, expectedVersion);
                return new ConcurrencyException(message);
            }
        }

        #endregion

        private readonly Dictionary<TKey, AggregateDataSet> _aggregates;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryRepository{T, S, R}" /> class.
        /// </summary>              
        /// <param name="serializationStrategy">Specifies the serialization strategy of this repository.</param>        
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="serializationStrategy" /> does not specify a valid serialization strategy.
        /// </exception>
        public MemoryRepository(SerializationStrategy serializationStrategy) :
            base(serializationStrategy)
        {
            _aggregates = new Dictionary<TKey, AggregateDataSet>();
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
        protected internal override Task<AggregateDataSet<TKey>> SelectByIdAsync(TKey id) => AsyncMethod.Run(() =>
        {
            if (_aggregates.TryGetValue(id, out var dataSet))
            {
                return dataSet.ToDataSet(id);
            }
            return null;
        });

        #endregion

        #region [====== Write Operations ======]

        /// <inheritdoc />
        public override Task FlushAsync() =>
            FlushAsync(false);

        /// <inheritdoc />
        protected internal override Task FlushAsync(IChangeSet<TKey, TVersion> changeSet) => AsyncMethod.Run(() =>
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
        });

        private void Insert(AggregateDataSet<TKey, TVersion> data) =>
            _aggregates.Add(data.Id, new AggregateDataSet(data));            

        private void Update(AggregateDataSet<TKey, TVersion> data) =>
            _aggregates[data.Id] = _aggregates[data.Id].Append(data);            

        private void Delete(TKey id) =>
            _aggregates.Remove(id);

        #endregion
    }
}
