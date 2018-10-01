using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Kingo.Resources;

namespace Kingo.Messaging.Domain
{
    /// <summary>
    /// Serves as a base-class implementation of the <see cref="IAggregateRoot{T}" /> interface.
    /// </summary>
    /// <typeparam name="TKey">Type of the identifier of this aggregate.</typeparam>
    /// <typeparam name="TVersion">Type of the version of this aggregate.</typeparam>
    public abstract class AggregateRoot<TKey, TVersion> : IAggregateRoot<TKey>
        where TKey : struct, IEquatable<TKey>
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>
    {        
        private readonly TKey _id;
        private TVersion _version;

        private List<IEvent> _events;
        private bool _wasRemovedFromRepository;

        private AggregateRoot(IAggregateDataObject<TKey, TVersion> snapshotOrEvent)
        {                        
            if (snapshotOrEvent == null)
            {
                throw new ArgumentNullException(nameof(snapshotOrEvent));
            }            
            _id = snapshotOrEvent.Id;
            _version = snapshotOrEvent.Version;    
            _events = new List<IEvent>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateRoot{T, S}" /> class.
        /// </summary>        
        /// <param name="snapshot">The event that defines the creation of this aggregate.</param>       
        /// <exception cref="ArgumentNullException">
        /// <paramref name="snapshot" /> is <c>null</c>.
        /// </exception>
        protected AggregateRoot(ISnapshot<TKey, TVersion> snapshot) :
            this(snapshot as IAggregateDataObject<TKey, TVersion>) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateRoot{T, S}" /> class.
        /// </summary>        
        /// <param name="event">The event that defines the creation of this aggregate.</param>  
        /// <param name="isNewAggregate">
        /// Indicates whether or not this aggregate is a new aggregate. If <c>true</c>, the specified
        /// <paramref name="event"/> is immediately added to the event buffer of this aggregate.
        /// </param>     
        /// <exception cref="ArgumentNullException">
        /// <paramref name="event" /> is <c>null</c>.
        /// </exception>
        protected AggregateRoot(IEvent<TKey, TVersion> @event, bool isNewAggregate) :
            this(@event)
        {
            if (isNewAggregate)
            {
                _events.Add(@event);
            }
        }

        /// <inheritdoc />
        public override string ToString() =>
            $"{GetType().FriendlyName()} [Id = {Id}, Version = {Version}, Events = {_events.Count}]";        

        #region [====== Id & Version ======]

        TKey IAggregateRoot<TKey>.Id =>
            Id;

        /// <summary>
        /// The identifier of this aggregate.
        /// </summary>
        protected TKey Id =>
            _id;

        /// <summary>
        /// The version of this aggregate.
        /// </summary>        
        protected TVersion Version
        {
            get => _version;
            private set
            {
                var oldVersion = _version;
                var newVersion = value;

                if (newVersion.CompareTo(oldVersion) <= 0)
                {
                    throw NewVersionUpdateException(oldVersion, newVersion);
                }
                _version = newVersion;
            }
        }

        /// <summary>
        /// When implemented, returns a version that is newer or higher that the current version of this aggregate.
        /// </summary>
        /// <returns>A newer or higher value than <see cref="Version"/>.</returns>
        protected abstract TVersion NextVersion();

        private static Exception NewVersionUpdateException(TVersion oldVersion, TVersion newVersion)
        {
            var messageFormat = ExceptionMessages.AggregateRoot_VersionUpdateException;
            var message = string.Format(messageFormat, newVersion, oldVersion);
            return new ArgumentOutOfRangeException(nameof(newVersion), message);
        }

        #endregion

        #region [====== EventHandlers ======]  

        /// <summary>
        /// Represents a collection of event handler delegates that are used by an <see cref="AggregateRoot{T, S}" /> to
        /// apply specific events to itself.
        /// </summary>        
        protected sealed class EventHandlerCollection            
        {
            private readonly Dictionary<Type, Action<object>> _eventHandlers;
            private readonly AggregateRoot<TKey, TVersion> _aggregate;

            internal EventHandlerCollection(AggregateRoot<TKey, TVersion> aggregate)
            {
                _eventHandlers = new Dictionary<Type, Action<object>>();
                _aggregate = aggregate;
            }

            /// <summary>
            /// Indicates whether or not any handlers were registered with this collection.
            /// </summary>
            public bool IsEmpty =>
                _eventHandlers.Count == 0;

            /// <inheritdoc />
            public override string ToString() =>
                $"{ _eventHandlers.Count } handler(s) registered.";

            #region [====== Register ======]

            /// <summary>
            /// Adds the specified <paramref name="eventHandler"/> to this collection and returns the collection
            /// where this handler has been added.
            /// </summary>
            /// <typeparam name="TEvent">Type of the event that is handled by the specified <paramref name="eventHandler"/>.</typeparam>
            /// <param name="eventHandler">The event handler to add.</param>
            /// <returns>The collection to which the specified <paramref name="eventHandler"/> has been added.</returns>
            /// <exception cref="ArgumentNullException">
            /// <paramref name="eventHandler"/> is <c>null</c>.
            /// </exception>
            /// <exception cref="ArgumentException">
            /// A handler for the specified event type <typeparamref name="TEvent"/> has already been added.
            /// </exception>
            public EventHandlerCollection Register<TEvent>(Action<TEvent> eventHandler)
                where TEvent : IEvent<TKey, TVersion>
            {
                if (eventHandler == null)
                {
                    throw new ArgumentNullException(nameof(eventHandler));
                }
                try
                {
                    _eventHandlers.Add(typeof(TEvent), @event => eventHandler.Invoke((TEvent)@event));
                }
                catch (ArgumentException)
                {
                    throw NewHandlerForEventTypeAlreadyRegisteredException(typeof(TEvent));
                }
                return this;
            }

            private static Exception NewHandlerForEventTypeAlreadyRegisteredException(Type type)
            {
                var messageFormat = ExceptionMessages.AggregateRoot_HandlerForEventTypeAlreadyAdded;
                var message = string.Format(messageFormat, type.FriendlyName());
                return new ArgumentException(message);
            }

            #endregion

            #region [====== Apply ======]            

            internal void ApplyOld(IEnumerable<IEvent> events)
            {
                if (events == null)
                {
                    throw new ArgumentNullException(nameof(events));
                }
                foreach (var @event in SelectEventsToApply(events).OrderBy(e => e.Version))
                {                    
                    if (Apply(@event))
                    {
                        _aggregate.Version = @event.Version;
                        continue;
                    }
                    throw NewMissingEventHandlerException(@event.GetType());
                }
            }

            private IEnumerable<IEvent<TKey, TVersion>> SelectEventsToApply(IEnumerable<IEvent> events) =>
                from @event in events.WhereNotNull().Select(Convert)
                where MustApply(@event)
                select @event;

            private static IEvent<TKey, TVersion> Convert(IEvent @event)
            {
                try
                {
                    return (IEvent<TKey, TVersion>) @event;
                }
                catch (InvalidCastException)
                {
                    throw NewEventConversionException(@event.GetType(), typeof(IEvent<TKey, TVersion>), nameof(IEvent.UpdateToLatestVersion));
                }
            }

            private bool MustApply(IEvent<TKey, TVersion> @event) =>
                @event.Id.Equals(_aggregate.Id) && @event.Version.CompareTo(_aggregate.Version) > 0;                      

            internal bool Apply(IEvent<TKey, TVersion> @event)
            {                
                if (_eventHandlers.TryGetValue(@event.GetType(), out var eventHandler))
                {
                    eventHandler.Invoke(@event);
                    return true;
                }
                return false;
            }                                    

            private static Exception NewEventConversionException(Type sourceType, Type targetType, string updateToLatestVersionMethodName)
            {
                var messageFormat = ExceptionMessages.AggregateRoot_EventConversionException;
                var message = string.Format(messageFormat, sourceType.FriendlyName(), targetType.FriendlyName(), updateToLatestVersionMethodName);
                return new ArgumentException(message);
            }           

            #endregion
        }

        private EventHandlerCollection _eventHandlers;

        private EventHandlerCollection EventHandlers
        {
            get
            {
                if (_eventHandlers == null)
                {
                    _eventHandlers = RegisterEventHandlers(new EventHandlerCollection(this));
                }
                return _eventHandlers;
            }
        }

        /// <summary>
        /// Adds all event handler delegates to the specified <paramref name="eventHandlers"/> and returns the resulting collection.
        /// </summary>
        /// <param name="eventHandlers">A collection of event handler delegates.</param>
        /// <returns>A completed collection of event handlers that is used by this aggregate to apply and/or replay certain events.</returns>
        protected virtual EventHandlerCollection RegisterEventHandlers(EventHandlerCollection eventHandlers) =>
            eventHandlers;

        #endregion

        #region [====== LoadFromHistory & TakeSnapshot =====]

        void IAggregateRoot.LoadFromHistory(IEnumerable<IEvent> events) =>
            LoadFromHistory(events);

        /// <summary>
        /// Reloads the state of this aggregate by replaying all specified <paramref name="events" />. If you override this method,
        /// make sure you always call the base-class implementation.
        /// </summary>
        /// <param name="events">The events to replay.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="events"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// This aggregate does not recognize one of the events.
        /// </exception>
        protected virtual void LoadFromHistory(IEnumerable<IEvent> events) =>
            EventHandlers.ApplyOld(events);
        
        ISnapshot IAggregateRoot.TakeSnapshot() =>
            TakeSnapshot();

        /// <summary>
        /// When overridden, creates and returns a snapshot of the current state of this aggregate.
        /// </summary>
        /// <returns>A snapshot of the current state of this aggregate.</returns>
        /// <exception cref="NotSupportedException">
        /// This aggregate does not support snapshots.
        /// </exception>
        protected virtual ISnapshot<TKey, TVersion> TakeSnapshot() =>
            throw NewSnapshotsNotSupportedException();

        private Exception NewSnapshotsNotSupportedException()
        {
            var messageFormat = ExceptionMessages.AggregateRoot_SnapshotsNotSupported;
            var message = string.Format(messageFormat, GetType().FriendlyName());
            return new NotSupportedException(message);
        }

        #endregion

        #region [====== Publish ======]  

        private EventHandler<EventPublishedEventArgs> _eventPublished;

        event EventHandler<EventPublishedEventArgs> IAggregateRoot.EventPublished
        {
            add => _eventPublished += value;
            remove => _eventPublished -= value;
        }               

        /// <summary>
        /// Updates the version of this aggregate and raises the <see cref="_eventPublished"/> event.
        /// </summary>        
        /// <param name="event">The event that was published.</param>
        protected virtual void OnEventPublished<TEvent>(TEvent @event) where TEvent : IEvent<TKey, TVersion>
        {
            Version = @event.Version;

            _events.Add(@event);
            _eventPublished.Raise(this, new EventPublishedEventArgs(@event));
        }

        IReadOnlyList<IEvent> IAggregateRoot.Events =>
            _events;

        /// <summary>
        /// Publishes the specified <paramref name="event"/> and updates the version of this aggregate.
        /// </summary>
        /// <typeparam name="TEvent">Type of the published event.</typeparam>
        /// <param name="event">The event to publish.</param>        
        /// <exception cref="BusinessRuleException">
        /// This aggregate has already been removed from the repository and therefore does not allow any further changes.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="event"/> is <c>null</c>.
        /// </exception>
        protected void Publish<TEvent>(TEvent @event) where TEvent : IEvent<TKey, TVersion>
        {
            if (_wasRemovedFromRepository)
            {
                throw NewAggregateRemovedException(GetType(), @event?.GetType());
            }
            if (@event == null)
            {
                throw new ArgumentNullException(nameof(@event));
            }
            @event.Id = Id;
            @event.Version = NextVersion();

            if (EventHandlers.Apply(@event) || EventHandlers.IsEmpty)
            {
                OnEventPublished(@event);
                return;
            }
            throw NewMissingEventHandlerException(@event.GetType());                                  
        }

        IReadOnlyList<IEvent> IAggregateRoot.Commit() =>
            Interlocked.Exchange(ref _events, new List<IEvent>());

        private static Exception NewMissingEventHandlerException(Type eventType)
        {
            var messageFormat = ExceptionMessages.AggregateRoot_MissingEventHandlerException;
            var message = string.Format(messageFormat, eventType.FriendlyName());
            return new ArgumentException(message);
        }

        #endregion

        #region [====== NotifyRemoved ======]

        void IAggregateRoot.NotifyRemoved()
        {
            try
            {
                OnRemoved();
            }
            finally
            {
                _wasRemovedFromRepository = true;
            }
        }

        /// <summary>
        /// This method is called when this aggregate was removed from the repository. It can be used
        /// to publish some last-minute events representing the removal of this aggregate and the end
        /// of its lifetime.
        /// </summary>
        protected virtual void OnRemoved() { }

        private static Exception NewAggregateRemovedException(Type aggregateType, Type eventType)
        {
            var messageFormat = ExceptionMessages.AggregateRoot_AggregateRemovedException;
            var message = string.Format(messageFormat, aggregateType.FriendlyName(), eventType?.FriendlyName());
            return new BusinessRuleException(message);
        }

        #endregion
    }
}
