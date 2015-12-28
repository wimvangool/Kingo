using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kingo.Messaging.Domain
{
    internal sealed class AggregateSet<TKey, TVersion, TAggregate>        
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>
        where TAggregate : class, IVersionedObject<TKey, TVersion>
    {
        private readonly Dictionary<object, Aggregate<TKey, TVersion, TAggregate>> _aggregates;

        internal AggregateSet()
        {
            _aggregates = new Dictionary<object, Aggregate<TKey, TVersion, TAggregate>>();
        }

        internal int Count
        {
            get { return _aggregates.Count; }
        }

        internal int CountUpdatedAggregates()
        {
            return _aggregates.Values.Count(aggregate => aggregate.HasBeenUpdated());
        }

        internal bool HasUpdates()
        {
            return _aggregates.Values.Any(aggregate => aggregate.HasBeenUpdated());
        }

        internal bool Contains(TAggregate aggregate)
        {            
            return _aggregates.Values.Any(a => a.Matches(aggregate));
        }

        internal bool ContainsKey<T>(T key, Func<TAggregate, T> keySelector)             
        {
            return _aggregates.ContainsKey(key) || _aggregates.Values.Any(aggregate => HasKey(key, keySelector, aggregate));
        }

        internal bool TryGetValue<T>(T key, Func<TAggregate, T> keySelector, out TAggregate aggregate)            
        {
            return TryGetValueByKey(key, out aggregate) || TryGetValueFromValues(key, keySelector, out aggregate);
        }

        private bool TryGetValueByKey(object key, out TAggregate aggregate)
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

        private bool TryGetValueFromValues<T>(T key, Func<TAggregate, T> keySelector, out TAggregate aggregate)            
        {
            var aggregateWrapper = _aggregates.Values.FirstOrDefault(a => HasKey(key, keySelector, a));
            if (aggregateWrapper == null)
            {
                aggregate = null;
                return false;
            }
            aggregate = aggregateWrapper.Value;
            return true;
        }
        
        internal void Add(object key, Aggregate<TKey, TVersion, TAggregate> aggregate)
        {
            _aggregates.Add(key, aggregate);
        }

        internal void RemoveByKey<T>(T key, Func<TAggregate, T> keySelector)            
        {
            object otherKey;

            if (_aggregates.Remove(key))
            {
                return;
            }            
            if (TryGetKey(key, keySelector, out otherKey))
            {
                _aggregates.Remove(otherKey);
            }
        }

        private bool TryGetKey<T>(T key, Func<TAggregate, T> keySelector, out object otherKey)            
        {
            foreach (var keyValuePair in _aggregates)
            {
                if (HasKey(key, keySelector, keyValuePair.Value))
                {
                    otherKey = keyValuePair.Key;
                    return true;
                }
            }
            otherKey = null;
            return false;
        }

        private static bool HasKey<T>(T key, Func<TAggregate, T> keySelector, Aggregate<TKey, TVersion, TAggregate> aggregate)            
        {
            if (aggregate.Value == null)
            {
                return false;
            }
            return keySelector.Invoke(aggregate.Value).Equals(key);
        }

        internal Task CommitAsync(IWritableEventStream<TKey, TVersion> domainEventStream)
        {
            return Task.WhenAll(_aggregates.Values.ToArray().Select(aggregate => aggregate.CommitAsync(domainEventStream)));
        }        
    }
}
