using System;
using System.Collections.Generic;
using System.Linq;

namespace ServiceComponents.ComponentModel.Server.Domain
{
    internal sealed class AggregateRootSet<TAggregate, TKey, TVersion>
        where TAggregate : class, IVersionedObject<TKey, TVersion>
        where TKey : struct, IEquatable<TKey>
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>        
    {
        private Dictionary<TKey, AggregateRootVersionTracker<TAggregate, TKey, TVersion>> _aggregates;

        internal AggregateRootSet()
        {
            _aggregates = new Dictionary<TKey, AggregateRootVersionTracker<TAggregate, TKey, TVersion>>();
        }

        internal IEnumerable<TKey>  Keys
        {
            get { return _aggregates.Keys; }
        }

        internal IEnumerable<AggregateRootVersionTracker<TAggregate, TKey, TVersion>> Trackers
        {
            get { return _aggregates.Values; }
        }

        internal int Count
        {
            get { return _aggregates.Count; }
        }

        internal bool Contains(TKey key)
        {
            return _aggregates.ContainsKey(key);
        }

        internal bool Contains(TAggregate aggregate)
        {
            return Trackers.Any(tracker => ReferenceEquals(tracker.Aggregate, aggregate));
        }

        internal bool TryGetValue(TKey key, out TAggregate value)
        {
            AggregateRootVersionTracker<TAggregate, TKey, TVersion> aggregate;

            if (_aggregates.TryGetValue(key, out aggregate))
            {
                value = aggregate.Aggregate;
                return true;
            }
            value = null;
            return false;
        }

        internal void Add(TAggregate value)
        {
            _aggregates.Add(value.Key, new AggregateRootVersionTracker<TAggregate, TKey, TVersion>(value));
        }

        internal void Add(TKey key)
        {
            _aggregates.Add(key, null);
        }

        internal void Remove(TKey key)
        {
            _aggregates.Remove(key);
        }

        internal void CopyAggregateTo(TKey key, AggregateRootSet<TAggregate, TKey, TVersion> set)
        {            
            var source = _aggregates;
            var target = set._aggregates;

            target.Add(key, source[key]);            
        }

        internal void CommitUpdates()
        {
            _aggregates = CommitUpdatesOfAggregates();
        }

        private Dictionary<TKey, AggregateRootVersionTracker<TAggregate, TKey, TVersion>> CommitUpdatesOfAggregates()
        {            
            var updatedDictionary = new Dictionary<TKey, AggregateRootVersionTracker<TAggregate, TKey, TVersion>>();

            foreach (var keyValuePair in _aggregates)
            {
                updatedDictionary.Add(keyValuePair.Key, keyValuePair.Value.CommitUpdates());
            }
            return updatedDictionary;
        }

        internal void Clear()
        {
            _aggregates.Clear();
        }              
    }
}
