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
        /// <param name="data">The event that defines the creation of this aggregate.</param>       
        /// <exception cref="ArgumentNullException">
        /// <paramref name="data" /> is <c>null</c>.
        /// </exception>
        protected AggregateRoot(IAggregateDataObject<TKey, TVersion> data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }
            _id = data.Id;
            _version = data.Version;
            _pendingEvents = new List<IEvent>();            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateRoot{T, S}" /> class.
        /// </summary>
        /// <param name="event">The event that defines the creation of this aggregate.</param>  
        /// <param name="isNewAggregate">
        /// Indicates whether or not this aggregate is a new aggregate and the event must be published.
        /// </param>     
        /// <exception cref="ArgumentNullException">
        /// <paramref name="event" /> is <c>null</c>.
        /// </exception>
        protected AggregateRoot(IEvent<TKey, TVersion> @event, bool isNewAggregate) :
            this(@event)
        {
            if (isNewAggregate)
            {
                _pendingEvents.Add(@event);
            }
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
                from @event in events.Select(Convert)
                where MustApply(@event)
                select @event;

            private static IEvent<TKey, TVersion> Convert(IEvent @event)
            {
                try
                {
                    return (IEvent<TKey, TVersion>)@event;
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
                if (_eventHandlers.TryGetValue(@event.GetType(), out Action<object> eventHandler))
                {
                    eventHandler.Invoke(@event);
                    return true;
                }
                return false;
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
        protected void Publish<TEvent>(TEvent @event) where TEvent : IEvent<TKey, TVersion>
        {
            if (@event == null)
            {
                throw new ArgumentNullException(nameof(@event));
            }
            if (_wasRemovedFromRepository)
            {
                throw NewAggregateRemovedException(GetType(), @event.GetType());
            }            
            @event.Id = Id;
            @event.Version = Version = NextVersion();

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
        protected virtual bool OnEventPublished<TEvent>(EventPublishedEventArgs<TEvent> e) where TEvent : IEvent<TKey, TVersion>
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
