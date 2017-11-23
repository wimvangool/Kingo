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
        private List<IEvent> _pendingEvents;
        private bool _wasRemovedFromRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateRoot{T, S}" /> class.
        /// </summary>
        /// <param name="aggregateEvent">The event that defines the creation of this aggregate.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="aggregateEvent" /> is <c>null</c>.
        /// </exception>
        protected AggregateRoot(IAggregateEvent<TKey, TVersion> aggregateEvent)
        {
            if (aggregateEvent == null)
            {
                throw new ArgumentNullException(nameof(aggregateEvent));
            }
            _id = aggregateEvent.AggregateId;
            _version = aggregateEvent.AggregateVersion;
            _pendingEvents = new List<IEvent> { aggregateEvent };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateRoot{T, S}" /> class.
        /// </summary>
        /// <param name="snapshot">A snapshot of this aggregate.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="snapshot"/> is <c>null</c>.
        /// </exception>
        protected AggregateRoot(ISnapshot<TKey, TVersion> snapshot)
        {
            if (snapshot == null)
            {
                throw new ArgumentNullException(nameof(snapshot));
            }
            _id = snapshot.AggregateId;
            _version = snapshot.AggregateVersion;
            _pendingEvents = new List<IEvent>();
        }

        /// <inheritdoc />
        public override string ToString() =>
            $"{GetType().FriendlyName()} [Id = {Id}, Version = {Version}, Events = {PublishedEventCount}]";

        private int PublishedEventCount =>
            _pendingEvents?.Count ?? 0;

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
        /// Represents a collection of event handler degelates that are used by an <see cref="AggregateRoot{T, S}" /> to
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
                where TEvent : IAggregateEvent<TKey, TVersion>
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
                foreach (var @event in UpdateToLatestVersion(events).OrderBy(e => e.AggregateVersion))
                {                    
                    if (Apply(@event))
                    {
                        _aggregate.Version = @event.AggregateVersion;
                        continue;
                    }
                    throw NewMissingEventHandlerException(@event.GetType());
                }
            }

            private static IEnumerable<IAggregateEvent<TKey, TVersion>> UpdateToLatestVersion(IEnumerable<IEvent> events)
            {
                if (events == null)
                {
                    throw new ArgumentNullException(nameof(events));
                }
                var updatedEvents = events.WhereNotNull().Select(@event => @event.UpdateToLatestVersion());
                var convertedEvents = new LinkedList<IAggregateEvent<TKey, TVersion>>();

                foreach (var @event in updatedEvents)
                {
                    convertedEvents.AddLast(Convert(@event));
                }
                return convertedEvents;
            }

            private static IAggregateEvent<TKey, TVersion> Convert(IEvent @event)
            {
                try
                {
                    return (IAggregateEvent<TKey, TVersion>) @event;
                }
                catch (InvalidCastException)
                {
                    throw NewEventConversionException(@event.GetType(), typeof(IAggregateEvent<TKey, TVersion>), nameof(IEvent.UpdateToLatestVersion));
                }
            }           

            internal bool Apply(IAggregateEvent<TKey, TVersion> aggregateEvent)
            {                
                if (_aggregate.Id.Equals(aggregateEvent.AggregateId))
                {
                    Action<object> eventHandler;

                    if (_eventHandlers.TryGetValue(aggregateEvent.GetType(), out eventHandler))
                    {
                        eventHandler.Invoke(aggregateEvent);
                        return true;
                    }
                    return false;
                }
                throw NewInvalidIdException(aggregateEvent, _aggregate.Id);
            }            

            private static Exception NewMissingEventHandlerException(Type eventType)
            {
                var messageFormat = ExceptionMessages.AggregateRoot_MissingEventHandlerException;
                var message = string.Format(messageFormat, eventType.FriendlyName());
                return new ArgumentException(message);
            }

            private static Exception NewEventConversionException(Type sourceType, Type targetType, string updateToLatestVersionMethodName)
            {
                var messageFormat = ExceptionMessages.AggregateRoot_EventConversionException;
                var message = string.Format(messageFormat, sourceType.FriendlyName(), targetType.FriendlyName(), updateToLatestVersionMethodName);
                return new ArgumentException(message);
            }

            private static Exception NewInvalidIdException(IAggregateEvent<TKey, TVersion> aggregateEvent, TKey id)
            {
                var messageFormat = ExceptionMessages.AggregateRoot_InvalidIdOnEvent;
                var message = string.Format(messageFormat, aggregateEvent.AggregateId, aggregateEvent.GetType().FriendlyName(), id);
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
        /// This aggregate does not recognise one of the events.
        /// </exception>
        protected virtual void LoadFromHistory(IEnumerable<IEvent> events) =>
            EventHandlers.ApplyOld(events);
        
        ISnapshot IAggregateRoot.TakeSnapshot() =>
            TakeSnapshot();

        /// <summary>
        /// Creates and returns a snapshot of the current state of this aggregate.
        /// </summary>
        /// <returns>A snapshot of the current state of this aggregate.</returns>
        protected abstract ISnapshot<TKey, TVersion> TakeSnapshot();

        #endregion

        #region [====== Publish & FlushEvents ======]

        /// <summary>
        /// Indicates whether or not every event that is published by this aggregate is also immediately applied
        /// to itself by invoking the appropriate event handler. Default is <c>false</c>.
        /// </summary>
        protected virtual bool ApplyEventsToSelf =>
            false;        

        /// <summary>
        /// Publishes the specified <paramref name="event"/> and updates the version of this aggregate.
        /// </summary>
        /// <typeparam name="TEvent">Type of the published event.</typeparam>
        /// <param name="event">The event to publish.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="event"/> is <c>null</c>.
        /// </exception>
        protected void Publish<TEvent>(TEvent @event) where TEvent : IAggregateEvent<TKey, TVersion>
        {
            if (@event == null)
            {
                throw new ArgumentNullException(nameof(@event));
            }
            if (_wasRemovedFromRepository)
            {
                throw NewAggregateRemovedException(GetType(), @event.GetType());
            }            
            @event.AggregateId = Id;
            @event.AggregateVersion = Version = NextVersion();

            if (ApplyEventsToSelf)
            {
                EventHandlers.Apply(@event);
            }
            if (OnEventPublished(new EventPublishedEventArgs<TEvent>(@event)))
            {
                return;
            }
            _pendingEvents.Add(@event);
        }

        event EventHandler<EventPublishedEventArgs> IAggregateRoot.EventPublished
        {
            add => EventPublished += value;
            remove => EventPublished -= value;
        }

        /// <summary>
        /// This event is raised each time this aggregate publishes a new event.
        /// </summary>
        protected EventHandler<EventPublishedEventArgs> EventPublished;

        /// <summary>
        /// Applies the published event to this aggregate, if and only if an event handler has been registered for this event,
        /// and raises the <see cref="EventPublished" /> event. When you override this method, make sure you call the base
        /// class implementation.
        /// </summary>
        /// <typeparam name="TEvent">Type of the event that was published.</typeparam>
        /// <param name="e">Argument of the </param>
        /// <returns>
        /// <c>true</c> if the event has been handled by a listener; otherwise <c>false</c>.
        /// </returns>
        protected virtual bool OnEventPublished<TEvent>(EventPublishedEventArgs<TEvent> e) where TEvent : IAggregateEvent<TKey, TVersion>
        {                                   
            EventPublished.Raise(this, e);
            return e.HasBeenPublished;            
        }

        bool IAggregateRoot.HasPendingEvents =>
            HasPublishedEvents;

        /// <summary>
        /// Indicates whether or not this aggregate has published any events that haven't been flushed yet.
        /// </summary>
        protected bool HasPublishedEvents =>
            _pendingEvents.Count > 0;

        IEnumerable<IEvent> IAggregateRoot.FlushEvents() =>
            FlushEvents();

        /// <summary>
        /// Returns all events that were published by this aggregate since it was instantiated or retrieved from a data store
        /// and empties the aggregate's internal event buffer.
        /// </summary>
        protected virtual IEnumerable<IEvent> FlushEvents() =>
            Interlocked.Exchange(ref _pendingEvents, new List<IEvent>());

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
            var message = string.Format(messageFormat, aggregateType.FriendlyName(), eventType.FriendlyName());
            return new IllegalOperationException(message);
        }

        #endregion
    }
}
