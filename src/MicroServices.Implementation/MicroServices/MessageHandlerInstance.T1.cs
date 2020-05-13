using System;
using System.Threading.Tasks;
using Kingo.Reflection;

namespace Kingo.MicroServices
{
    internal sealed class MessageHandlerInstance<TMessage> : MessageHandlerComponent, IMessageHandler<TMessage>
    {        
        private readonly IMessageHandler<TMessage> _messageHandler;

        public MessageHandlerInstance(Action<TMessage, MessageHandlerOperationContext> messageHandler) :
            this(MessageHandlerDecorator<TMessage>.Decorate(messageHandler)) { }

        public MessageHandlerInstance(Func<TMessage, MessageHandlerOperationContext, Task> messageHandler) :
            this(MessageHandlerDecorator<TMessage>.Decorate(messageHandler)) { }

        public MessageHandlerInstance(IMessageHandler<TMessage> messageHandler) :
            base(MessageHandlerType.FromInstance(messageHandler))
        {
            _messageHandler = messageHandler;
        }

        public Task HandleAsync(TMessage message, MessageHandlerOperationContext context) =>
            _messageHandler.HandleAsync(message, context);

        internal override object ResolveMessageHandler(IServiceProvider serviceProvider) =>
            _messageHandler;

        internal override bool IsInternalServiceBusEndpoint(MessageHandlerInterface @interface) =>
            true;

        public override string ToString() =>
            _messageHandler.GetType().FriendlyName();
    }
}
