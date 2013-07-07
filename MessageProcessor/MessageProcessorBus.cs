using System;
using System.Collections.Generic;
using System.Threading;

namespace YellowFlare.MessageProcessing
{       
    internal sealed class MessageProcessorBus : IMessageProcessorBus
    {                
        private readonly ThreadLocal<List<MessageProcessorBusSubscription>> _subscriptions;
        private readonly IMessageProcessor _processor;  
        
        public MessageProcessorBus(IMessageProcessor processor)
        {
            if (processor == null)
            {
                throw new ArgumentNullException("processor");
            }
            _subscriptions = new ThreadLocal<List<MessageProcessorBusSubscription>>(() => new List<MessageProcessorBusSubscription>());  
            _processor = processor;                
        }
        
        public IDisposable Subscribe<TMessage>(Action<TMessage> action) where TMessage : class
        {
            return new MessageProcessorBusSubscriptionForAction<TMessage>(_subscriptions.Value, action);
        }
        
        public IDisposable Subscribe<TMessage>(IMessageHandler<TMessage> handler) where TMessage : class
        {            
            return new MessageProcessorBusSubscriptionForInterface<TMessage>(_subscriptions.Value, handler);
        }        
                       
        public void Publish<TMessage>(TMessage message) where TMessage : class
        {
            InvokeRegisteredHandlers(message);
            InvokeSubscribedHandlers(message);                            
        }

        private void InvokeRegisteredHandlers<TMessage>(TMessage message) where TMessage : class
        {
            _processor.Handle(message);
        }              

        private void InvokeSubscribedHandlers<TMessage>(TMessage message) where TMessage : class
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
