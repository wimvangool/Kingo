using System.Collections;
using System.Collections.Generic;

namespace System.ComponentModel.Server.Domain
{
    internal sealed class AggregateRootSet<TAggregate, TKey, TVersion> : IEnumerable<AggregateRootVersionTracker<TAggregate, TKey, TVersion>>
        where TAggregate : class, IVersionedObject<TKey, TVersion>
        where TKey : struct, IEquatable<TKey>
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>        
    {
        private readonly Dictionary<TKey, AggregateRootVersionTracker<TAggregate, TKey, TVersion>> _aggregates;

        public AggregateRootSet()
        {
            _aggregates = new Dictionary<TKey, AggregateRootVersionTracker<TAggregate, TKey, TVersion>>();
        }

        public int Count
        {
            get { return _aggregates.Count; }
        }

        public bool Contains(TKey key)
        {
            return _aggregates.ContainsKey(key);
        }

        public bool TryGetValue(TKey key, out TAggregate value)
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

        public void Add(TAggregate value)
        {
            _aggregates.Add(value.Key, new AggregateRootVersionTracker<TAggregate, TKey, TVersion>(value));
        }

        public void Remove(TKey key)
        {
            _aggregates.Remove(key);
        }

        public void MoveAggregateTo(TKey key, AggregateRootSet<TAggregate, TKey, TVersion> set)
        {            
            var source = _aggregates;
            var target = set._aggregates;

            target.Add(key, source[key]);

            source.Remove(key);
        }

        public void Clear()
        {
            _aggregates.Clear();
        }

        public IEnumerator<AggregateRootVersionTracker<TAggregate, TKey, TVersion>> GetEnumerator()
        {
            return _aggregates.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }        
    }
}
