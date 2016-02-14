using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kingo.Messaging.Domain
{
    internal sealed class AggregateSet<TKey, TVersion, TAggregate>        
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>
        where TAggregate : class, IHasKeyAndVersion<TKey, TVersion>
    {
        private readonly Dictionary<TKey, Aggregate<TKey, TVersion, TAggregate>> _aggregates;

        internal AggregateSet()
        {
            _aggregates = new Dictionary<TKey, Aggregate<TKey, TVersion, TAggregate>>();
        }

        internal int Count
        {
            get { return _aggregates.Count; }
        }

        public override string ToString()
        {
            return _aggregates.ToString();
        }

        internal int CountUpdatedAggregates()
        {
            return _aggregates.Values.Count(aggregate => aggregate.HasBeenUpdated());
        }

        internal bool HasUpdates()
        {
            return _aggregates.Values.Any(aggregate => aggregate.HasBeenUpdated());
        }

        internal bool ContainsValue(TAggregate aggregate)
        {
            TAggregate other;

            if (TryGetValue(aggregate.Key, out other))
            {
                return ReferenceEquals(aggregate, other);
            }
            return false;
        }

        internal bool ContainsKey(TKey key)             
        {
            return _aggregates.ContainsKey(key);
        }

        internal bool TryGetValue(TKey key, out TAggregate aggregate)            
        {
            Aggregate<TKey, TVersion, TAggregate> aggregateWrapper;

            if (_aggregates.TryGetValue(key, out aggregateWrapper))
            {
                aggregate = aggregateWrapper.Value;
                return true;
            }
            aggregate = null;
            return false;
        }        
        
        internal void Add(TKey key, Aggregate<TKey, TVersion, TAggregate> aggregate)
        {
            _aggregates.Add(key, aggregate);
        }

        internal void RemoveByKey(TKey key)            
        {
            _aggregates.Remove(key);            
        }                

        internal Task CommitAsync(IWritableEventStream<TKey, TVersion> domainEventStream)
        {
            return Task.WhenAll(_aggregates.Values.ToArray().Select(aggregate => aggregate.CommitAsync(domainEventStream)));
        }        
    }
}
