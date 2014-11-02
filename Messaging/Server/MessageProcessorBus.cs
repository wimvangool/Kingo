using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace System.ComponentModel.Server
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

        #region [====== Connect ======]

        public IConnection Connect(object handler, bool openConnection)
        {
            return Connect(handler, openConnection, _connections);
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

        #endregion

        #region [====== ConnectThreadLocal ======]

        public IConnection ConnectThreadLocal(object handler, bool openConnection)
        {
            return Connect(handler, openConnection, _threadLocalConnections.Value);
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

        #endregion        

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

        private static IConnection Connect(object handler, bool openConnection, ICollection<MessageProcessorBusConnection> connections)
        {
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }
            var connection = new CompositeConnection(CreateConnections(handler, connections));

            if (openConnection)
            {
                connection.Open();
            }
            return connection;
        }

        private static IEnumerable<IConnection> CreateConnections(object handler, ICollection<MessageProcessorBusConnection> connections)
        {
            return from interfaceType in handler.GetType().GetInterfaces()
                   where IsMessageHandler(interfaceType)
                   select CreateConnection(handler, connections, interfaceType);
        }

        private static bool IsMessageHandler(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IMessageHandler<>);
        }

        private static IConnection CreateConnection(object handler, ICollection<MessageProcessorBusConnection> connections, Type interfaceType)
        {
            var messageType = interfaceType.GetGenericArguments()[0];
            var connectionTypeDefinition = typeof(MessageProcessorBusConnectionForInterface<>);
            var connectionType = connectionTypeDefinition.MakeGenericType(messageType);            
            var constructor = connectionType.GetConstructor(new [] { typeof(ICollection<MessageProcessorBusConnection>), interfaceType });

            return (IConnection) constructor.Invoke(new [] { connections, handler });
        }
    }
}
