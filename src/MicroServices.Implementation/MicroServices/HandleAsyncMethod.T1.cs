using System.Threading.Tasks;

namespace Kingo.MicroServices
{   
    internal sealed class HandleAsyncMethod<TMessage> : HandleAsyncMethod, IMessageHandler<TMessage>
    {                
        private readonly IMessageHandler<TMessage> _messageHandler;

        public HandleAsyncMethod(IMessageHandler<TMessage> messageHandler) :
            this(messageHandler, MessageHandlerType.FromInstance(messageHandler), MessageHandlerInterface.FromType<TMessage>()) { }

        public HandleAsyncMethod(IMessageHandler<TMessage> messageHandler, MessageHandlerComponent component, MessageHandlerInterface @interface) :
            this(messageHandler, @interface.CreateMethod(component)) { }

        public HandleAsyncMethod(IMessageHandler<TMessage> messageHandler, HandleAsyncMethod method) :
            base(method)
        {            
            _messageHandler = messageHandler;
        }

        /// <inheritdoc />
        public Task HandleAsync(TMessage message, MessageHandlerOperationContext context) =>
            _messageHandler.HandleAsync(message, context);        
    }
}
