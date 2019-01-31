using System;
using System.Collections.Generic;

namespace Kingo.MicroServices.Domain
{
    internal sealed class ChangeSet<TKey, TVersion, TSnapshot> : IChangeSet<TKey, TVersion, TSnapshot>
        where TKey : struct, IEquatable<TKey>
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>        
        where TSnapshot : class, ISnapshotOrEvent<TKey, TVersion>
    {
        private readonly ISerializationStrategyFactory _serializationStrategyFactory;
        private readonly List<AggregateWriteSet<TKey, TVersion, TSnapshot>> _aggregatesToInsert;
        private readonly List<AggregateWriteSet<TKey, TVersion, TSnapshot>> _aggregatesToUpdate;
        private readonly List<TKey> _aggregatesToDelete;        

        public ChangeSet(ISerializationStrategyFactory serializationStrategyFactory)
        {
            _serializationStrategyFactory = serializationStrategyFactory;
            _aggregatesToInsert = new List<AggregateWriteSet<TKey, TVersion, TSnapshot>>();
            _aggregatesToUpdate = new List<AggregateWriteSet<TKey, TVersion, TSnapshot>>();
            _aggregatesToDelete = new List<TKey>();                
        }                
            
        public IReadOnlyList<IAggregateWriteSet<TKey, TVersion, TSnapshot>> AggregatesToInsert =>
            _aggregatesToInsert;

        public IReadOnlyList<IAggregateWriteSet<TKey, TVersion, TSnapshot>> AggregatesToUpdate =>
            _aggregatesToUpdate;

        public IReadOnlyList<TKey> AggregatesToDelete =>
            _aggregatesToDelete;

        public int AddAggregateToInsert<TAggregate>(TAggregate aggregate) where TAggregate : class, IAggregateRoot<TKey, TVersion, TSnapshot> =>
            AddAggregateTo(_aggregatesToInsert, aggregate, null, 0);

        public int AddAggregateToUpdate<TAggregate>(TAggregate aggregate, TVersion oldVersion, int eventsSinceLastSnapshot) where TAggregate : class, IAggregateRoot<TKey, TVersion, TSnapshot> =>
            AddAggregateTo(_aggregatesToUpdate, aggregate, oldVersion, eventsSinceLastSnapshot);

        private int AddAggregateTo<TAggregate>(ICollection<AggregateWriteSet<TKey, TVersion, TSnapshot>> aggregatesToSave, TAggregate aggregate, TVersion? oldVersion, int eventsSinceLastSnapshot)
            where TAggregate : class, IAggregateRoot<TKey, TVersion, TSnapshot>
        {
            var serializationStrategy = _serializationStrategyFactory.CreateSerializationStrategy<TKey, TVersion, TSnapshot, TAggregate>();
            var dataSet = serializationStrategy.Serialize(aggregate, oldVersion, eventsSinceLastSnapshot);

            try
            {
                // If the data-set contains a snapshot, the number or events
                // since the last snapshot is reset to 0, since this snapshot
                // represents the latest version of the aggregate.
                //
                // If not, then the remembered value is added to the new amount
                // of events that were published.
                if (dataSet.Snapshot == null)
                {
                    return dataSet.Events.Count + eventsSinceLastSnapshot;
                }
                return 0;
            }
            finally
            {
                aggregatesToSave.Add(dataSet);
            }
        }

        public void AddAggregateToDelete(TKey id) =>
            _aggregatesToDelete.Add(id);

        public override string ToString() =>
            $"[Inserted: {_aggregatesToInsert.Count}, Updated: {_aggregatesToUpdate.Count}, Deleted: {_aggregatesToDelete.Count}]";
    }
}
