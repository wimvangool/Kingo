using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kingo.Resources;
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
        #region [====== Implementation ======]

        private abstract class Implementation : IReadOnlyCollection<TAggregate>
        {            
            protected abstract MemoryRepository<TKey, TAggregate> Repository
            {
                get;
            }

            protected abstract IEnumerable<TKey> Keys
            {
                get;
            }

            public abstract int Count
            {
                get;
            }

            public IEnumerator<TAggregate> GetEnumerator()
            {
                foreach (var id in Keys)
                {
                    yield return Repository.SelectByIdAndRestoreAsync(id).Result;
                }
            }

            IEnumerator IEnumerable.GetEnumerator() =>
                GetEnumerator();            

            public abstract AggregateData<TKey> SelectById(TKey id);

            public abstract void Insert(AggregateData<TKey> data);

            public abstract void Update(AggregateData<TKey> data);

            public abstract void Delete(TKey id);           
        }

        private sealed class StateBasedImplementation : Implementation
        {
            private readonly MemoryRepository<TKey, TAggregate> _repository;
            private readonly Dictionary<TKey, ISnapshot> _snapshots;

            public StateBasedImplementation(MemoryRepository<TKey, TAggregate> repository, IEnumerable<TAggregate> aggregates)
            {
                _repository = repository;
                _snapshots = InitializeRepository(aggregates);
            }

            private static Dictionary<TKey, ISnapshot> InitializeRepository(IEnumerable<TAggregate> aggregates)
            {
                var snapshots = new Dictionary<TKey, ISnapshot>();

                foreach (var aggregate in aggregates)
                {
                    snapshots.Add(aggregate.Id, aggregate.TakeSnapshot());
                }
                return snapshots;
            }

            protected override MemoryRepository<TKey, TAggregate> Repository =>
                _repository;

            protected override IEnumerable<TKey> Keys =>
                _snapshots.Keys;

            public override int Count =>
                _snapshots.Count;            

            public override AggregateData<TKey> SelectById(TKey id)
            {
                ISnapshot snapshot;

                if (_snapshots.TryGetValue(id, out snapshot))
                {
                    return new AggregateData<TKey>(id, snapshot);
                }
                return null;
            }

            public override void Insert(AggregateData<TKey> data) =>
                _snapshots.Add(data.Id, data.Snapshot);

            public override void Update(AggregateData<TKey> data) =>
                _snapshots[data.Id] = data.Snapshot;

            public override void Delete(TKey id) =>
                _snapshots.Remove(id);
        }

        private sealed class EventStoreImplementation : Implementation
        {
            private readonly MemoryRepository<TKey, TAggregate> _repository;
            private readonly Dictionary<TKey, AggregateData<TKey>> _snapshotsAndEvents;

            public EventStoreImplementation(MemoryRepository<TKey, TAggregate> repository, IEnumerable<TAggregate> aggregates)
            {
                _repository = repository;
                _snapshotsAndEvents = InitializeRepository(aggregates);
            }

            private static Dictionary<TKey, AggregateData<TKey>> InitializeRepository(IEnumerable<TAggregate> aggregates)
            {
                var snapshotsAndEvents = new Dictionary<TKey, AggregateData<TKey>>();

                foreach (var aggregate in aggregates)
                {
                    snapshotsAndEvents.Add(aggregate.Id, new AggregateData<TKey>(aggregate.Id, aggregate.TakeSnapshot()));
                }
                return snapshotsAndEvents;
            }

            protected override MemoryRepository<TKey, TAggregate> Repository =>
                _repository;

            protected override IEnumerable<TKey> Keys =>
                _snapshotsAndEvents.Keys;

            public override int Count =>
                _snapshotsAndEvents.Count;

            public override AggregateData<TKey> SelectById(TKey id)
            {
                AggregateData<TKey> aggregate;

                if (_snapshotsAndEvents.TryGetValue(id, out aggregate))
                {
                    return aggregate;
                }
                return null;
            }

            public override void Insert(AggregateData<TKey> data) =>
                _snapshotsAndEvents.Add(data.Id, data.SnapshotOnly());

            public override void Update(AggregateData<TKey> data) =>
                _snapshotsAndEvents[data.Id] = _snapshotsAndEvents[data.Id].Append(data.Events);

            public override void Delete(TKey id) =>
                _snapshotsAndEvents.Remove(id);
        }

        #endregion

        private readonly Implementation _implementation;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryRepository{T, S}" /> class.
        /// </summary>
        /// <param name="behavior">Defines how this repository saves and restores the aggregates.</param>
        /// <param name="aggregates">
        /// A collection of aggregates that are initially present in this repository.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="behavior" /> is not in the range of acceptable values.
        /// </exception>
        public MemoryRepository(MemoryRepositoryBehavior behavior, IEnumerable<TAggregate> aggregates = null)
        {
            _implementation = CreateImplementation(behavior, aggregates ?? Enumerable.Empty<TAggregate>());
        }

        private Implementation CreateImplementation(MemoryRepositoryBehavior behavior, IEnumerable<TAggregate> aggregates)
        {
            switch (behavior)
            {
                case MemoryRepositoryBehavior.StoreSnapshots:
                    return new StateBasedImplementation(this, aggregates);

                case MemoryRepositoryBehavior.StoreEvents:
                    return new EventStoreImplementation(this, aggregates);

                default:
                    throw NewInvalidBehaviorException(behavior);
            }
        }

        private static Exception NewInvalidBehaviorException(MemoryRepositoryBehavior behavior)
        {
            var validValues = string.Join(", ", EnumOperators<MemoryRepositoryBehavior>.AllValues());
            var messageFormat = ExceptionMessages.MemoryRepository_InvalidBehavior;
            var message = string.Format(messageFormat, behavior, validValues);
            return new ArgumentOutOfRangeException(nameof(behavior), message);
        }

        /// <inheritdoc />
        public override string ToString() =>
            base.ToString() + $" (Count = {Count})";

        #region [====== IReadOnlyCollection<TAggregate> ======]

        /// <inheritdoc />
        public int Count =>
            _implementation.Count;

        /// <inheritdoc />
        public IEnumerator<TAggregate> GetEnumerator() =>
            _implementation.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();

        #endregion

        #region [====== Read Operations ======]

        /// <inheritdoc />
        protected internal override Task<AggregateData<TKey>> SelectByIdAsync(TKey id) =>
            AsyncMethod.RunSynchronously(() => _implementation.SelectById(id));

        #endregion

        #region [====== Write Operations ======]

        /// <inheritdoc />
        public override Task FlushAsync() =>
            FlushAsync(false);

        /// <inheritdoc />
        protected internal override Task FlushAsync(IChangeSet<TKey> changeSet)
        {
            return AsyncMethod.RunSynchronously(() =>
            {
                foreach (var data in changeSet.AggregatesToInsert)
                {
                    _implementation.Insert(data);
                }
                foreach (var data in changeSet.AggregatesToUpdate)
                {
                    _implementation.Update(data);
                }
                foreach (var id in changeSet.AggregatesToDelete)
                {
                    _implementation.Delete(id);
                }
            });
        }

        #endregion
    }
}
