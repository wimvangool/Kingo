using System.Collections.Generic;
using System.Globalization;
using System.Resources;

namespace System.ComponentModel.Server.Domain
{
    /// <summary>
    /// Represents an aggregate that is modeled as a stream of events and can also be restored as such.
    /// </summary>
    /// <typeparam name="TKey">Type of the aggregate-key.</typeparam>
    /// <typeparam name="TVersion">Type of the aggregate-version.</typeparam>
    public abstract class AggregateEventStream<TKey, TVersion> : AggregateRoot<TKey, TVersion>, IWritableEventStream<TKey, TVersion>
        where TKey : struct, IEquatable<TKey>
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>
    {
        private readonly Dictionary<Type, Action<IAggregateRootEvent<TKey, TVersion>>> _eventHandlers;

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateEventStream{TKey, TVersion}" /> class.
        /// </summary>
        protected AggregateEventStream()
        {
            _eventHandlers = new Dictionary<Type, Action<IAggregateRootEvent<TKey, TVersion>>>();
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
            if (@event.AggregateKey.Equals(Key))
            {
                Handle(@event);
                return;
            }
            throw NewNonMatchingAggregateKeyException(@event);
        }

        /// <summary>
        /// Applies the event that was created using the specified <paramref name="eventFactory"/> to this aggregate.
        /// </summary>
        /// <typeparam name="TEvent">Type of the event to apply.</typeparam>
        /// <param name="eventFactory">A factory used to create the event..</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="eventFactory"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The created event's <see cref="IAggregateRootEvent{S, T}.AggregateKey" /> does not match
        /// this aggregate's <see cref="AggregateRoot{S, T}.Key" />, or no handler for an event of the specified
        /// type has been registered.
        /// </exception>
        protected void Apply<TEvent>(Func<TVersion, TEvent> eventFactory) where TEvent : class, IAggregateRootEvent<TKey, TVersion>, IMessage<TEvent>
        {
            if (eventFactory == null)
            {
                throw new ArgumentNullException("eventFactory");
            }
            Apply(eventFactory.Invoke(Increment(Version)));
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
        /// The <paramref name="event"/>'s <see cref="IAggregateRootEvent{S, T}.AggregateKey" /> does not match
        /// this aggregate's <see cref="AggregateRoot{S, T}.Key" />, or no handler for an event of the specified
        /// type has been registered.
        /// </exception>
        protected void Apply<TEvent>(TEvent @event) where TEvent : class, IAggregateRootEvent<TKey, TVersion>, IMessage<TEvent>
        {
            if (@event == null)
            {
                throw new ArgumentNullException("event");
            }
            if (@event.AggregateKey.Equals(Key))
            {
                Handle(@event);
                Publish(@event);
                return;
            }
            throw NewNonMatchingAggregateKeyException(@event);            
        }

        private void Handle<TEvent>(TEvent @event) where TEvent : class, IAggregateRootEvent<TKey, TVersion>
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
            Version = @event.AggregateVersion;
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
