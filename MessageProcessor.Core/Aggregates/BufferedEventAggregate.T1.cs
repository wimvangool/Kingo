using System;

namespace YellowFlare.MessageProcessing.Aggregates
{
    public abstract class BufferedEventAggregate<TKey> : IBufferedEventStream<TKey>, IAggregate<TKey>
        where TKey : struct, IEquatable<TKey>
    {
        private readonly MemoryEventStream<TKey> _buffer;

        protected BufferedEventAggregate()
        {
            _buffer = new MemoryEventStream<TKey>();
        }

        protected BufferedEventAggregate(int capacity)
        {
            _buffer = new MemoryEventStream<TKey>(capacity);
        }

        TKey IAggregate<TKey>.Key
        {
            get { return Key; }
        }

        AggregateVersion IAggregate<TKey>.Version
        {
            get { return Version; }
        }

        protected abstract TKey Key
        {
            get;
        }

        protected abstract AggregateVersion Version
        {
            get;
        }

        void IBufferedEventStream<TKey>.FlushTo(IWritableEventStream<TKey> stream)
        {
            _buffer.FlushTo(stream);
        }

        protected void Write<TDomainEvent>(TDomainEvent domainEvent) where TDomainEvent : class, IDomainEvent<TKey>
        {
            _buffer.Write(domainEvent);

            BufferedEventBus.Publish(domainEvent);
        }        
    }
}
