using System;
using System.Collections.Generic;

namespace Kingo.MicroServices.Domain
{
    internal sealed class UseSnapshotsStrategy<TKey, TVersion, TSnapshot, TAggregate> : SerializationStrategy<TKey, TVersion, TSnapshot, TAggregate>
        where TKey : struct, IEquatable<TKey>
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>        
        where TSnapshot : class, ISnapshotOrEvent<TKey, TVersion>
        where TAggregate : class, IAggregateRoot<TKey, TVersion, TSnapshot>
    {
        #region [====== Serialize ======]

        protected override IReadOnlyList<ISnapshotOrEvent<TKey, TVersion>> GetEvents(IReadOnlyList<ISnapshotOrEvent<TKey, TVersion>> events) =>
            new ISnapshotOrEvent<TKey, TVersion>[0];

        #endregion

        #region [====== Deserialize ======]

        protected override IAggregateRoot<TKey, TVersion> Deserialize(AggregateWriteSet<TKey, TVersion, ISnapshotOrEvent<TKey, TVersion>> dataSet, IEventBus eventBus)
        {
            if (dataSet.Snapshot == null)
            {
                throw NewMissingSnapshotException(dataSet.ToReadSet());
            }
            return dataSet.Snapshot.RestoreAggregate(eventBus);
        }

        private static Exception NewMissingSnapshotException(AggregateReadSet dataSet) =>
            new CouldNotRestoreAggregateException(dataSet, ExceptionMessages.SerializationStrategy_MissingSnapshot);

        #endregion
    }
}
