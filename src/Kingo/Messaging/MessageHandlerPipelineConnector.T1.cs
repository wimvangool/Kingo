using System.Threading.Tasks;

namespace Kingo.Messaging
{
    internal sealed class MessageHandlerPipelineConnector<TMessage> : MessageHandler<TMessage>
    {
        private readonly MessageHandler<TMessage> _nextHandler;
        private readonly IMicroProcessorFilter _filter;

        public MessageHandlerPipelineConnector(MessageHandler<TMessage> nextHandler, IMicroProcessorFilter filter) :
            base(nextHandler, nextHandler)
        {
            _nextHandler = nextHandler;
            _filter = filter;
        }        

        public override Task<HandleAsyncResult> HandleAsync(TMessage message, IMicroProcessorContext context) =>
            _filter.HandleAsync(_nextHandler, message, context);

        public override string ToString() =>
            MicroProcessorPipeline.ToString(_filter, _nextHandler);
    }
}
