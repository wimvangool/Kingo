using System;
using System.Collections.Generic;

namespace Kingo.MicroServices.Domain
{
    internal sealed class ChangeSetDecorator<TKey, TVersion, TSnapshot> : IChangeSet<TKey, TVersion>
        where TKey : struct, IEquatable<TKey>
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>
        where TSnapshot : class, ISnapshotOrEvent<TKey, TVersion>
    {
        private readonly IChangeSet<TKey, TVersion, TSnapshot> _changeSet;

        public ChangeSetDecorator(IChangeSet<TKey, TVersion, TSnapshot> changeSet)
        {
            _changeSet = changeSet;
        }

        public override string ToString() =>
            _changeSet.ToString();

        public IReadOnlyList<IAggregateWriteSet<TKey, TVersion>> AggregatesToInsert =>
            _changeSet.AggregatesToInsert;

        public IReadOnlyList<IAggregateWriteSet<TKey, TVersion>> AggregatesToUpdate =>
            _changeSet.AggregatesToUpdate;

        public IReadOnlyList<TKey> AggregatesToDelete =>
            _changeSet.AggregatesToDelete;
    }
}
