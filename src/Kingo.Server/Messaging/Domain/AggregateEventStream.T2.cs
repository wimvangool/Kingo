﻿using System;
using System.Collections.Generic;
using System.Globalization;
using Kingo.Resources;

namespace Kingo.Messaging.Domain
{
    /// <summary>
    /// Represents an aggregate that is modeled as a stream of events and can also be restored as such.
    /// </summary>
    /// <typeparam name="TKey">Type of the aggregate-key.</typeparam>
    /// <typeparam name="TVersion">Type of the aggregate-version.</typeparam>    
    [Serializable]
    public abstract class AggregateEventStream<TKey, TVersion> : AggregateRoot<TKey, TVersion>, IWritableEventStream<TKey, TVersion>        
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>
    {                     
        [NonSerialized]
        private readonly Dictionary<Type, Action<IVersionedObject<TKey, TVersion>>> _eventHandlers;

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateEventStream{TKey, TVersion}" /> class.
        /// </summary>
        protected AggregateEventStream()
        {            
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

        private static Exception NewHandlerForTypeAlreadyRegisteredException(Type domainEventType)
        {
            var messageFormat = ExceptionMessages.AggregateEventStream_HandlerAlreadyRegistered;
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