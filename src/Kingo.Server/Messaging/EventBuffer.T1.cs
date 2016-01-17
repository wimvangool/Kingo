using System;
using System.Threading.Tasks;

namespace Kingo.Messaging
{    
    internal class EventBuffer<TMessage> : IEventBuffer where TMessage : class, IMessage
    {
        private readonly IDomainEventBus _eventBus;
        private readonly TMessage _domainEvent;
        
        public EventBuffer(IDomainEventBus eventBus, TMessage domainEvent)
        {
            if (eventBus == null)
            {
                throw new ArgumentNullException("eventBus");
            }
            if (domainEvent == null)
            {
                throw new ArgumentNullException("domainEvent");
            }
            _eventBus = eventBus;
            _domainEvent = domainEvent;
        }
        
        public Task FlushAsync()
        {
            return _eventBus.PublishAsync(_domainEvent);
        }
    }
}
