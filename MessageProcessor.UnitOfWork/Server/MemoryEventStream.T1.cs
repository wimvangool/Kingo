using System;
using System.Collections.Generic;

namespace YellowFlare.MessageProcessing.Server
{
    /// <summary>
    /// Represents an in-memory stream of <see cref="IDomainEvent{T}">domain events</see>.
    /// </summary>
    /// <typeparam name="TKey">Type of aggregate=key.</typeparam>
    public sealed class MemoryEventStream<TKey> : IBufferedEventStream<TKey>, IWritableEventStream<TKey>
        where TKey : struct, IEquatable<TKey>
    {
        private readonly List<IBufferedEvent<TKey>> _buffer;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryEventStream{T}" /> class.
        /// </summary>
        public MemoryEventStream()
        {
            _buffer = new List<IBufferedEvent<TKey>>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryEventStream{T}" /> class.
        /// </summary>
        /// <param name="capacity">The initial capacity of this buffer.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="capacity"/> is negative.
        /// </exception>
        public MemoryEventStream(int capacity)
        {
            _buffer = new List<IBufferedEvent<TKey>>(capacity);
        }

        /// <summary>
        /// Appends the specified event to this stream.
        /// </summary>
        /// <typeparam name="TDomainEvent">Type of event to append.</typeparam>
        /// <param name="domainEvent">Event to append.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="domainEvent"/> is <c>null</c>.
        /// </exception>
        public void Write<TDomainEvent>(TDomainEvent domainEvent) where TDomainEvent : class, IDomainEvent<TKey>
        {
            if (domainEvent == null)
            {
                throw new ArgumentNullException("domainEvent");
            }
            _buffer.Add(new BufferedEvent<TKey, TDomainEvent>(domainEvent));
        }

        /// <summary>
        /// Flushes all events of this stream to the specified stream.
        /// </summary>
        /// <param name="stream">The stream to flush to.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="stream"/> is <c>null</c>.
        /// </exception>
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
            _buffer.Clear();
        }
    }
}
