using System;

namespace Kingo.MicroServices
{
    internal sealed class MicroProcessorStub : MicroProcessor
    {
        public MicroProcessorStub(IMicroServiceBus serviceBus = null, IMessageHandlerFactory messageHandlers = null, IMicroProcessorPipelineFactory pipeline = null, IServiceProvider serviceProvider = null) :
            base(serviceBus, messageHandlers, pipeline, serviceProvider) { }
    }
}
