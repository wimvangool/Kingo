using System.Collections.Generic;
using System.ComponentModel.Resources;
using System.Globalization;

namespace System.ComponentModel.Server.Domain
{
    /// <summary>
    /// Represents an aggregate that is modeled as a stream of events and can also be restored as such.
    /// </summary>
    /// <typeparam name="TKey">Type of the aggregate-key.</typeparam>
    /// <typeparam name="TVersion">Type of the aggregate-version.</typeparam>
    public abstract class EventSourcedAggregate<TKey, TVersion> : Aggregate<TKey, TVersion>, IWritableEventStream<TKey, TVersion>
        where TKey : struct, IEquatable<TKey>
        where TVersion : struct, IAggregateVersion<TVersion>
    {
        private readonly Dictionary<Type, Action<IAggregateEvent<TKey, TVersion>>> _eventHandlers;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventSourcedAggregate{TKey, TVersion}" /> class.
        /// </summary>
        protected EventSourcedAggregate()
        {
            _eventHandlers = new Dictionary<Type, Action<IAggregateEvent<TKey, TVersion>>>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventSourcedAggregate{TKey, TVersion}" /> class.
        /// </summary>
        /// <param name="capacity">The initial capacity of the buffer.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="capacity"/> is negative.
        /// </exception>
        protected EventSourcedAggregate(int capacity) : base(capacity)
        {
            _eventHandlers = new Dictionary<Type, Action<IAggregateEvent<TKey, TVersion>>>();
        }

        /// <summary>
        /// Registers a handler that is invoked when the aggregate writes an event of the specified type.
        /// </summary>
        /// <typeparam name="TEvent">Type of the event to handle.</typeparam>
        /// <param name="eventHandler">The handler of the event.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="eventHandler"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// A handler for the specified event-type <paramtyperef name="TDomainEvent"/> has already been registered.
        /// </exception>
        protected void RegisterEventHandler<TEvent>(Action<TEvent> eventHandler)
        {
            if (eventHandler == null)
            {
                throw new ArgumentNullException("eventHandler");
            }
            try
            {
                _eventHandlers.Add(typeof(TEvent), @event => eventHandler.Invoke((TEvent) @event));
            }
            catch (ArgumentException)
            {
                throw NewHandlerForTypeAlreadyRegisteredException(typeof(TEvent));
            }            
        }

        void IWritableEventStream<TKey, TVersion>.Write<TEvent>(TEvent @event)
        {
            if (@event == null)
            {
                throw new ArgumentNullException("event");
            }
            Handle(@event);
        }

        /// <summary>
        /// Applies the specified event to this aggregate.
        /// </summary>
        /// <typeparam name="TEvent">Type of the event to apply.</typeparam>
        /// <param name="event">The event to apply.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="event"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// No handler for an event of the specified type has been registered.
        /// </exception>
        protected void Apply<TEvent>(TEvent @event) where TEvent : class, IAggregateEvent<TKey, TVersion>, IMessage<TEvent>
        {
            if (@event == null)
            {
                throw new ArgumentNullException("event");
            }
            Handle(@event);
            Write(@event);
        }

        private void Handle<TEvent>(TEvent @event) where TEvent : class, IAggregateEvent<TKey, TVersion>
        {
            Type eventType = typeof(TEvent);

            try
            {
                _eventHandlers[eventType].Invoke(@event);
            }
            catch (KeyNotFoundException)
            {
                throw NewMissingEventHandlerException(eventType);
            }
        }

        private static Exception NewHandlerForTypeAlreadyRegisteredException(Type domainEventType)
        {
            var messageFormat = ExceptionMessages.EventSourcedAggregate_HandlerAlreadyRegistered;
            var message = string.Format(CultureInfo.CurrentCulture, messageFormat, domainEventType.Name);
            return new ArgumentException(message);
        }

        private static Exception NewMissingEventHandlerException(Type domainEventType)
        {
            var messageFormat = ExceptionMessages.WritableEventStream_MissingEventHandler;
            var message = string.Format(CultureInfo.CurrentCulture, messageFormat, domainEventType);
            return new ArgumentException(message);
        }        
    }
}
