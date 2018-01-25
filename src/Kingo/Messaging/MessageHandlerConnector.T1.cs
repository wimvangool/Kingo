using System.Threading.Tasks;

namespace Kingo.Messaging
{
    internal sealed class MessageHandlerConnector<TMessage> : MessageHandler<TMessage>
    {
        private readonly MessageHandler<TMessage> _nextHandler;
        private readonly IMicroProcessorFilter _filter;

        public MessageHandlerConnector(MessageHandler<TMessage> nextHandler, IMicroProcessorFilter filter)
        {
            _nextHandler = nextHandler;
            _filter = filter;
        }

        protected override ITypeAttributeProvider TypeAttributeProvider =>
            _nextHandler;

        protected override IMethodAttributeProvider MethodAttributeProvider =>
            _nextHandler;

        public override Task<HandleAsyncResult> HandleAsync(TMessage message, IMicroProcessorContext context) =>
            _filter.HandleAsync(_nextHandler, message, context);

        public override void Accept(IMicroProcessorFilterVisitor visitor)
        {
            if (visitor != null)
            {
                visitor.Visit(_filter);

                _nextHandler.Accept(visitor);
            }
        }
    }
}
