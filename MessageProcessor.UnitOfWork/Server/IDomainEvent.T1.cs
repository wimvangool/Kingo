using System;

namespace YellowFlare.MessageProcessing.Server
{
    public interface IDomainEvent<out TKey> where TKey : struct, IEquatable<TKey>
    {
        TKey AggregateKey
        {
            get;
        }

        int AggregateVersion
        {
            get;
        }
    }
}
