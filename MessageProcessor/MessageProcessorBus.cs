using System;
using System.Collections.Generic;
using System.Threading;

namespace YellowFlare.MessageProcessing
{       
    internal sealed class MessageProcessorBus : IMessageProcessorBus
    {                
        private readonly ThreadLocal<List<MessageProcessorBusConnection>> _subscriptions;
        private readonly IMessageProcessor _processor;  
        
        public MessageProcessorBus(IMessageProcessor processor)
        {
            if (processor == null)
            {
                throw new ArgumentNullException("processor");
            }
            _subscriptions = new ThreadLocal<List<MessageProcessorBusConnection>>(() => new List<MessageProcessorBusConnection>());  
            _processor = processor;                
        }
        
        public IConnection Connect<TMessage>(Action<TMessage> action, bool openConnection) where TMessage : class
        {
            var connection = new MessageProcessorBusConnectionForAction<TMessage>(_subscriptions.Value, action);

            if (openConnection)
            {
                connection.Open();
            }
            return connection;
        }
        
        public IConnection Connect<TMessage>(IMessageHandler<TMessage> handler, bool openConnection) where TMessage : class
        {            
            var connection = new MessageProcessorBusConnectionForInterface<TMessage>(_subscriptions.Value, handler);

            if (openConnection)
            {
                connection.Open();
            }
            return connection;
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
