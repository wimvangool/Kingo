using System;
using System.Threading.Tasks;

namespace Kingo.MicroServices.TestEngine
{
    internal sealed class GivenCommandOperation<TMessage, TMessageHandler> : GivenMessageOperation<TMessage>
        where TMessageHandler : class, IMessageHandler<TMessage>
    {
        public GivenCommandOperation(Action<MessageHandlerTestOperationInfo<TMessage>, MicroProcessorTestContext> configurator) :
            base(configurator) { }

        public override Task RunAsync(MicroProcessorTestRunner runner, MicroProcessorTestContext context) =>
            CreateOperation(context.Resolve<TMessageHandler>()).RunAsync(runner, context);

        private GivenCommandOperation<TMessage> CreateOperation(IMessageHandler<TMessage> messageHandler) =>
            new GivenCommandOperation<TMessage>(this, messageHandler);
    }
}
