using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kingo.MicroServices
{
    internal sealed class EventOperation<TMessage, TMessageHandler> : MessageHandlerTestOperation<TMessage>
        where TMessageHandler : class, IMessageHandler<TMessage>
    {
        public EventOperation(Action<MessageHandlerTestOperationInfo<TMessage>, MicroProcessorTestContext> configurator) :
            base(configurator) { }

        public override Type MessageHandlerType =>
            typeof(TMessageHandler);

        public override Task<MicroProcessorTestOperationId> RunAsync(RunningTestState state, Queue<MicroProcessorTestOperation> nextOperations, MicroProcessorTestContext context) =>
            CreateOperation(context.Resolve<TMessageHandler>()).RunAsync(state, nextOperations, context);

        private EventOperation<TMessage> CreateOperation(IMessageHandler<TMessage> messageHandler) =>
            new EventOperation<TMessage>(messageHandler, this);
    }
}
