using System;
using System.Threading.Tasks;

namespace Kingo.MicroServices.TestEngine
{
    internal sealed class EventOperation<TMessage> : MessageHandlerTestOperation<TMessage>
    {
        private readonly IMessageHandler<TMessage> _messageHandler;

        public EventOperation(MessageHandlerTestOperation<TMessage> operation, IMessageHandler<TMessage> messageHandler) :
            base(operation)
        {
            _messageHandler = messageHandler;
        }

        public EventOperation(Action<MessageHandlerTestOperationInfo<TMessage>, MicroProcessorTestContext> configurator, IMessageHandler<TMessage> messageHandler) :
            base(configurator)
        {
            _messageHandler = messageHandler ?? throw new ArgumentNullException(nameof(messageHandler));
        }

        public override Type MessageHandlerType =>
            _messageHandler.GetType();

        public override Task<MicroProcessorTestOperationId> RunAsync(RunningTestState state, MicroProcessorTestContext context) =>
            state.HandleEventAsync(context, _messageHandler, CreateOperationInfo(context));
    }
}
