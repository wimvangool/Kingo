using System;

namespace YellowFlare.MessageProcessing
{
    internal sealed class LazyMessageHandlerPipeline<TMessage> : MessageHandlerPipelineDecorator<TMessage> where TMessage : class
    {
        private readonly MessageHandlerPipelineFactory _pipelineFactory;
        private readonly Lazy<IMessageHandler<TMessage>> _pipeline;

        public LazyMessageHandlerPipeline(IMessageHandlerPipeline<TMessage> handler, MessageHandlerPipelineFactory pipelineFactory) : base(handler)
        {
            _pipelineFactory = pipelineFactory;
            _pipeline = new Lazy<IMessageHandler<TMessage>>(CreatePipeline);
        }

        private IMessageHandler<TMessage> CreatePipeline()
        {
            return _pipelineFactory.CreatePipeline(Handler);
        }

        public override void Handle(TMessage message)
        {
            _pipeline.Value.Handle(message);
        }
    }
}
