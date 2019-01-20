using System;

namespace Kingo.MicroServices
{
    internal sealed class MessageHandlerInstance<TMessage> : MessageHandlerInstance
    {
        private readonly IMessageHandler<TMessage> _handler;
        private readonly MicroProcessorOperationTypes _supportedOperationTypes;

        public MessageHandlerInstance(IMessageHandler<TMessage> handler, MicroProcessorOperationTypes? operationTypes)
        {           
            _handler = handler ?? throw new ArgumentNullException(nameof(handler));
            _supportedOperationTypes = operationTypes ?? DetermineOperationTypes(handler.GetType());
        }

        private static MicroProcessorOperationTypes DetermineOperationTypes(Type messageHandlerType)
        {
            if (MessageHandlerClass.TryGetMessageHandlerAttribute(messageHandlerType, out var attribute))
            {
                return attribute.SupportedOperationTypes;
            }
            return MessageHandlerConfiguration.DefaultOperationTypes;
        }

        public override bool TryCreateMessageHandler<TOther>(TOther message, MessageHandlerContext context, out MessageHandler handler)
        {
            if (context.Operation.IsSupported(_supportedOperationTypes) && message is TMessage messageOfSupportedType)
            {
                handler = new MessageHandlerDecorator<TMessage>(_handler, messageOfSupportedType, context);
                return true;
            }
            handler = null;
            return false;
        }

        public override string ToString() =>
            $"{_handler.GetType().FriendlyName()} ({typeof(IMessageHandler<TMessage>).FriendlyName()}) - {_supportedOperationTypes}";
    }
}
