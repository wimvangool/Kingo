using System;
using System.Collections;
using System.Collections.Generic;

namespace YellowFlare.MessageProcessing.Aggregates
{
    internal sealed class AggregateSet<TKey, TValue> : IEnumerable<Aggregate<TKey, TValue>>
        where TKey : struct, IEquatable<TKey>
        where TValue : class, IAggregate<TKey>
    {
        private readonly Dictionary<TKey, Aggregate<TKey, TValue>> _aggregates;

        public AggregateSet()
        {
            _aggregates = new Dictionary<TKey, Aggregate<TKey, TValue>>(4);
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
            Aggregate<TKey, TValue> aggregate;

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
            _aggregates.Add(value.Key, new Aggregate<TKey, TValue>(value));
        }

        public void Remove(TKey key)
        {
            _aggregates.Remove(key);
        }

        public void MoveAggregateTo(TKey key, AggregateSet<TKey, TValue> set)
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

        public IEnumerator<Aggregate<TKey, TValue>> GetEnumerator()
        {
            return _aggregates.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
