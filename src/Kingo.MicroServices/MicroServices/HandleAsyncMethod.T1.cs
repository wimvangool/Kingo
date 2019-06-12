﻿using System.Threading.Tasks;

namespace Kingo.MicroServices
{   
    internal sealed class HandleAsyncMethod<TMessage> : HandleAsyncMethod, IMessageHandler<TMessage>
    {                
        private readonly IMessageHandler<TMessage> _messageHandler;

        public HandleAsyncMethod(IMessageHandler<TMessage> messageHandler) :
            this(messageHandler, MessageHandlerType.FromInstance(messageHandler), MessageHandlerInterface.FromType<TMessage>()) { }

        public HandleAsyncMethod(IMessageHandler<TMessage> messageHandler, MessageHandler component, MessageHandlerInterface @interface) :
            base(component, @interface)
        {            
            _messageHandler = messageHandler;
        }

        /// <inheritdoc />
        public Task HandleAsync(TMessage message, MessageHandlerOperationContext context) =>
            _messageHandler.HandleAsync(message, context);                    
    }
}