using System;
using System.Collections.Generic;
using System.Linq;

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
            
        public IReadOnlyCollection<AggregateDataSet<TKey, TVersion>> AggregatesToInsert =>
            _aggregatesToInsert;

        public IReadOnlyCollection<AggregateDataSet<TKey, TVersion>> AggregatesToUpdate =>
            _aggregatesToUpdate;

        public IReadOnlyCollection<TKey> AggregatesToDelete =>
            _aggregatesToDelete;

        public int AddAggregateToInsert(IAggregateRoot<TKey, TVersion> aggregate) =>
            _serializationStrategy.AddAggregateTo(_aggregatesToInsert, aggregate, null, 0);

        public int AddAggregateToUpdate(IAggregateRoot<TKey, TVersion> aggregate, TVersion oldVersion, int eventsSinceLastSnapshot) =>
            _serializationStrategy.AddAggregateTo(_aggregatesToUpdate, aggregate, oldVersion, eventsSinceLastSnapshot);

        public void AddAggregateToDelete(TKey id) =>
            _aggregatesToDelete.Add(id);

        public override string ToString() =>
            $"[Inserted: {_aggregatesToInsert.Count}, Updated: {_aggregatesToUpdate.Count}, Deleted: {_aggregatesToDelete.Count}]";
    }
}
