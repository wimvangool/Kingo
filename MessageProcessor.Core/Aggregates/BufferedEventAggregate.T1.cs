using System;

namespace YellowFlare.MessageProcessing.Aggregates
{
    /// <summary>
    /// Represents an aggregate that is modeled as a stream of events.
    /// </summary>
    /// <typeparam name="TKey">Type of the aggregate-key.</typeparam>
    public abstract class BufferedEventAggregate<TKey> : IBufferedEventStream<TKey>, IAggregate<TKey>
        where TKey : struct, IEquatable<TKey>
    {
        private readonly MemoryEventStream<TKey> _buffer;

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedEventAggregate{T}" /> class.
        /// </summary>
        protected BufferedEventAggregate()
        {
            _buffer = new MemoryEventStream<TKey>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedEventAggregate{T}" /> class.
        /// </summary>
        /// <param name="capacity">The initial capacity of the event-buffer.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="capacity"/> is negative.
        /// </exception>
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

        /// <summary>
        /// Returns the key of this aggregate.
        /// </summary>
        public abstract TKey Key
        {
            get;
        }

        /// <summary>
        /// Returns the version of this aggregate.
        /// </summary>
        protected abstract AggregateVersion Version
        {
            get;
        }

        void IBufferedEventStream<TKey>.FlushTo(IWritableEventStream<TKey> stream)
        {
            _buffer.FlushTo(stream);
        }

        /// <summary>
        /// Appends the specified event to this buffer.
        /// </summary>
        /// <typeparam name="TDomainEvent">Type of the event to append.</typeparam>
        /// <param name="domainEvent">The event to append.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="domainEvent"/> is <c>null</c>.
        /// </exception>
        protected void Write<TDomainEvent>(TDomainEvent domainEvent) where TDomainEvent : class, IDomainEvent<TKey>
        {
            _buffer.Write(domainEvent);

            BufferedEventBus.Publish(domainEvent);
        }        
    }
}
