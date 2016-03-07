using System;
using System.Collections.Generic;
using System.Threading;
using Kingo.Resources;

namespace Kingo.Messaging.Domain
{
    /// <summary>
    /// Represents an aggregate root as defined by the principles of Domain Driven Design.
    /// </summary>
    /// <typeparam name="TKey">Type of the aggregate-key.</typeparam>
    /// <typeparam name="TVersion">Type of the aggregate-version.</typeparam>      
    [Serializable]
    public abstract class AggregateRoot<TKey, TVersion> : Entity<TKey>, IAggregateRoot<TKey, TVersion>, IMemento<TKey, TVersion>
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>       
    {
        private static readonly Func<TVersion, TVersion> _IncrementMethod = AggregateRootVersion.IncrementMethod<TVersion>();

        [NonSerialized]
        private List<IDomainEventToPublish<TKey, TVersion>> _eventsToPublish;        

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateRoot{T, S}" /> class.
        /// </summary>
        /// <param name="firstEvent">The event of that represents the creation of this aggregate.</param>        
        protected AggregateRoot(IDomainEvent<TKey, TVersion> firstEvent = null)
        {
            if (firstEvent != null)
            {
                EventsToPublish.Add(DomainEventToPublish<TKey, TVersion>.FromMessage(firstEvent));
            }
        }

        private List<IDomainEventToPublish<TKey, TVersion>> EventsToPublish
        {
            get
            {
                if (_eventsToPublish == null)
                {
                    _eventsToPublish = new List<IDomainEventToPublish<TKey, TVersion>>();
                }
                return _eventsToPublish;
            }
        }

        /// <summary>
        /// Returns the number of events that were published by this aggregate.
        /// </summary>
        protected int EventCount
        {
            get { return EventsToPublish.Count; }
        }

        void IAggregateRoot<TKey, TVersion>.Commit(IDomainEventBus<TKey, TVersion> eventBus)
        {
            Commit(eventBus, Interlocked.Exchange(ref _eventsToPublish, new List<IDomainEventToPublish<TKey, TVersion>>()));
        }

        private static void Commit(IDomainEventBus<TKey, TVersion> eventBus, IEnumerable<IDomainEventToPublish<TKey, TVersion>> events)
        {
            if (eventBus == null)
            {
                throw new ArgumentNullException(nameof(eventBus));
            }
            foreach (var @event in events)
            {
                @event.Publish(eventBus);
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{GetType().Name} ({EventCount} event(s) to be published.";
        }        

        #region [====== Version ======]

        TVersion IHasKeyAndVersion<TKey, TVersion>.Version
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

        #region [====== Snapshot ======]

        IMemento<TKey, TVersion> IAggregateRoot<TKey, TVersion>.CreateSnapshot()
        {
            return CreateSnapshot();
        }

        /// <summary>
        /// When overridden, creates and returns a <see cref="IMemento{TKey,TVersion}" /> of this aggregate.        
        /// </summary>
        /// <returns>A new snapshot of this aggregate.</returns>
        protected virtual IMemento<TKey, TVersion> CreateSnapshot()
        {
            return this;
        }

        IAggregateRoot<TKey, TVersion> IMemento<TKey, TVersion>.RestoreAggregate()
        {
            return this;
        }

        #endregion              

        #region [====== Publish & Apply ======]

        void IAggregateRoot<TKey, TVersion>.Publish<TEvent>(TEvent @event)
        {
            Publish(@event);
        }

        /// <summary>
        /// Publishes the specified <paramref name="event"/>.
        /// </summary>
        /// <typeparam name="TEvent">Type of the event that is published.</typeparam>
        /// <param name="event">The event to publish.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="event"/> is <c>null</c>.
        /// </exception>        
        protected virtual void Publish<TEvent>(TEvent @event) where TEvent : class, IDomainEvent<TKey, TVersion>
        {
            AssignIdAndVersionTo(@event);
            EventsToPublish.Add(new DomainEventToPublish<TKey, TVersion, TEvent>(@event));
            Apply(@event);            
        }

        private void AssignIdAndVersionTo(IDomainEvent<TKey, TVersion> @event)
        {
            if (@event == null)
            {
                throw new ArgumentNullException(nameof(@event));
            }
            @event.Key = Id;
            @event.Version = NextVersion();
        }

        internal virtual void Apply(IHasKeyAndVersion<TKey, TVersion> @event)
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

        private static Exception NewInvalidKeyException<TEvent>(TEvent @event, TKey aggregateKey) where TEvent : class, IHasKeyAndVersion<TKey, TVersion>
        {
            var messageFormat = ExceptionMessages.AggregateRoot_InvalidKey;
            var message = string.Format(messageFormat, @event.Key, aggregateKey);
            return new ArgumentException(message, nameof(@event));
        }        

        private static Exception NewInvalidVersionException<TEvent>(TEvent @event, TVersion aggregateVersion) where TEvent : class, IHasKeyAndVersion<TKey, TVersion>
        {
            var messageFormat = ExceptionMessages.AggregateRoot_InvalidVersion;
            var message = string.Format(messageFormat, @event.Version, aggregateVersion);
            return new ArgumentException(message, nameof(@event));
        }

        #endregion
    }
}
