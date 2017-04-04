using System;
using System.Collections.Generic;

namespace Kingo.Messaging.Domain
{
    internal sealed class ChangeSet<TKey> : IChangeSet<TKey>
        where TKey : struct, IEquatable<TKey>
    {        
        private readonly List<AggregateData<TKey>> _aggregatesToInsert;
        private readonly List<AggregateData<TKey>> _aggregatesToUpdate;
        private readonly List<TKey> _aggregatesToDelete;        

        public ChangeSet()
        {
            _aggregatesToInsert = new List<AggregateData<TKey>>();
            _aggregatesToUpdate = new List<AggregateData<TKey>>();
            _aggregatesToDelete = new List<TKey>();            
        }                
            
        public IReadOnlyCollection<AggregateData<TKey>> AggregatesToInsert =>
            _aggregatesToInsert;

        public IReadOnlyCollection<AggregateData<TKey>> AggregatesToUpdate =>
            _aggregatesToUpdate;

        public IReadOnlyCollection<TKey> AggregatesToDelete =>
            _aggregatesToDelete;

        public void AddAggregateToInsert(IAggregateRoot<TKey> aggregate, IEnumerable<IEvent> events) =>
            _aggregatesToInsert.Add(new AggregateData<TKey>(aggregate.Id, aggregate.TakeSnapshot(), events));

        public void AddAggregateToUpdate(IAggregateRoot<TKey> aggregate, IEnumerable<IEvent> events) =>
            _aggregatesToUpdate.Add(new AggregateData<TKey>(aggregate.Id, aggregate.TakeSnapshot(), events));

        public void AddAggregateToDelete(TKey id) =>
            _aggregatesToDelete.Add(id);

        public override string ToString() =>
            $"[Inserted: {_aggregatesToInsert.Count}, Updated: {_aggregatesToUpdate.Count}, Deleted: {_aggregatesToDelete.Count}]";
    }
}
