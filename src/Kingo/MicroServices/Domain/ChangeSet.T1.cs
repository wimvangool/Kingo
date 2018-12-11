using System;
using System.Collections.Generic;

namespace Kingo.MicroServices.Domain
{
    internal sealed class ChangeSet<TKey> : IChangeSet<TKey>
        where TKey : struct, IEquatable<TKey>
    {
        private readonly bool _storeSnapshots;
        private readonly bool _storeEvents;
        private readonly List<AggregateDataSet<TKey>> _aggregatesToInsert;
        private readonly List<AggregateDataSet<TKey>> _aggregatesToUpdate;
        private readonly List<TKey> _aggregatesToDelete;        

        public ChangeSet(AggregateSerializationStrategy serializationStrategy)
        {
            _storeSnapshots = serializationStrategy.UsesSnapshots();
            _storeEvents = serializationStrategy.UsesEvents();
            _aggregatesToInsert = new List<AggregateDataSet<TKey>>();
            _aggregatesToUpdate = new List<AggregateDataSet<TKey>>();
            _aggregatesToDelete = new List<TKey>();                
        }                
            
        public IReadOnlyCollection<AggregateDataSet<TKey>> AggregatesToInsert =>
            _aggregatesToInsert;

        public IReadOnlyCollection<AggregateDataSet<TKey>> AggregatesToUpdate =>
            _aggregatesToUpdate;

        public IReadOnlyCollection<TKey> AggregatesToDelete =>
            _aggregatesToDelete;       

        public void AddAggregateToInsert(IAggregateRoot<TKey> aggregate) =>
            AddAggregateTo(_aggregatesToInsert, aggregate);

        public void AddAggregateToUpdate(IAggregateRoot<TKey> aggregate) =>
            AddAggregateTo(_aggregatesToUpdate, aggregate);

        private void AddAggregateTo(ICollection<AggregateDataSet<TKey>> aggregates, IAggregateRoot<TKey> aggregate) =>
            aggregates.Add(ToDataSet(aggregate, aggregate.Commit()));       

        private AggregateDataSet<TKey> ToDataSet(IAggregateRoot<TKey> aggregate, IEnumerable<IEvent> events) =>
            new AggregateDataSet<TKey>(aggregate.Id, _storeSnapshots ? aggregate.TakeSnapshot() : null, _storeEvents ? events : null);

        public void AddAggregateToDelete(TKey id) =>
            _aggregatesToDelete.Add(id);

        public override string ToString() =>
            $"[Inserted: {_aggregatesToInsert.Count}, Updated: {_aggregatesToUpdate.Count}, Deleted: {_aggregatesToDelete.Count}]";
    }
}
