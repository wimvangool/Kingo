using System;
using System.Threading.Tasks;
using Kingo.Reflection;

namespace Kingo.MicroServices
{
    internal sealed class MessageHandlerInstance<TMessage> : MessageHandlerComponent, IMessageHandler<TMessage>
    {        
        private readonly IMessageHandler<TMessage> _messageHandler;
        private readonly InternalEventBusEndpointAttribute _attribute;

        public MessageHandlerInstance(Action<TMessage, IMessageHandlerOperationContext> messageHandler, InternalEventBusEndpointAttribute attribute) :
            this(MessageHandlerDecorator<TMessage>.Decorate(messageHandler), attribute) { }

        public MessageHandlerInstance(Func<TMessage, IMessageHandlerOperationContext, Task> messageHandler, InternalEventBusEndpointAttribute attribute) :
            this(MessageHandlerDecorator<TMessage>.Decorate(messageHandler), attribute) { }

        public MessageHandlerInstance(IMessageHandler<TMessage> messageHandler, InternalEventBusEndpointAttribute attribute = null) :
            base(MessageHandlerType.FromInstance(messageHandler))
        {
            _messageHandler = messageHandler;
            _attribute = attribute;
        }

        public Task HandleAsync(TMessage message, IMessageHandlerOperationContext context) =>
            _messageHandler.HandleAsync(message, context);

        internal override object ResolveMessageHandler(IServiceProvider serviceProvider) =>
            _messageHandler;

        internal override bool IsInternalEventBusEndpoint(MessageHandlerInterface @interface, out InternalEventBusEndpointAttribute attribute)
        {
            if (_attribute == null)
            {
                return base.IsInternalEventBusEndpoint(@interface, out attribute);
            }
            attribute = _attribute;
            return true;
        }

        public override string ToString() =>
            _messageHandler.GetType().FriendlyName();
    }
}
