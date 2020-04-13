using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kingo.MicroServices.TestEngine
{
    internal sealed class EventOperation<TMessage> : MessageHandlerTestOperation<TMessage>
    {
        private readonly IMessageHandler<TMessage> _messageHandler;

        public EventOperation(IMessageHandler<TMessage> messageHandler, MessageHandlerTestOperation<TMessage> operation) :
            base(operation)
        {
            _messageHandler = messageHandler;
        }

        public EventOperation(IMessageHandler<TMessage> messageHandler, Action<MessageHandlerTestOperationInfo<TMessage>, MicroProcessorTestContext> configurator) :
            base(configurator)
        {
            _messageHandler = messageHandler ?? throw new ArgumentNullException(nameof(messageHandler));
        }

        public override Type MessageHandlerType =>
            _messageHandler.GetType();

        public override Task<MicroProcessorTestOperationId> RunAsync(RunningTestState state, Queue<MicroProcessorTestOperation> nextOperations, MicroProcessorTestContext context) =>
            state.HandleEventAsync(context, _messageHandler, CreateOperationInfo(context));
    }
}
