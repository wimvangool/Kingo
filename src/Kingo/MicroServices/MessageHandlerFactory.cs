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
       
        public static readonly MessageHandlerFactory Null = new MessageHandlerFactory(Enumerable.Empty<MessageHandlerClass>(), Enumerable.Empty<MessageHandlerInstance>(), new NullServiceProvider());

        #endregion
        
        private readonly MessageHandlerClass[] _messageHandlerClasses;
        private readonly MessageHandlerInstance[] _messageHandlerInstances;
        private readonly IServiceProvider _serviceProvider;
              
        public MessageHandlerFactory(IEnumerable<MessageHandlerClass> messageHandlerClasses, IEnumerable<MessageHandlerInstance> messageHandlerInstances, IServiceProvider serviceProvider)
        {
            _messageHandlerClasses = messageHandlerClasses.ToArray();
            _messageHandlerInstances = messageHandlerInstances.ToArray();
            _serviceProvider = serviceProvider;
        }        

        /// <inheritdoc />
        public override string ToString() =>
            $"{_messageHandlerClasses.Length + _messageHandlerInstances.Length} MessageHandler(s) Registered";

        #region [====== Resolve ======]                            

        /// <inheritdoc />
        public IEnumerable<MessageHandler> ResolveMessageHandlers<TMessage>(TMessage message, MessageHandlerContext context) =>
            ResolveMessageHandlersFromClasses(message, context).Concat(ResolveMessageHandlersFromInstances(message, context));

        private IEnumerable<MessageHandler> ResolveMessageHandlersFromClasses<TMessage>(TMessage message, MessageHandlerContext context) =>
            from handlerClass in _messageHandlerClasses
            from handler in handlerClass.CreateInstancesInEveryRoleFor(message, context)
            select handler;

        private IEnumerable<MessageHandler> ResolveMessageHandlersFromInstances<TMessage>(TMessage message, MessageHandlerContext context)
        {
            foreach (var messageHandlerInstance in _messageHandlerInstances)
            {
                if (messageHandlerInstance.TryCreateMessageHandler(message, context, out var handler))
                {
                    yield return handler;
                }
            }
        }            

        public object GetService(Type serviceType) =>
            _serviceProvider.GetService(serviceType);

        #endregion        
    }
}
