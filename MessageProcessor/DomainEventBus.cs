using System;
using System.Collections.Generic;
using System.Threading;

namespace YellowFlare.MessageProcessing
{       
    internal sealed class DomainEventBus : IDomainEventBus
    {                
        private readonly ThreadLocal<List<DomainEventBusSubscription>> _subscriptions;
        private readonly MessageProcessor _processor;  
        
        public DomainEventBus(MessageProcessor processor)
        {            
            _subscriptions = new ThreadLocal<List<DomainEventBusSubscription>>(() => new List<DomainEventBusSubscription>());  
            _processor = processor;                
        }
        
        public IDisposable Subscribe<TMessage>(Action<TMessage> action) where TMessage : class
        {
            return new DomainEventBusSubscriptionForAction<TMessage>(_subscriptions.Value, action);
        }
        
        public IDisposable Subscribe<TMessage>(IMessageHandler<TMessage> handler) where TMessage : class
        {            
            return new DomainEventBusSubscriptionForInterface<TMessage>(_subscriptions.Value, handler);
        }        
                       
        public void Publish<TMessage>(TMessage message) where TMessage : class
        {
            HandleMessageWithRegisteredHandlers(message);
            HandleMessageWithSubscribedHandlers(message);                            
        }

        private void HandleMessageWithRegisteredHandlers<TMessage>(TMessage message) where TMessage : class
        {
            _processor.Handle(message, MessageSources.DomainEventBus);
        }              

        private void HandleMessageWithSubscribedHandlers<TMessage>(TMessage message) where TMessage : class
        {
            if (_subscriptions.IsValueCreated)
            {
                foreach (var subscription in _subscriptions.Value)
                {
                    subscription.Handle(_processor, message);
                }
            } 
        }       
    }
}
