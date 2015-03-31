using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace System.ComponentModel.Server
{       
    [DebuggerTypeProxy(typeof(DebuggerProxy))]
    internal sealed class MessageProcessorBus : IMessageProcessorBus
    {
        #region [====== DebuggerProxy ======]

        private sealed class DebuggerProxy
        {
            private static readonly IMessageProcessorBusConnection[] _NoConnections = new IMessageProcessorBusConnection[0];
            private readonly MessageProcessorBus _bus;

            internal DebuggerProxy(MessageProcessorBus bus)
            {
                _bus = bus;
            }

            [DebuggerDisplay("Count = {StaticConnectionCount}")]            
            public IEnumerable<IMessageProcessorBusConnection> StaticConnections
            {
                get { return _bus._connections; }
            }            

            [DebuggerDisplay("Count = {ThreadStaticConnectionCount}")]            
            public IEnumerable<IMessageProcessorBusConnection> ThreadStaticConnections
            {
                get
                {
                    if (_bus._threadLocalConnections.IsValueCreated)
                    {
                        return _bus._threadLocalConnections.Value;
                    }
                    return _NoConnections;
                }
            }

            private int StaticConnectionCount
            {
                get { return StaticConnections.Count(); }
            }

            private int ThreadStaticConnectionCount
            {
                get { return ThreadStaticConnections.Count(); }
            }

            public override string ToString()
            {
                return string.Format("ConnectionCount = {0}", StaticConnections.Count() + ThreadStaticConnections.Count());
            }
        }

        #endregion

        private readonly ICollection<IMessageProcessorBusConnection> _connections;
        private readonly ThreadLocal<ICollection<IMessageProcessorBusConnection>> _threadLocalConnections;        
        private readonly IMessageProcessor _processor;  
        
        internal MessageProcessorBus(IMessageProcessor processor)
        {            
            _connections = new SynchronizedCollection<IMessageProcessorBusConnection>();
            _threadLocalConnections = new ThreadLocal<ICollection<IMessageProcessorBusConnection>>(() => new List<IMessageProcessorBusConnection>());  
            _processor = processor;                
        }

        #region [====== Connect ======]

        public IConnection Connect(object handler, bool openConnection)
        {
            return Connect(handler, openConnection, _connections);
        }

        public IConnection Connect<TMessage>(Action<TMessage> handler, bool openConnection) where TMessage : class
        {
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }
            var connection = new MessageProcessorBusConnection<TMessage>(_connections, handler);

            if (openConnection)
            {
                connection.Open();
            }
            return connection;
        }

        public IConnection Connect<TMessage>(IMessageHandler<TMessage> handler, bool openConnection) where TMessage : class
        {
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }
            var connection = new MessageProcessorBusConnection<TMessage>(_connections, handler);

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

        public IConnection ConnectThreadLocal<TMessage>(Action<TMessage> handler, bool openConnection) where TMessage : class
        {
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }
            var connection = new MessageProcessorBusConnection<TMessage>(_threadLocalConnections.Value, handler);

            if (openConnection)
            {
                connection.Open();
            }
            return connection;
        }
        
        public IConnection ConnectThreadLocal<TMessage>(IMessageHandler<TMessage> handler, bool openConnection) where TMessage : class
        {
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }
            var connection = new MessageProcessorBusConnection<TMessage>(_threadLocalConnections.Value, handler);

            if (openConnection)
            {
                connection.Open();
            }
            return connection;
        }

        #endregion        

        public void Publish<TMessage>(TMessage message) where TMessage : class, IMessage<TMessage>
        {
            InvokeRegisteredHandlers(message);
            InvokeConnectedHandlers(message);                            
        }

        private void InvokeRegisteredHandlers<TMessage>(TMessage message) where TMessage : class, IMessage<TMessage>
        {
            _processor.Handle(message);
        }              

        private void InvokeConnectedHandlers<TMessage>(TMessage message) where TMessage : class, IMessage<TMessage>
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

        private static IConnection Connect(object handler, bool openConnection, ICollection<IMessageProcessorBusConnection> connections)
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

        private static IEnumerable<IConnection> CreateConnections(object handler, ICollection<IMessageProcessorBusConnection> connections)
        {
            return from interfaceType in handler.GetType().GetInterfaces()
                   where IsMessageHandler(interfaceType)
                   select CreateConnection(handler, connections, interfaceType);
        }

        private static bool IsMessageHandler(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IMessageHandler<>);
        }

        private static IConnection CreateConnection(object handler, ICollection<IMessageProcessorBusConnection> connections, Type interfaceType)
        {
            var messageType = interfaceType.GetGenericArguments()[0];
            var connectionTypeDefinition = typeof(MessageProcessorBusConnection<>);
            var connectionType = connectionTypeDefinition.MakeGenericType(messageType);            
            var constructor = connectionType.GetConstructor(new [] { typeof(ICollection<IMessageProcessorBusConnection>), interfaceType });

            return (IConnection) constructor.Invoke(new [] { connections, handler });
        }
    }
}
