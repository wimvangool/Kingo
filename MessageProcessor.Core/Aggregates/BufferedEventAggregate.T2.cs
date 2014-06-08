using System;

namespace YellowFlare.MessageProcessing.Aggregates
{
    /// <summary>
    /// Represents an aggregate that is modeled as a stream of events.
    /// </summary>
    /// <typeparam name="TKey">Type of the aggregate-key.</typeparam>
    /// <typeparam name="TVersion">Type of the aggregate-version.</typeparam>
    public abstract class BufferedEventAggregate<TKey, TVersion> : IBufferedEventStream<TKey>, IAggregate<TKey, TVersion>
        where TKey : struct, IEquatable<TKey>
        where TVersion : struct, IAggregateVersion<TVersion>
    {
        private readonly MemoryEventStream<TKey> _buffer;

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedEventAggregate{TKey, TVersion}" /> class.
        /// </summary>
        protected BufferedEventAggregate()
        {
            _buffer = new MemoryEventStream<TKey>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedEventAggregate{TKey, TVersion}" /> class.
        /// </summary>
        /// <param name="capacity">The initial capacity of the event-buffer.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="capacity"/> is negative.
        /// </exception>
        protected BufferedEventAggregate(int capacity)
        {
            _buffer = new MemoryEventStream<TKey>(capacity);
        }

        TKey IAggregate<TKey, TVersion>.Key
        {
            get { return Key; }
        }

        TVersion IAggregate<TKey, TVersion>.Version
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
        protected abstract TVersion Version
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
