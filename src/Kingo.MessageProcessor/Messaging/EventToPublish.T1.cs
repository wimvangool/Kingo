using System;
using System.Threading.Tasks;

namespace Kingo.Messaging
{    
    internal class EventToPublish<TEvent> : IEventToPublish where TEvent : class, IMessage
    {        
        private readonly TEvent _event;
        
        public EventToPublish(TEvent @event)
        {           
            if (@event == null)
            {
                throw new ArgumentNullException(nameof(@event));
            }            
            _event = @event;
        }
        
        public Task PublishAsync(IMessageProcessorBus eventBus)
        {
            return eventBus.PublishAsync(_event);
        }
    }
}
