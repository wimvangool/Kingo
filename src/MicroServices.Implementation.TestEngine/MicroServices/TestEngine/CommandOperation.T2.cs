using System;
using System.Threading.Tasks;

namespace Kingo.MicroServices.TestEngine
{
    internal sealed class CommandOperation<TMessage, TMessageHandler> : MessageHandlerTestOperation<TMessage>
        where TMessageHandler : class, IMessageHandler<TMessage>
    {
        public CommandOperation(Action<MessageHandlerTestOperationInfo<TMessage>, MicroProcessorTestContext> configurator) :
            base(configurator) { }

        public override Type MessageHandlerType =>
            typeof(TMessageHandler);

        public override Task<MicroProcessorTestOperationId> RunAsync(RunningTestState state, MicroProcessorTestContext context) =>
            CreateOperation(context.Resolve<TMessageHandler>()).RunAsync(state, context);

        private CommandOperation<TMessage> CreateOperation(IMessageHandler<TMessage> messageHandler) =>
            new CommandOperation<TMessage>(messageHandler, this);
    }
}
