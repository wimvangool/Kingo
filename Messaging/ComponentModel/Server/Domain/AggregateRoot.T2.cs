using System.Resources;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace System.ComponentModel.Server.Domain
{
    /// <summary>
    /// Represents an aggregate that is modeled as a stream of events.
    /// </summary>
    /// <typeparam name="TKey">Type of the aggregate-key.</typeparam>
    /// <typeparam name="TVersion">Type of the aggregate-version.</typeparam>
    [Serializable]
    public abstract class AggregateRoot<TKey, TVersion> : IEventStream<TKey, TVersion>, IAggregateRoot<TKey, TVersion>, ISerializable
        where TKey : struct, IEquatable<TKey>
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>
    {
        private const string _BufferKey = "_buffer";
        private readonly MemoryEventStream<TKey, TVersion> _buffer;

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateRoot{TKey, TVersion}" /> class.
        /// </summary>
        protected AggregateRoot()
        {
            _buffer = new MemoryEventStream<TKey, TVersion>();
        }        

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateRoot{T, K}" /> class.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        protected AggregateRoot(SerializationInfo info, StreamingContext context)            
        {
            _buffer = (MemoryEventStream<TKey, TVersion>) info.GetValue(_BufferKey, typeof(MemoryEventStream<TKey, TVersion>));
        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            GetObjectData(info, context);
        }

        /// <summary>
        /// Populates a <see cref="SerializationInfo" /> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        protected virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {            
            info.AddValue(_BufferKey, _buffer);
        }

        TKey IAggregateRoot<TKey>.Key
        {
            get { return Key; }
        }

        TVersion IAggregateRoot<TKey, TVersion>.Version
        {
            get { return Version; }
        }

        /// <summary>
        /// Returns the key of this aggregate.
        /// </summary>
        protected abstract TKey Key
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
        protected void Publish<TEvent>(Func<TVersion, TEvent> eventFactory) where TEvent : class, IAggregateRootEvent<TKey, TVersion>, IMessage<TEvent>
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
        protected virtual void Publish<TEvent>(TEvent @event) where TEvent : class, IAggregateRootEvent<TKey, TVersion>, IMessage<TEvent>
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

        internal Exception NewNonMatchingAggregateKeyException<TEvent>(TEvent @event) where TEvent : class, IAggregateRootEvent<TKey, TVersion>
        {
            var messageFormat = ExceptionMessages.Aggregate_NonMatchingKey;
            var message = string.Format(messageFormat, Key, @event.AggregateKey);
            return new ArgumentException(message, "event");
        }
    }
}
