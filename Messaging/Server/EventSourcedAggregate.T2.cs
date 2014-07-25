using System.Collections.Generic;
using System.ComponentModel.Messaging.Resources;
using System.Globalization;

namespace System.ComponentModel.Messaging.Server
{
    /// <summary>
    /// Represents an aggregate that is modeled as a stream of events and can also be restored as such.
    /// </summary>
    /// <typeparam name="TKey">Type of the aggregate-key.</typeparam>
    /// <typeparam name="TVersion">Type of the aggregate-version.</typeparam>
    public abstract class EventSourcedAggregate<TKey, TVersion> : BufferedEventAggregate<TKey, TVersion>, IWritableEventStream<TKey>
        where TKey : struct, IEquatable<TKey>
        where TVersion : struct, IAggregateVersion<TVersion>
    {
        private readonly Dictionary<Type, Action<IDomainEvent<TKey>>> _eventHandlers;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventSourcedAggregate{TKey, TVersion}" /> class.
        /// </summary>
        protected EventSourcedAggregate()
        {
            _eventHandlers = new Dictionary<Type, Action<IDomainEvent<TKey>>>();
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
            _eventHandlers = new Dictionary<Type, Action<IDomainEvent<TKey>>>();
        }

        /// <summary>
        /// Registers a handler that is invoked when the aggregate writes an event of the specified type.
        /// </summary>
        /// <typeparam name="TDomainEvent">Type of the event to handle.</typeparam>
        /// <param name="eventHandler">The handler of the event.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="eventHandler"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// A handler for the specified event-type <paramtyperef name="TDomainEvent"/> has already been registered.
        /// </exception>
        protected void RegisterEventHandler<TDomainEvent>(Action<TDomainEvent> eventHandler)
        {
            if (eventHandler == null)
            {
                throw new ArgumentNullException("eventHandler");
            }
            try
            {
                _eventHandlers.Add(typeof(TDomainEvent), domainEvent => eventHandler.Invoke((TDomainEvent)domainEvent));
            }
            catch (ArgumentException)
            {
                throw NewHandlerForTypeAlreadyRegisteredException(typeof(TDomainEvent));
            }            
        }

        void IWritableEventStream<TKey>.Write<TDomainEvent>(TDomainEvent domainEvent)
        {
            if (domainEvent == null)
            {
                throw new ArgumentNullException("domainEvent");
            }
            Handle(domainEvent);
        }

        /// <summary>
        /// Applies the specified event to this aggregate.
        /// </summary>
        /// <typeparam name="TDomainEvent">Type of the event to apply.</typeparam>
        /// <param name="domainEvent">The event to apply.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="domainEvent"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// No handler for an event of the specified type has been registered.
        /// </exception>
        protected void Apply<TDomainEvent>(TDomainEvent domainEvent)
            where TDomainEvent : class, IDomainEvent<TKey>
        {
            if (domainEvent == null)
            {
                throw new ArgumentNullException("domainEvent");
            }
            Handle(domainEvent);
            Write(domainEvent);
        }

        private void Handle<TDomainEvent>(TDomainEvent domainEvent) where TDomainEvent : class, IDomainEvent<TKey>
        {
            Type domainEventType = typeof(TDomainEvent);

            try
            {
                _eventHandlers[domainEventType].Invoke(domainEvent);
            }
            catch (KeyNotFoundException)
            {
                throw NewMissingEventHandlerException(domainEventType);
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
