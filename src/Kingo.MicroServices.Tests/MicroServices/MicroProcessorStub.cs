using System;

namespace Kingo.MicroServices
{
    internal sealed class MicroProcessorStub : MicroProcessor
    {
        public MicroProcessorStub(IMessageHandlerFactory messageHandlers = null, IMicroProcessorPipelineFactory pipeline = null, IServiceProvider serviceProvider = null) :
            base(messageHandlers, pipeline, serviceProvider) { }
    }
}
