using System;
using System.Threading.Tasks;

namespace Kingo.Messaging
{    
    internal sealed class MessageHandlerDecorator<TMessage> : MessageHandler<TMessage>
    {
        private readonly MessageHandlerContext _context;
        private readonly IMessageHandler<TMessage> _handler;        
        
        public MessageHandlerDecorator(MessageHandlerContext context, IMessageHandler<TMessage> handler) :
            base(new TypeAttributeProvider(handler.GetType()), Messaging.MethodAttributeProvider.FromMessageHandler(handler))
        {
            _context = context;
            _handler = handler;
        }

        public MessageHandlerDecorator(MessageHandlerContext context, IMessageHandler<TMessage> handler, Type handlerType, Type interfaceType) :
            base(new TypeAttributeProvider(handlerType), Messaging.MethodAttributeProvider.FromMessageHandler(handlerType, interfaceType))
        {
            _context = context;
            _handler = handler;           
        }               
        
        public override async Task<HandleAsyncResult> HandleAsync(TMessage message, IMicroProcessorContext context)
        {
            await _handler.HandleAsync(message, context);

            return _context.CreateHandleAsyncResult();
        }
        
        public override string ToString() =>
            MicroProcessorPipeline.ToString(_handler);
    }
}
