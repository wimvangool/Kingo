using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Kingo.MicroServices.Domain
{
    /// <summary>
    /// Serves as a base-class implementation of the <see cref="IAggregateRoot{T}" /> interface.
    /// </summary>
    /// <typeparam name="TKey">Type of the identifier of this aggregate.</typeparam>
    /// <typeparam name="TVersion">Type of the version of this aggregate.</typeparam>
    public abstract class AggregateRoot<TKey, TVersion> : AggregateRoot, IAggregateRoot<TKey, TVersion>
        where TKey : struct, IEquatable<TKey>
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>
    {
        #region [====== EventBuffer ======]

        private sealed class EventBuffer : ReadOnlyList<ISnapshotOrEvent>
        {
            private readonly IEventBus _eventBus;
            private List<ISnapshotOrEvent<TKey, TVersion>> _events;

            public EventBuffer(IEventBus eventBus)
            {
                _eventBus = eventBus;
                _events = new List<ISnapshotOrEvent<TKey, TVersion>>();
            }

            public IEventBus EventBus =>
                _eventBus;

            public override int Count =>
                _events.Count;

            public override IEnumerator<ISnapshotOrEvent> GetEnumerator() =>
                _events.GetEnumerator();

            public void Add(ISnapshotOrEvent<TKey, TVersion> @event)
            {
                _events.Add(@event);
                _eventBus?.Publish(@event);
            }

            public IReadOnlyList<ISnapshotOrEvent<TKey, TVersion>> Commit() =>
                Interlocked.Exchange(ref _events, new List<ISnapshotOrEvent<TKey, TVersion>>());
        }

        #endregion  
        
        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateRoot" /> class.
        /// </summary>
        /// <param name="parent">The parent of this aggregate (the one that is creating this instance).</param>
        /// <param name="event">Event that is used to initialize this aggregate.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="parent"/> or <paramref name="event"/> is <c>null</c>.
        /// </exception>
        protected AggregateRoot(AggregateRoot parent, ISnapshotOrEvent<TKey, TVersion> @event) :
            this(GetEventBusFrom(parent), @event, true) { }

        private static IEventBus GetEventBusFrom(AggregateRoot parent)
        {
            if (parent == null)
            {
                throw new ArgumentNullException(nameof(parent));
            }
            return parent.EventBus;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateRoot" /> class.
        /// </summary>
        /// <param name="eventBus">Optional event-bus to which all events will be published.</param>
        /// <param name="snapshotOrEvent">Snapshot or event that is used to initialize this aggregate.</param>
        /// <param name="isNewAggregate">
        /// Indicates whether or not this aggregate is a new conceptual entity (and is, as such, not being
        /// restored). If <c>true</c>, then <paramref name="snapshotOrEvent"/> must be an event that will be
        /// immediately added to the collection of published events of this aggregate.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="snapshotOrEvent"/> is <c>null</c>.
        /// </exception>
        protected AggregateRoot(IEventBus eventBus, ISnapshotOrEvent<TKey, TVersion> snapshotOrEvent, bool isNewAggregate)            
        {
            if (snapshotOrEvent == null)
            {
                throw new ArgumentNullException(nameof(snapshotOrEvent));
            }
            Id = snapshotOrEvent.Id;
            Version = snapshotOrEvent.Version;
            Events = CreateEventBuffer(eventBus, snapshotOrEvent, isNewAggregate);
        }

        private static EventBuffer CreateEventBuffer(IEventBus eventBus, ISnapshotOrEvent<TKey, TVersion> snapshotOrEvent, bool isNewAggregate)
        {            
            var eventBuffer = new EventBuffer(eventBus);

            if (isNewAggregate)
            {
                eventBuffer.Add(snapshotOrEvent);
            }
            return eventBuffer;
        }

        internal override IEventBus EventBus =>
            Events.EventBus;

        /// <inheritdoc />
        protected override bool HasBeenModified =>
            Events.Count > 0;

        private EventBuffer Events
        {
            get;
        }

        /// <inheritdoc />
        public override string ToString() =>
            $"{GetType().FriendlyName()} [Events = {Events.Count}]";

        #region [====== Id & Version ======]

        private TVersion _version;

        TKey IAggregateRoot<TKey>.Id =>
            Id;

        /// <summary>
        /// The identifier of this aggregate.
        /// </summary>
        protected TKey Id
        {
            get;
        }

        TVersion IAggregateRoot<TKey, TVersion>.Version =>
            Version;

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

        #region [====== Events & EventHandlers ======]

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
                where TEvent : ISnapshotOrEvent<TKey, TVersion>
            {
                if (eventHandler == null)
                {
                    throw new ArgumentNullException(nameof(eventHandler));
                }
                try
                {
                    _eventHandlers.Add(typeof(TEvent), @event => eventHandler.Invoke((TEvent) @event));
                }
                catch (ArgumentException)
                {
                    throw NewHandlerForEventTypeAlreadyRegisteredException(typeof(TEvent));
                }
                return this;
            }            

            internal void ApplyOld(IEnumerable<ISnapshotOrEvent<TKey, TVersion>> events)
            {
                if (events == null)
                {
                    throw new ArgumentNullException(nameof(events));
                }
                foreach (var @event in SelectEventsToApply(events))
                {
                    if (Apply(@event))
                    {
                        _aggregate.Version = @event.Version;
                        continue;
                    }
                    throw NewMissingEventHandlerException(@event.GetType());
                }
            }

            private IEnumerable<ISnapshotOrEvent<TKey, TVersion>> SelectEventsToApply(IEnumerable<ISnapshotOrEvent<TKey, TVersion>> events) =>
                from @event in events.WhereNotNull()
                where CanApply(@event)
                select @event;            

            private bool CanApply(ISnapshotOrEvent<TKey, TVersion> @event) =>
                @event.Id.Equals(_aggregate.Id) && @event.Version.CompareTo(_aggregate.Version) > 0;

            internal bool Apply(ISnapshotOrEvent<TKey, TVersion> @event)
            {
                if (_eventHandlers.TryGetValue(@event.GetType(), out var eventHandler))
                {
                    eventHandler.Invoke(@event);
                    return true;
                }
                return false;
            }

            private static Exception NewHandlerForEventTypeAlreadyRegisteredException(Type type)
            {
                var messageFormat = ExceptionMessages.AggregateRoot_HandlerForEventTypeAlreadyAdded;
                var message = string.Format(messageFormat, type.FriendlyName());
                return new ArgumentException(message);
            }            
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

        void IAggregateRoot<TKey, TVersion>.LoadFromHistory(IEnumerable<ISnapshotOrEvent<TKey, TVersion>> events) =>
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
        protected virtual void LoadFromHistory(IEnumerable<ISnapshotOrEvent<TKey, TVersion>> events) =>
            EventHandlers.ApplyOld(events);                  

        /// <summary>
        /// Adds all event handler delegates to the specified <paramref name="eventHandlers"/> and returns the resulting collection.
        /// </summary>
        /// <param name="eventHandlers">A collection of event handler delegates.</param>
        /// <returns>A completed collection of event handlers that is used by this aggregate to apply and/or replay certain events.</returns>
        protected virtual EventHandlerCollection RegisterEventHandlers(EventHandlerCollection eventHandlers) =>
            eventHandlers;

        /// <summary>
        /// Publishes the event that is created by the specified <paramref name="eventFactory"/> and updates the version of this aggregate.
        /// </summary>        
        /// <param name="eventFactory">The event to publish.</param>        
        /// <exception cref="BusinessRuleException">
        /// This aggregate has already been removed from the repository and therefore does not allow any further changes.
        /// </exception>        
        protected void Publish(Func<TKey, TVersion, ISnapshotOrEvent<TKey, TVersion>> eventFactory)  =>
            PublishAndApply(eventFactory.Invoke(Id, NextVersion()));

        private void PublishAndApply(ISnapshotOrEvent<TKey, TVersion> @event)            
        {
            if (EventHandlers.Apply(@event) || EventHandlers.IsEmpty)
            {
                Version = Publish(@event);
                return;
            }
            throw NewMissingEventHandlerException(@event.GetType());
        }

        internal TVersion Publish(ISnapshotOrEvent<TKey, TVersion> @event)           
        {
            if (HasBeenRemoved)
            {
                throw NewAggregateRemovedException(GetType(), @event.GetType());
            }
            Events.Add(@event);
            OnModified();
            return @event.Version;
        }

        private static Exception NewMissingEventHandlerException(Type eventType)
        {
            var messageFormat = ExceptionMessages.AggregateRoot_MissingEventHandlerException;
            var message = string.Format(messageFormat, eventType.FriendlyName());
            return new ArgumentException(message);
        }

        private static Exception NewAggregateRemovedException(Type aggregateType, Type eventType)
        {
            var messageFormat = ExceptionMessages.AggregateRoot_AggregateRemovedException;
            var message = string.Format(messageFormat, aggregateType.FriendlyName(), eventType?.FriendlyName());
            return new BusinessRuleException(message);
        }

        #endregion

        #region [====== TakeSnapshot & Commit =====]        

        ISnapshotOrEvent<TKey, TVersion> IAggregateRoot<TKey, TVersion>.TakeSnapshot() =>
            TakeSnapshot();

        /// <summary>
        /// When overridden, creates and returns a snapshot of the current state of this aggregate.
        /// </summary>
        /// <returns>A snapshot of the current state of this aggregate.</returns>
        /// <exception cref="NotSupportedException">
        /// This aggregate does not support snapshots.
        /// </exception>
        protected virtual ISnapshotOrEvent<TKey, TVersion> TakeSnapshot() =>
            throw NewSnapshotsNotSupportedException();

        private Exception NewSnapshotsNotSupportedException()
        {
            var messageFormat = ExceptionMessages.AggregateRoot_SnapshotsNotSupported;
            var message = string.Format(messageFormat, GetType().FriendlyName());
            return new NotSupportedException(message);
        }

        IReadOnlyList<ISnapshotOrEvent<TKey, TVersion>> IAggregateRoot<TKey, TVersion>.Commit() =>
            Commit();

        /// <summary>
        /// Commits this aggregate by flushing  and returning all events that were published
        /// since the last commit.
        /// </summary>
        /// <returns>All events that were published since the last commit.</returns>
        protected virtual IReadOnlyList<ISnapshotOrEvent<TKey, TVersion>> Commit() =>
            Events.Commit();

        #endregion
    }
}
