using System;
using System.Collections.Generic;

namespace Kingo.MicroServices.Domain
{
    internal sealed class AggregateRootDecorator<TKey, TVersion, TAggregate> : IAggregateRoot<TKey, TVersion, ISnapshotOrEvent<TKey, TVersion>>
        where TKey : struct, IEquatable<TKey>
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>
        where TAggregate : class, IAggregateRoot<TKey, TVersion>
    {
        private readonly TAggregate _aggregate;

        public AggregateRootDecorator(TAggregate aggregate)
        {
            _aggregate = aggregate ?? throw new ArgumentNullException(nameof(aggregate));
        }

        #region [====== Id & Version =====]

        public TKey Id =>
            _aggregate.Id;

        public TVersion Version =>
            _aggregate.Version;

        #endregion

        #region [====== Modification ======]

        public bool HasBeenModified =>
            _aggregate.HasBeenModified;

        public event EventHandler Modified
        {
            add => _aggregate.Modified += value;
            remove => _aggregate.Modified -= value;
        }

        #endregion
        
        #region [====== Removal ======]

        public bool HasBeenRemoved =>
            _aggregate.HasBeenRemoved;

        public bool NotifyRemoved() =>
            _aggregate.NotifyRemoved();

        #endregion

        #region [====== Events ======]

        public void LoadFromHistory(IEnumerable<ISnapshotOrEvent<TKey, TVersion>> events) =>
            _aggregate.LoadFromHistory(events);

        public IReadOnlyList<ISnapshotOrEvent<TKey, TVersion>> Commit() =>
            _aggregate.Commit();

        #endregion

        #region [====== Snapshots ======]

        public ISnapshotOrEvent<TKey, TVersion> TakeSnapshot() =>
            throw NewSnapshotsNotSupportedException(typeof(TAggregate));

        private static Exception NewSnapshotsNotSupportedException(Type aggregateType)
        {
            var messageFormat = ExceptionMessages.AggregateRootDecorator_SnapshotsNotSupported;
            var message = string.Format(messageFormat, aggregateType.FriendlyName());
            return new InvalidOperationException(message);
        }

        #endregion

        public static implicit operator TAggregate(AggregateRootDecorator<TKey, TVersion, TAggregate> decorator) =>
            decorator?._aggregate;
    }
}
