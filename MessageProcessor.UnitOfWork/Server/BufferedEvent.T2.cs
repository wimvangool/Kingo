using System;

namespace YellowFlare.MessageProcessing.Server
{
    internal sealed class BufferedEvent<TKey, TDomainEvent> : IBufferedEvent<TKey>
        where TKey : struct, IEquatable<TKey>
        where TDomainEvent : class, IDomainEvent<TKey>
    {
        private readonly TDomainEvent _domainEvent;

        public BufferedEvent(TDomainEvent domainEvent)
        {            
            _domainEvent = domainEvent;
        }

        public void WriteTo(IWritableEventStream<TKey> stream)
        {
            stream.Write(_domainEvent);
        }
    }
}
