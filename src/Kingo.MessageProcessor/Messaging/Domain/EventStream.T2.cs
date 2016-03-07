using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Kingo.Resources;

namespace Kingo.Messaging.Domain
{
    /// <summary>
    /// Represents an aggregate that is modeled as a stream of events and can also be restored as such.
    /// </summary>
    /// <typeparam name="TKey">Type of the aggregate-key.</typeparam>
    /// <typeparam name="TVersion">Type of the aggregate-version.</typeparam>        
    [Serializable]
    public abstract class EventStream<TKey, TVersion> : AggregateRoot<TKey, TVersion>, IEventStream<TKey, TVersion>        
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>        
    {                     
        [NonSerialized]
        private Dictionary<Type, Action<IHasKeyAndVersion<TKey, TVersion>>> _eventHandlers;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="EventStream{T, S}" /> class.
        /// </summary>
        /// <param name="event">The event of that represents the creation of this aggregate.</param>        
        protected EventStream(IDomainEvent<TKey, TVersion> @event = null)
            : base(@event) { }

        private Dictionary<Type, Action<IHasKeyAndVersion<TKey, TVersion>>> EventHandlers
        {
            get
            {
                if (_eventHandlers == null)
                {
                    _eventHandlers = new Dictionary<Type, Action<IHasKeyAndVersion<TKey, TVersion>>>();
                }
                return _eventHandlers;
            }
        }

        /// <summary>
        /// When overridden, registers all event handlers of this aggregate through the <see cref="RegisterEventHandler{T}" /> method.
        /// </summary>
        protected abstract void RegisterEventHandlers();

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
                throw new ArgumentNullException(nameof(eventHandler));
            }
            try
            {
                EventHandlers.Add(typeof(TEvent), @event => eventHandler.Invoke((TEvent) @event));
            }
            catch (ArgumentException)
            {
                throw NewHandlerForTypeAlreadyRegisteredException(typeof(TEvent));
            }            
        }        

        void IEventStream<TKey, TVersion>.LoadFromHistory(IEnumerable<IDomainEvent<TKey, TVersion>> historicEvents)
        {
            if (historicEvents == null)
            {
                throw new ArgumentNullException(nameof(historicEvents));
            }
            foreach (var historicEvent in historicEvents.OrderBy(e => e.Version))
            {
                Apply(historicEvent.UpgradeToLatestVersion());
            }            
        }

        internal override void Apply(IHasKeyAndVersion<TKey, TVersion> @event)
        {
            // The EventHandlers are registered just-in-time. Since we expect at least one handler
            // to be registered (to handle the created event), we can check this by looking at the
            // the number of handlers that were registered.
            if (EventHandlers.Count == 0)
            {
                RegisterEventHandlers();
            }
            base.Apply(@event);

            var eventType = @event.GetType();

            try
            {
                EventHandlers[eventType].Invoke(@event);                
            }
            catch (KeyNotFoundException)
            {
                throw NewMissingEventHandlerException(eventType);
            }            
        }        

        private static Exception NewHandlerForTypeAlreadyRegisteredException(Type domainEventType)
        {
            var messageFormat = ExceptionMessages.EventStream_HandlerAlreadyRegistered;
            var message = string.Format(CultureInfo.CurrentCulture, messageFormat, domainEventType.Name);
            return new ArgumentException(message);
        }

        private static Exception NewMissingEventHandlerException(Type domainEventType)
        {
            var messageFormat = ExceptionMessages.EventStream_MissingEventHandler;
            var message = string.Format(CultureInfo.CurrentCulture, messageFormat, domainEventType);
            return new ArgumentException(message);
        }        
    }
}
