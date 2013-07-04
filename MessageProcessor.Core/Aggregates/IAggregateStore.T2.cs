using System;

namespace YellowFlare.MessageProcessing.Aggregates
{
    public interface IAggregateStore<in TKey, TValue>
        where TKey : struct, IEquatable<TKey>
        where TValue : class, IAggregate<TKey>
    {
        bool TrySelect(TKey key, out TValue value);

        void Insert(TValue value);

        void Update(TValue value, AggregateVersion originalVersion);

        void Delete(TValue value);
    }
}
