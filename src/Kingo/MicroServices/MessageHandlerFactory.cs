using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security;

namespace Kingo.MicroServices
{        
    internal sealed class MessageHandlerFactory : IMessageHandlerFactory
    {
        #region [====== Null ======]        

        public static readonly MessageHandlerFactory Null = new MessageHandlerFactory(Enumerable.Empty<MessageHandlerClass>(), new SimpleServiceProvider());

        #endregion

        [DebuggerDisplay("Count = {_messageHandlers.Count}")]
        private readonly MessageHandlerClass[] _messageHandlers;
        private readonly IServiceProvider _serviceProvider;
              
        public MessageHandlerFactory(IEnumerable<MessageHandlerClass> messageHandlers, IServiceProvider serviceProvider)
        {
            _messageHandlers = messageHandlers.ToArray();
            _serviceProvider = serviceProvider;
        }        

        /// <inheritdoc />
        public override string ToString() =>
            $"{_messageHandlers.Length} MessageHandler(s) Registered";        

        #region [====== Resolve ======]                            

        /// <inheritdoc />
        public IEnumerable<MessageHandler> ResolveMessageHandlers<TMessage>(TMessage message, MessageHandlerContext context) =>
            from handlerClass in _messageHandlers
            let handlers = handlerClass.CreateInstancesInEveryRoleFor(message, context)
            from handler in handlers
            select handler;

        public object GetService(Type serviceType) =>
            _serviceProvider.GetService(serviceType);

        #endregion        
    }
}
