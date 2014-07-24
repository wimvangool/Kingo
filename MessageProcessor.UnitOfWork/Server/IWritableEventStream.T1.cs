using System;

namespace YellowFlare.MessageProcessing.Server
{
    public interface IWritableEventStream<in TKey>
        where TKey : struct, IEquatable<TKey>
    {
        void Write<TDomainEvent>(TDomainEvent domainEvent) where TDomainEvent : class, IDomainEvent<TKey>;
    }
}
