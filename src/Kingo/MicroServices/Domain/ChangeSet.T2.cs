using System;
using System.Collections.Generic;

namespace Kingo.MicroServices.Domain
{
    internal sealed class ChangeSet<TKey, TVersion> : IChangeSet<TKey, TVersion>
        where TKey : struct, IEquatable<TKey>
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>
    {
        private readonly SerializationStrategy _serializationStrategy;
        private readonly List<AggregateDataSet<TKey, TVersion>> _aggregatesToInsert;
        private readonly List<AggregateDataSet<TKey, TVersion>> _aggregatesToUpdate;
        private readonly List<TKey> _aggregatesToDelete;        

        public ChangeSet(SerializationStrategy serializationStrategy)
        {
            _serializationStrategy = serializationStrategy;
            _aggregatesToInsert = new List<AggregateDataSet<TKey, TVersion>>();
            _aggregatesToUpdate = new List<AggregateDataSet<TKey, TVersion>>();
            _aggregatesToDelete = new List<TKey>();                
        }                
            
        public IReadOnlyList<AggregateDataSet<TKey, TVersion>> AggregatesToInsert =>
            _aggregatesToInsert;

        public IReadOnlyList<AggregateDataSet<TKey, TVersion>> AggregatesToUpdate =>
            _aggregatesToUpdate;

        public IReadOnlyList<TKey> AggregatesToDelete =>
            _aggregatesToDelete;

        public int AddAggregateToInsert(IAggregateRoot<TKey, TVersion> aggregate) =>
            AddAggregateTo(_aggregatesToInsert, aggregate, null, 0);

        public int AddAggregateToUpdate(IAggregateRoot<TKey, TVersion> aggregate, TVersion oldVersion, int eventsSinceLastSnapshot) =>
            AddAggregateTo(_aggregatesToUpdate, aggregate, oldVersion, eventsSinceLastSnapshot);

        private int AddAggregateTo(ICollection<AggregateDataSet<TKey, TVersion>> aggregatesToSave, IAggregateRoot<TKey, TVersion> aggregate, TVersion? oldVersion, int eventsSinceLastSnapshot)
        {
            var dataSet = _serializationStrategy.Serialize(aggregate, oldVersion, eventsSinceLastSnapshot);

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
