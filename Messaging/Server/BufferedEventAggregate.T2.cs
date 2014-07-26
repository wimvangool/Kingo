namespace System.ComponentModel.Messaging.Server
{
    /// <summary>
    /// Represents an aggregate that is modeled as a stream of events.
    /// </summary>
    /// <typeparam name="TKey">Type of the aggregate-key.</typeparam>
    /// <typeparam name="TVersion">Type of the aggregate-version.</typeparam>
    public abstract class BufferedEventAggregate<TKey, TVersion> : IBufferedEventStream<TKey, TVersion>, IAggregate<TKey, TVersion>
        where TKey : struct, IEquatable<TKey>
        where TVersion : struct, IAggregateVersion<TVersion>
    {
        private readonly MemoryEventStream<TKey, TVersion> _buffer;

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedEventAggregate{TKey, TVersion}" /> class.
        /// </summary>
        protected BufferedEventAggregate()
        {
            _buffer = new MemoryEventStream<TKey, TVersion>();
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
            _buffer = new MemoryEventStream<TKey, TVersion>(capacity);
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

        void IBufferedEventStream<TKey, TVersion>.FlushTo(IWritableEventStream<TKey, TVersion> stream)
        {
            _buffer.FlushTo(stream);
        }

        /// <summary>
        /// Appends the specified event to this buffer.
        /// </summary>
        /// <typeparam name="TEvent">Type of the event to append.</typeparam>
        /// <param name="event">The event to append.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="event"/> is <c>null</c>.
        /// </exception>
        protected void Write<TEvent>(TEvent @event) where TEvent : class, IAggregateEvent<TKey, TVersion>
        {
            _buffer.Write(@event);

            BufferedEventBus.Publish(@event);
        }        
    }
}
