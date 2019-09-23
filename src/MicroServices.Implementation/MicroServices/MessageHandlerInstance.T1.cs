using System;
using System.Threading.Tasks;

namespace Kingo.MicroServices
{
    internal sealed class MessageHandlerInstance<TMessage> : MessageHandler, IMessageHandler<TMessage>
    {        
        private readonly IMessageHandler<TMessage> _messageHandler;        

        public MessageHandlerInstance(Action<TMessage, IMessageHandlerOperationContext> messageHandler) :
            this(MessageHandlerDecorator<TMessage>.Decorate(messageHandler)) { }

        public MessageHandlerInstance(Func<TMessage, IMessageHandlerOperationContext, Task> messageHandler) :
            this(MessageHandlerDecorator<TMessage>.Decorate(messageHandler)) { }

        private MessageHandlerInstance(IMessageHandler<TMessage> messageHandler) :
            base(MessageHandlerType.FromInstance(messageHandler), MessageHandlerInterface.FromType<TMessage>())
        {
            _messageHandler = messageHandler;            
        }

        public Task HandleAsync(TMessage message, IMessageHandlerOperationContext context) =>
            _messageHandler.HandleAsync(message, context);

        internal override object ResolveMessageHandler(IServiceProvider serviceProvider) =>
            _messageHandler;

        internal override bool IsInternalEventBusEndpoint(MessageHandlerInterface @interface, out InternalEventBusEndpointAttribute attribute)
        {
            attribute = new InternalEventBusEndpointAttribute();
            return true;
        }

        #region [====== Equals, GetHashCode & ToString ======]

        public override bool Equals(MicroProcessorComponent other) =>
            Equals(other as MessageHandlerInstance<TMessage>);

        public bool Equals(MessageHandlerInstance<TMessage> other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }
            return _messageHandler.Equals(other._messageHandler);          
        }

        public override int GetHashCode() =>
            _messageHandler.GetHashCode();

        public override string ToString() =>
            _messageHandler.ToString();

        #endregion
    }
}
