using System;
using Kingo.Resources;

namespace Kingo.Messaging.Domain
{
    /// <summary>
    /// Represents an aggregate root as defined by the principles of Domain Driven Design.
    /// </summary>
    /// <typeparam name="TKey">Type of the aggregate-key.</typeparam>
    /// <typeparam name="TVersion">Type of the aggregate-version.</typeparam>  
    [Serializable]
    public abstract class AggregateRoot<TKey, TVersion> : Entity<TKey>, IVersionedObject<TKey, TVersion>, IReadableEventStream<TKey, TVersion>        
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>
    {
        private static readonly Func<TVersion, TVersion> _IncrementMethod = AggregateRootVersion.IncrementMethod<TVersion>();

        [NonSerialized]
        private readonly MemoryEventStream<TKey, TVersion> _eventsToPublish;

        internal AggregateRoot()
        {
            _eventsToPublish = new MemoryEventStream<TKey, TVersion>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateRoot{T, S}" /> class.
        /// </summary>
        /// <param name="event">An event stream containing all events that need to be published.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="event"/> is <c>null</c>.
        /// </exception>
        protected AggregateRoot(IVersionedObject<TKey, TVersion> @event)
        {            
            _eventsToPublish = new MemoryEventStream<TKey, TVersion>();
            _eventsToPublish.Write(@event);
        }

        /// <summary>
        /// Returns the number of events that were published by this aggregate.
        /// </summary>
        protected int EventCount
        {
            get { return _eventsToPublish.Count; }
        }

        void IReadableEventStream<TKey, TVersion>.WriteTo(IWritableEventStream<TKey, TVersion> stream)
        {
            _eventsToPublish.WriteTo(stream);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return string.Format("{0} ({1} event(s) to be published.)", GetType().Name, EventCount);
        }        

        #region [====== Version ======]

        TVersion IVersionedObject<TKey, TVersion>.Version
        {
            get { return Version; }
        }        

        /// <summary>
        /// Gets or sets the version of this aggregate.
        /// </summary>
        protected abstract TVersion Version
        {
            get;
            set;
        }

        /// <summary>
        /// Returns a incremented version number relative to the current version.
        /// </summary>
        /// <returns>The increment version of the current version.</returns>
        protected virtual TVersion NextVersion()
        {
            return _IncrementMethod.Invoke(Version);
        }        

        #endregion                        

        #region [====== Publish & Apply ======]

        /// <summary>
        /// Publishes the specified <paramref name="event"/>.
        /// </summary>
        /// <typeparam name="TEvent">Type of the event that is published.</typeparam>
        /// <param name="event">The event to publish.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="event"/> is <c>null</c>.
        /// </exception>        
        protected virtual void Publish<TEvent>(TEvent @event) where TEvent : class, IVersionedObject<TKey, TVersion>, IMessage<TEvent>
        {
            _eventsToPublish.Write(@event);

            Apply(@event);            
        }

        internal virtual void Apply<TEvent>(TEvent @event) where TEvent : class, IVersionedObject<TKey, TVersion>
        {            
            if (@event.Key.Equals(Id))
            {
                if (Version.CompareTo(@event.Version) < 0)
                {
                    Version = @event.Version;
                    return;
                }
                throw NewInvalidVersionException(@event, Version);
            }
            throw NewInvalidKeyException(@event, Id);                                  
        }

        private static Exception NewInvalidKeyException<TEvent>(TEvent @event, TKey aggregateKey) where TEvent : class, IVersionedObject<TKey, TVersion>
        {
            var messageFormat = ExceptionMessages.AggregateRoot_InvalidKey;
            var message = string.Format(messageFormat, @event.Key, aggregateKey);
            return new ArgumentException(message, "event");
        }        

        private static Exception NewInvalidVersionException<TEvent>(TEvent @event, TVersion aggregateVersion) where TEvent : class, IVersionedObject<TKey, TVersion>
        {
            var messageFormat = ExceptionMessages.AggregateRoot_InvalidVersion;
            var message = string.Format(messageFormat, @event.Version, aggregateVersion);
            return new ArgumentException(message, "event");
        }

        #endregion
    }
}
