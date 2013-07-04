using System;

namespace YellowFlare.MessageProcessing.Aggregates
{
    public interface IAggregate<out TKey> where TKey : struct, IEquatable<TKey>
    {
        TKey Key
        {
            get;
        }

        AggregateVersion Version
        {
            get;
        }
    }
}
