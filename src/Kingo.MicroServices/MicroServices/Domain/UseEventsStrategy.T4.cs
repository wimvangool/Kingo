using System;
using System.Collections.Generic;
using System.Linq;

namespace Kingo.MicroServices.Domain
{
    internal sealed class UseEventsStrategy<TKey, TVersion, TSnapshot, TAggregate> : SerializationStrategy<TKey, TVersion, TSnapshot, TAggregate>
        where TKey : struct, IEquatable<TKey>
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>        
        where TSnapshot : class, ISnapshotOrEvent<TKey, TVersion>
        where TAggregate : class, IAggregateRoot<TKey, TVersion, TSnapshot>
    {
        private readonly int _eventsPerSnapshot;

        public UseEventsStrategy(int eventsPerSnapshot)
        {            
            _eventsPerSnapshot = eventsPerSnapshot;
        }

        private bool UseSnapshots =>
            0 < _eventsPerSnapshot;        

        #region [====== Serialize ======]

        protected override TSnapshot GetSnapshot(IAggregateRoot<TKey, TVersion, TSnapshot> aggregate, int eventsSinceLastSnapshot)
        {
            if (UseSnapshots && _eventsPerSnapshot <= eventsSinceLastSnapshot)
            {
                return base.GetSnapshot(aggregate, eventsSinceLastSnapshot);
            }
            return null;
        }

        #endregion

        #region [====== Deserialize ======]

        protected override IAggregateRoot<TKey, TVersion> Deserialize(AggregateWriteSet<TKey, TVersion, ISnapshotOrEvent<TKey, TVersion>> dataSet, IEventBus eventBus)
        {
            if (dataSet.Snapshot != null && UseSnapshots)
            {
                return RestoreAggregate(eventBus, dataSet.Snapshot, dataSet.Events);
            }
            if (dataSet.Events.Count > 0)
            {
                return RestoreAggregate(eventBus, dataSet.Events[0], dataSet.Events.Skip(1));
            }
            throw NewMissingSnapshotOrEventException(dataSet.ToReadSet());
        }

        private static IAggregateRoot<TKey, TVersion> RestoreAggregate(IEventBus eventBus, ISnapshotOrEvent<TKey, TVersion> factory, IEnumerable<ISnapshotOrEvent<TKey, TVersion>> events)            
        {
            var aggregate = factory.RestoreAggregate(eventBus);
            aggregate.LoadFromHistory(events);
            return aggregate;
        }

        private static Exception NewMissingSnapshotOrEventException(AggregateReadSet dataSet) =>
            new CouldNotRestoreAggregateException(dataSet, ExceptionMessages.SerializationStrategy_MissingSnapshotOrEvent);        

        #endregion
    }
}
