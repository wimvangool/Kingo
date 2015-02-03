using System.Collections;
using System.Collections.Generic;

namespace System.ComponentModel.Server.Domain
{
    internal sealed class AggregateSet<TKey, TVersion, TValue> : IEnumerable<AggregateVersionTracker<TKey, TVersion, TValue>>
        where TKey : struct, IEquatable<TKey>
        where TVersion : struct, IEquatable<TVersion>
        where TValue : class, IAggregate<TKey, TVersion>
    {
        private readonly Dictionary<TKey, AggregateVersionTracker<TKey, TVersion, TValue>> _aggregates;

        public AggregateSet()
        {
            _aggregates = new Dictionary<TKey, AggregateVersionTracker<TKey, TVersion, TValue>>(4);
        }

        public int Count
        {
            get { return _aggregates.Count; }
        }

        public bool Contains(TKey key)
        {
            return _aggregates.ContainsKey(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            AggregateVersionTracker<TKey, TVersion, TValue> aggregate;

            if (_aggregates.TryGetValue(key, out aggregate))
            {
                value = aggregate.Aggregate;
                return true;
            }
            value = null;
            return false;
        }

        public void Add(TValue value)
        {
            _aggregates.Add(value.Key, new AggregateVersionTracker<TKey, TVersion, TValue>(value));
        }

        public void Remove(TKey key)
        {
            _aggregates.Remove(key);
        }

        public void MoveAggregateTo(TKey key, AggregateSet<TKey, TVersion, TValue> set)
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

        public IEnumerator<AggregateVersionTracker<TKey, TVersion, TValue>> GetEnumerator()
        {
            return _aggregates.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }        
    }
}
