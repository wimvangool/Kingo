using System;
using System.Threading.Tasks;

namespace Kingo.MicroServices.TestEngine
{
    internal sealed class CommandOperation<TMessage> : MessageHandlerTestOperation<TMessage>
    {
        private readonly IMessageHandler<TMessage> _messageHandler;

        public CommandOperation(IMessageHandler<TMessage> messageHandler, MessageHandlerTestOperation<TMessage> operation) :
            base(operation)
        {
            _messageHandler = messageHandler;
        }

        public CommandOperation(IMessageHandler<TMessage> messageHandler, Action<MessageHandlerTestOperationInfo<TMessage>, MicroProcessorTestContext> configurator) :
            base(configurator)
        {
            _messageHandler = messageHandler ?? throw new ArgumentNullException(nameof(messageHandler));
        }

        public override Type MessageHandlerType =>
            _messageHandler.GetType();

        public override Task<MicroProcessorTestOperationId> RunAsync(RunningTestState state, MicroProcessorTestContext context) =>
            state.ExecuteCommandAsync(context, _messageHandler, CreateOperationInfo(context));
    }
}
