using System;
using System.Collections.Generic;
using System.Globalization;
using YellowFlare.MessageProcessing.Resources;

namespace YellowFlare.MessageProcessing.Aggregates
{
    public abstract class EventSourcedAggregate<TKey> : BufferedEventAggregate<TKey>, IWritableEventStream<TKey>
        where TKey : struct, IEquatable<TKey>
    {
        private readonly Dictionary<Type, Action<IDomainEvent<TKey>>> _eventHandlers;

        protected EventSourcedAggregate()
        {
            _eventHandlers = new Dictionary<Type, Action<IDomainEvent<TKey>>>();
        }

        protected EventSourcedAggregate(int capacity) : base(capacity)
        {
            _eventHandlers = new Dictionary<Type, Action<IDomainEvent<TKey>>>();
        }

        protected void RegisterEventHandler<TDomainEvent>(Action<TDomainEvent> eventHandler)
        {
            if (eventHandler == null)
            {
                throw new ArgumentNullException("eventHandler");
            }
            _eventHandlers.Add(typeof(TDomainEvent), domainEvent => eventHandler.Invoke((TDomainEvent) domainEvent));
        }

        void IWritableEventStream<TKey>.Write<TDomainEvent>(TDomainEvent domainEvent)
        {
            if (domainEvent == null)
            {
                throw new ArgumentNullException("domainEvent");
            }
            Handle(domainEvent);
        }

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

        private static Exception NewMissingEventHandlerException(Type domainEventType)
        {
            var messageFormat = ExceptionMessages.WritableEventStream_MissingEventHandler;
            var message = string.Format(CultureInfo.CurrentCulture, messageFormat, domainEventType);
            return new ArgumentException(message);
        }        
    }
}
