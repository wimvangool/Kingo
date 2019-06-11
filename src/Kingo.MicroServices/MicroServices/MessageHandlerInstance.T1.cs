using System;
using System.Collections.Generic;
using Kingo.Reflection;

namespace Kingo.MicroServices
{
    internal sealed class MessageHandlerInstance<TMessage> : MessageHandlerInstance
    {
        private readonly IMessageHandler<TMessage> _messageHandler;

        public MessageHandlerInstance(IMessageHandler<TMessage> messageHandler, IMessageHandlerConfiguration configuration) :
            base(MessageHandlerType.FromInstance(messageHandler), MessageHandlerInterface.FromType<TMessage>(), configuration)
        {
            _messageHandler = messageHandler;
        }

        public override bool TryCreateMethod<TOther>(MicroProcessorOperationKinds operationKind, out HandleAsyncMethod<TOther> method)
        {
            if (operationKind.IsSupportedBy(GetSupportedOperations()) && _messageHandler is IMessageHandler<TOther> messageHandler)
            {
                method = new HandleAsyncMethod<TOther>(messageHandler, this, MessageHandlerInterface.FromType<TMessage>());
                return true;
            }
            method = null;
            return false;
        }        
    }
}
