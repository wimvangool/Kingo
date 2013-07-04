using System;
using System.Collections.Generic;

namespace YellowFlare.MessageProcessing.Aggregates
{
    public sealed class MemoryEventStream<TKey> : IBufferedEventStream<TKey>, IWritableEventStream<TKey>
        where TKey : struct, IEquatable<TKey>
    {
        private readonly List<IBufferedEvent<TKey>> _buffer;

        public MemoryEventStream()
        {
            _buffer = new List<IBufferedEvent<TKey>>();
        }

        public MemoryEventStream(int capacity)
        {
            _buffer = new List<IBufferedEvent<TKey>>(capacity);
        }

        public void Write<TDomainEvent>(TDomainEvent domainEvent) where TDomainEvent : class, IDomainEvent<TKey>
        {
            if (domainEvent == null)
            {
                throw new ArgumentNullException("domainEvent");
            }
            _buffer.Add(new BufferedEvent<TKey, TDomainEvent>(domainEvent));
        }

        public void FlushTo(IWritableEventStream<TKey> stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }
            foreach (var bufferedEvent in _buffer)
            {
                bufferedEvent.WriteTo(stream);
            }
        }
    }
}
