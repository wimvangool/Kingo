using System;
using System.Collections;
using System.Collections.Generic;

namespace YellowFlare.MessageProcessing.Server
{
    internal sealed class AggregateSet<TKey, TVersion, TValue> : IEnumerable<Aggregate<TKey, TVersion, TValue>>
        where TKey : struct, IEquatable<TKey>
        where TVersion : struct, IAggregateVersion<TVersion>
        where TValue : class, IAggregate<TKey, TVersion>
    {
        private readonly Dictionary<TKey, Aggregate<TKey, TVersion, TValue>> _aggregates;

        public AggregateSet()
        {
            _aggregates = new Dictionary<TKey, Aggregate<TKey, TVersion, TValue>>(4);
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
            Aggregate<TKey, TVersion, TValue> aggregate;

            if (_aggregates.TryGetValue(key, out aggregate))
            {
                value = aggregate.Value;
                return true;
            }
            value = null;
            return false;
        }

        public void Add(TValue value)
        {
            _aggregates.Add(value.Key, new Aggregate<TKey, TVersion, TValue>(value));
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

        public IEnumerator<Aggregate<TKey, TVersion, TValue>> GetEnumerator()
        {
            return _aggregates.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
