using System.Collections.Generic;
using System.Threading;

namespace System.ComponentModel.Messaging.Server
{       
    internal sealed class MessageProcessorBus : IMessageProcessorBus
    {
        private readonly ICollection<MessageProcessorBusConnection> _connections;
        private readonly ThreadLocal<ICollection<MessageProcessorBusConnection>> _threadLocalConnections;
        private readonly IMessageProcessor _processor;  
        
        public MessageProcessorBus(IMessageProcessor processor)
        {
            if (processor == null)
            {
                throw new ArgumentNullException("processor");
            }
            _connections = new SynchronizedCollection<MessageProcessorBusConnection>();
            _threadLocalConnections = new ThreadLocal<ICollection<MessageProcessorBusConnection>>(() => new List<MessageProcessorBusConnection>());  
            _processor = processor;                
        }

        public IConnection Connect<TMessage>(Action<TMessage> action, bool openConnection) where TMessage : class
        {
            var connection = new MessageProcessorBusConnectionForAction<TMessage>(_connections, action);

            if (openConnection)
            {
                connection.Open();
            }
            return connection;
        }

        public IConnection Connect<TMessage>(IMessageHandler<TMessage> handler, bool openConnection) where TMessage : class
        {
            var connection = new MessageProcessorBusConnectionForInterface<TMessage>(_connections, handler);

            if (openConnection)
            {
                connection.Open();
            }
            return connection;
        }
        
        public IConnection ConnectThreadLocal<TMessage>(Action<TMessage> action, bool openConnection) where TMessage : class
        {
            var connection = new MessageProcessorBusConnectionForAction<TMessage>(_threadLocalConnections.Value, action);

            if (openConnection)
            {
                connection.Open();
            }
            return connection;
        }
        
        public IConnection ConnectThreadLocal<TMessage>(IMessageHandler<TMessage> handler, bool openConnection) where TMessage : class
        {            
            var connection = new MessageProcessorBusConnectionForInterface<TMessage>(_threadLocalConnections.Value, handler);

            if (openConnection)
            {
                connection.Open();
            }
            return connection;
        }        
                       
        public void Publish<TMessage>(TMessage message) where TMessage : class
        {
            InvokeRegisteredHandlers(message);
            InvokeConnectedHandlers(message);                            
        }

        private void InvokeRegisteredHandlers<TMessage>(TMessage message) where TMessage : class
        {
            _processor.Process(message);
        }              

        private void InvokeConnectedHandlers<TMessage>(TMessage message) where TMessage : class
        {
            foreach (var connection in _connections)
            {
                connection.Handle(_processor, message);
            }
            if (_threadLocalConnections.IsValueCreated)
            {
                foreach (var connection in _threadLocalConnections.Value)
                {
                    connection.Handle(_processor, message);
                }
            } 
        }        
    }
}
