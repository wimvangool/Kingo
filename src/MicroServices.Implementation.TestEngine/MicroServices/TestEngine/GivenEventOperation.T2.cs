using System;
using System.Threading.Tasks;

namespace Kingo.MicroServices.TestEngine
{
    internal sealed class GivenEventOperation<TMessage, TMessageHandler> : GivenMessageOperation<TMessage>
        where TMessageHandler : class, IMessageHandler<TMessage>
    {
        public GivenEventOperation(Action<MessageHandlerTestOperationInfo<TMessage>, MicroProcessorTestContext> configurator) :
            base(configurator) { }

        public override Task RunAsync(MicroProcessorTestRunner runner, MicroProcessorTestContext context) =>
            CreateOperation(context.Resolve<TMessageHandler>()).RunAsync(runner, context);

        private GivenEventOperation<TMessage> CreateOperation(IMessageHandler<TMessage> messageHandler) =>
            new GivenEventOperation<TMessage>(this, messageHandler);
    }
}
