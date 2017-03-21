using System.Threading.Tasks;

namespace Kingo.Messaging
{
    internal sealed class MessageHandlerConnector<TMessage> : MessageHandler<TMessage>
    {
        private readonly MessageHandler<TMessage> _nextHandler;
        private readonly IMicroProcessorPipeline _pipeline;

        public MessageHandlerConnector(MessageHandler<TMessage> nextHandler, IMicroProcessorPipeline pipeline)
        {
            _nextHandler = nextHandler;
            _pipeline = pipeline;
        }

        protected override ITypeAttributeProvider TypeAttributeProvider =>
            _nextHandler;

        protected override IMethodAttributeProvider MethodAttributeProvider =>
            _nextHandler;

        public override Task<HandleAsyncResult> HandleAsync(TMessage message, IMicroProcessorContext context) =>
            _pipeline.HandleAsync(_nextHandler, message, context);

        public override void Accept(IMicroProcessorPipelineVisitor visitor)
        {
            if (visitor != null)
            {
                visitor.Visit(_pipeline);

                _nextHandler.Accept(visitor);
            }
        }
    }
}
