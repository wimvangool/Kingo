﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Syztem.ComponentModel.Server
{           
    internal sealed class MessageProcessorBus : IMessageProcessorBus
    {        
        private readonly ICollection<IMessageProcessorBusConnection> _connections;        
        private readonly IMessageProcessor _processor;  
        
        internal MessageProcessorBus(IMessageProcessor processor)
        {            
            _connections = new SynchronizedCollection<IMessageProcessorBusConnection>();            
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

        public async Task PublishAsync<TMessage>(TMessage message) where TMessage : class, IMessage<TMessage>
        {
            await InvokeRegisteredHandlers(message);
            await InvokeConnectedHandlers(message);                            
        }

        private Task InvokeRegisteredHandlers<TMessage>(TMessage message) where TMessage : class, IMessage<TMessage>
        {
            return _processor.HandleAsync(message);
        }              

        private async Task InvokeConnectedHandlers<TMessage>(TMessage message) where TMessage : class, IMessage<TMessage>
        {
            foreach (var connection in _connections)
            {
                await connection.HandleAsync(_processor, message);
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
