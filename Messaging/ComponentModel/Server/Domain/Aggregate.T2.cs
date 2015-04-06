using System.Resources;

namespace System.ComponentModel.Server.Domain
{
    /// <summary>
    /// Represents an aggregate that is modeled as a stream of events.
    /// </summary>
    /// <typeparam name="TKey">Type of the aggregate-key.</typeparam>
    /// <typeparam name="TVersion">Type of the aggregate-version.</typeparam>
    public abstract class Aggregate<TKey, TVersion> : IEventStream<TKey, TVersion>, IAggregate<TKey, TVersion>
        where TKey : struct, IEquatable<TKey>
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>
    {
        private readonly MemoryEventStream<TKey, TVersion> _buffer;

        /// <summary>
        /// Initializes a new instance of the <see cref="Aggregate{TKey, TVersion}" /> class.
        /// </summary>
        protected Aggregate()
        {
            _buffer = new MemoryEventStream<TKey, TVersion>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Aggregate{TKey, TVersion}" /> class.
        /// </summary>
        /// <param name="capacity">The initial capacity of the event-buffer.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="capacity"/> is negative.
        /// </exception>
        protected Aggregate(int capacity)
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
            set;
        }

        /// <summary>
        /// Increments the specified <paramref name="version"/> and returns the result.
        /// </summary>
        /// <returns>The incremented value.</returns>
        protected abstract TVersion Increment(TVersion version);

        void IEventStream<TKey, TVersion>.FlushTo(IWritableEventStream<TKey, TVersion> stream)
        {
            _buffer.FlushTo(stream);
        }

        /// <summary>
        /// Appends the event that is created using the specified <paramref name="eventFactory"/>
        /// to the aggregate's buffer and publishes it.
        /// </summary>
        /// <typeparam name="TEvent">Type of the event that is created and written.</typeparam>
        /// <param name="eventFactory">The factory that is used to created the event.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="eventFactory"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The method is not being called inside a <see cref="UnitOfWorkScope" />.
        /// </exception>
        protected void Publish<TEvent>(Func<TVersion, TEvent> eventFactory) where TEvent : class, IAggregateEvent<TKey, TVersion>, IMessage<TEvent>
        {
            if (eventFactory == null)
            {
                throw new ArgumentNullException("eventFactory");
            }
            Publish(eventFactory.Invoke(Increment(Version)));
        }

        /// <summary>
        /// Appends the specified event to the aggregate's buffer and publishes it.
        /// </summary>
        /// <typeparam name="TEvent">Type of the event to append.</typeparam>
        /// <param name="event">The event to append.</param>        
        /// <exception cref="ArgumentNullException">
        /// <paramref name="event"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The method is not being called inside a <see cref="UnitOfWorkScope" />.
        /// </exception>
        protected virtual void Publish<TEvent>(TEvent @event) where TEvent : class, IAggregateEvent<TKey, TVersion>, IMessage<TEvent>
        {
            if (@event == null)
            {
                throw new ArgumentNullException("event");
            }
            if (@event.AggregateKey.Equals(Key))
            {
                Version = @event.AggregateVersion;

                _buffer.Write(@event);

                MessageProcessor.Publish(@event);
                return;
            }
            throw NewNonMatchingAggregateKeyException(@event);
        }

        internal Exception NewNonMatchingAggregateKeyException<TEvent>(TEvent @event) where TEvent : class, IAggregateEvent<TKey, TVersion>
        {
            var messageFormat = ExceptionMessages.Aggregate_NonMatchingKey;
            var message = string.Format(messageFormat, Key, @event.AggregateKey);
            return new ArgumentException(message, "event");
        }
    }
}
