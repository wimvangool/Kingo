using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Kingo.BuildingBlocks.Resources;

namespace Kingo.BuildingBlocks.ComponentModel.Server.Domain
{
    /// <summary>
    /// Represents an aggregate that is modeled as a stream of events and can also be restored as such.
    /// </summary>
    /// <typeparam name="TKey">Type of the aggregate-key.</typeparam>
    /// <typeparam name="TVersion">Type of the aggregate-version.</typeparam>
    [Serializable]
    public abstract class AggregateEventStream<TKey, TVersion> : AggregateRoot<TKey, TVersion>, IReadableEventStream<TKey, TVersion>, IWritableEventStream<TKey, TVersion>
        where TKey : struct, IEquatable<TKey>
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>
    {
        private const string _BufferKey = "_buffer";
        private readonly MemoryEventStream<TKey, TVersion> _buffer;        
        private readonly Dictionary<Type, Action<IVersionedObject<TKey, TVersion>>> _eventHandlers;

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateEventStream{TKey, TVersion}" /> class.
        /// </summary>
        protected AggregateEventStream()
        {
            _buffer = new MemoryEventStream<TKey, TVersion>();
            _eventHandlers = new Dictionary<Type, Action<IVersionedObject<TKey, TVersion>>>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateEventStream{TKey, TVersion}" /> class.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        protected AggregateEventStream(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _buffer = (MemoryEventStream<TKey, TVersion>) info.GetValue(_BufferKey, typeof(MemoryEventStream<TKey, TVersion>));
            _eventHandlers = new Dictionary<Type, Action<IVersionedObject<TKey, TVersion>>>();
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

        /// <inheritdoc />
        internal override void Publish<TEvent>(TEvent @event)
        {
            base.Publish(@event);

            _buffer.Write(@event);
        }

        void IWritableEventStream<TKey, TVersion>.Write<TEvent>(TEvent @event)
        {
            Apply(@event);
        }

        internal override void Apply<TEvent>(TEvent @event)
        {
            base.Apply(@event);

            var eventType = typeof(TEvent);

            try
            {
                _eventHandlers[eventType].Invoke(@event);                
            }
            catch (KeyNotFoundException)
            {
                throw NewMissingEventHandlerException(eventType);
            }            
        }

        void IReadableEventStream<TKey, TVersion>.FlushTo(IWritableEventStream<TKey, TVersion> stream)
        {
            _buffer.FlushTo(stream);
        }

        private static Exception NewHandlerForTypeAlreadyRegisteredException(Type domainEventType)
        {
            var messageFormat = ExceptionMessages.EventSourcedAggregate_HandlerAlreadyRegistered;
            var message = string.Format(CultureInfo.CurrentCulture, messageFormat, domainEventType.Name);
            return new ArgumentException(message);
        }

        private static Exception NewMissingEventHandlerException(Type domainEventType)
        {
            var messageFormat = ExceptionMessages.AggregateEventStream_MissingEventHandler;
            var message = string.Format(CultureInfo.CurrentCulture, messageFormat, domainEventType);
            return new ArgumentException(message);
        }        
    }
}
