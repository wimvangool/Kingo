using System.Threading.Tasks;

namespace Kingo.Messaging
{
    internal sealed class MessageHandlerWrapper<TMessage> : MessageHandlerOrQuery<HandleAsyncResult>
    {
        private readonly MessageHandler<TMessage> _handler;
        private readonly TMessage _message;

        public MessageHandlerWrapper(MessageHandler<TMessage> handler, TMessage message)
        {
            _handler = handler;
            _message = message;
        }

        protected override ITypeAttributeProvider TypeAttributeProvider =>
            _handler;

        protected override IMethodAttributeProvider MethodAttributeProvider =>
            _handler;

        public override Task<HandleAsyncResult> InvokeAsync(IMicroProcessorContext context) =>
            _handler.HandleAsync(_message, context);

        public override string ToString() =>
            _handler.ToString();
    }
}
