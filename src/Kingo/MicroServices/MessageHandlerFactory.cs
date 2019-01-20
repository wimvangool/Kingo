using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Kingo.MicroServices
{        
    internal sealed class MessageHandlerFactory : IMessageHandlerFactory
    {                
        public static readonly IMessageHandlerFactory Null = new MessageHandlerFactory(Enumerable.Empty<MessageHandlerClass>(), Enumerable.Empty<MessageHandlerInstance>(), new ServiceCollection());

        private readonly MessageHandlerClass[] _messageHandlerClasses;
        private readonly MessageHandlerInstance[] _messageHandlerInstances;
        private readonly IServiceCollection _services;
              
        public MessageHandlerFactory(IEnumerable<MessageHandlerClass> messageHandlerClasses, IEnumerable<MessageHandlerInstance> messageHandlerInstances, IServiceCollection services)
        {
            _messageHandlerClasses = messageHandlerClasses.ToArray();
            _messageHandlerInstances = messageHandlerInstances.ToArray();
            _services = services;
        }

        public IServiceProvider CreateServiceProvider() =>
            _services.BuildServiceProvider();
        
        public override string ToString() =>
            $"{_messageHandlerClasses.Length + _messageHandlerInstances.Length} MessageHandler(s) Registered";

        #region [====== Resolve ======]                            
        
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

        #endregion             
    }
}
