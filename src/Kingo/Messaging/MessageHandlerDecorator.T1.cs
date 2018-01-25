using System;
using System.Threading.Tasks;

namespace Kingo.Messaging
{    
    internal sealed class MessageHandlerDecorator<TMessage> : MessageHandler<TMessage>
    {
        private readonly MessageHandlerContext _context;
        private readonly IMessageHandler<TMessage> _handler;        
        
        public MessageHandlerDecorator(MessageHandlerContext context, IMessageHandler<TMessage> handler)
        {
            _context = context;
            _handler = handler;

            TypeAttributeProvider = new TypeAttributeProvider(handler.GetType());
            MethodAttributeProvider = Messaging.MethodAttributeProvider.FromMessageHandler(handler);
        }

        public MessageHandlerDecorator(MessageHandlerContext context, IMessageHandler<TMessage> handler, Type handlerType, Type interfaceType)
        {
            _context = context;
            _handler = handler;

            TypeAttributeProvider = new TypeAttributeProvider(handlerType);
            MethodAttributeProvider = Messaging.MethodAttributeProvider.FromMessageHandler(handlerType, interfaceType);
        }
        
        protected override ITypeAttributeProvider TypeAttributeProvider
        {
            get;
        }
        
        protected override IMethodAttributeProvider MethodAttributeProvider
        {
            get;
        }
        
        public override async Task<HandleAsyncResult> HandleAsync(TMessage message, IMicroProcessorContext context)
        {
            await _handler.HandleAsync(message, context);

            return _context.CreateHandleAsyncResult();
        }

        /// <inheritdoc />
        public override void Accept(IMicroProcessorFilterVisitor visitor) =>
            visitor?.Visit(_handler);
    }
}
