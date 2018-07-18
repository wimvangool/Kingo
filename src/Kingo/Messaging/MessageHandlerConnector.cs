using System.Threading.Tasks;

namespace Kingo.Messaging
{
    internal sealed class MessageHandlerConnector : MessageHandler
    {
        private readonly MessageHandler _nextHandler;
        private readonly IMicroProcessorFilter _filter;

        public MessageHandlerConnector(MessageHandler nextHandler, IMicroProcessorFilter filter)             
        {
            _nextHandler = nextHandler;
            _filter = filter;
        }

        protected override ITypeAttributeProvider TypeAttributeProvider =>
            _nextHandler;

        protected override IMethodAttributeProvider MethodAttributeProvider =>
            _nextHandler;

        public override Task<InvokeAsyncResult<IMessageStream>> InvokeAsync(MicroProcessorContext context)
        {
            if (_filter.IsEnabled(context))
            {
                return _filter.InvokeMessageHandlerAsync(_nextHandler, context);
            }
            return _nextHandler.InvokeAsync(context);
        }

        public override string ToString() =>
            MicroProcessorPipeline.ToString(_filter, _nextHandler);
    }
}
