using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Kingo.MicroServices.Domain
{
    internal sealed class MemoryDataStore<TKey, TVersion> : IReadOnlyDictionary<TKey, AggregateReadSet>
        where TKey : struct, IEquatable<TKey>
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>        
    {
        #region [====== VersionedDataSet ======]

        private sealed class VersionedDataSet
        {
            private readonly ISnapshotOrEvent<TKey, TVersion> _snapshot;
            private readonly ISnapshotOrEvent<TKey, TVersion>[] _events;
            private readonly TVersion _version;

            public VersionedDataSet(IAggregateWriteSet<TKey, TVersion> dataSet, ISnapshotOrEvent<TKey, TVersion> snapshot)
            {
                _snapshot = snapshot;
                _events = dataSet.Events.ToArray();
                _version = dataSet.NewVersion;
            }

            private VersionedDataSet(ISnapshotOrEvent<TKey, TVersion> snapshot, IEnumerable<ISnapshotOrEvent<TKey, TVersion>> events, TVersion version)
            {
                _snapshot = snapshot;
                _events = events.ToArray();
                _version = version;
            }

            public AggregateReadSet ToReadSet() =>
                new AggregateReadSet(_snapshot, _events);

            public VersionedDataSet Append(IAggregateWriteSet<TKey, TVersion> dataSet, ISnapshotOrEvent<TKey, TVersion> snapshot)
            {
                if (_version.Equals(dataSet.OldVersion))
                {
                    return new VersionedDataSet(snapshot ?? _snapshot, _events.Concat(dataSet.Events), dataSet.NewVersion);
                }
                throw NewConcurrencyException(dataSet.Id, dataSet.OldVersion, _version);
            }

            private static Exception NewConcurrencyException(TKey id, TVersion expectedVersion, TVersion actualVersion)
            {
                var messageFormat = ExceptionMessages.MemoryDataStore_ConcurrencyConflict;
                var message = string.Format(messageFormat, id, actualVersion, expectedVersion);
                return new ConcurrencyException(message);
            }
        }

        #endregion

        private readonly Dictionary<TKey, VersionedDataSet> _aggregates;

        public MemoryDataStore()
        {
            _aggregates = new Dictionary<TKey, VersionedDataSet>();
        }

        public override string ToString() =>
            $"{_aggregates.Count} aggregate(s)";

        #region [====== IReadOnlyDictionary<TKey, AggregateReadSet> ======]

        public int Count =>
            _aggregates.Count;

        public IEnumerable<TKey> Keys =>
            _aggregates.Keys;

        public IEnumerable<AggregateReadSet> Values =>
            _aggregates.Values.Select(dataSet => dataSet.ToReadSet());

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();

        public IEnumerator<KeyValuePair<TKey, AggregateReadSet>> GetEnumerator() =>
            _aggregates.Select(pair => new KeyValuePair<TKey, AggregateReadSet>(pair.Key, pair.Value.ToReadSet())).GetEnumerator();

        public AggregateReadSet this[TKey key] =>
            _aggregates[key].ToReadSet();

        public bool ContainsKey(TKey key) =>
            _aggregates.ContainsKey(key);

        public bool TryGetValue(TKey key, out AggregateReadSet value)
        {
            if (_aggregates.TryGetValue(key, out var dataSet))
            {
                value = dataSet.ToReadSet();
                return true;
            }
            value = null;
            return false;
        }

        #endregion

        #region [====== Flush ======]

        public void Flush<TSnapshot>(IChangeSet<TKey, TVersion, TSnapshot> changeSet)
            where TSnapshot : class, ISnapshotOrEvent<TKey, TVersion>
        {
            foreach (var aggregate in changeSet.AggregatesToInsert)
            {
                Insert(aggregate, aggregate.Snapshot);
            }
            foreach (var aggregate in changeSet.AggregatesToUpdate)
            {
                Update(aggregate, aggregate.Snapshot);
            }
            foreach (var aggregateId in changeSet.AggregatesToDelete)
            {
                Delete(aggregateId);
            }
        }

        public void Flush(IChangeSet<TKey, TVersion> changeSet)
        {
            foreach (var aggregate in changeSet.AggregatesToInsert)
            {
                Insert(aggregate);
            }
            foreach (var aggregate in changeSet.AggregatesToUpdate)
            {
                Update(aggregate);
            }
            foreach (var aggregateId in changeSet.AggregatesToDelete)
            {
                Delete(aggregateId);
            }
        }

        private void Insert(IAggregateWriteSet<TKey, TVersion> data, ISnapshotOrEvent<TKey, TVersion> snapshot = null) =>
            _aggregates.Add(data.Id, new VersionedDataSet(data, snapshot));

        private void Update(IAggregateWriteSet<TKey, TVersion> data, ISnapshotOrEvent<TKey, TVersion> snapshot = null) =>
            _aggregates[data.Id] = _aggregates[data.Id].Append(data, snapshot);

        private void Delete(TKey id) =>
            _aggregates.Remove(id);

        #endregion
    }
}
