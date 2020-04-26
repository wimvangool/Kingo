using System;
using static Kingo.MicroServices.TestEngine.MicroProcessorTestContext;

namespace Kingo.MicroServices.TestEngine
{
    internal sealed class WhenEventState<TEvent> : WhenCommandOrEventState<TEvent>, IWhenEventState<TEvent>
    {
        public WhenEventState(MicroProcessorTest test, MicroProcessorTestOperationQueue givenOperations) :
            base(test, givenOperations) { }

        public override string ToString() =>
            ToString("event-handler");

        public IReadyToRunMessageHandlerTestState<TEvent> IsHandledBy<TMessageHandler>(TEvent message) where TMessageHandler : class, IMessageHandler<TEvent> =>
            IsHandledBy<TMessageHandler>(ConfigureMessage(message));

        public IReadyToRunMessageHandlerTestState<TEvent> IsHandledBy<TMessageHandler>(Action<MessageHandlerTestOperationInfo<TEvent>, MicroProcessorTestContext> configurator) where TMessageHandler : class, IMessageHandler<TEvent> =>
            MoveToReadyToRunMessageHandlerTestState(new EventOperation<TEvent, TMessageHandler>(configurator));

        public IReadyToRunMessageHandlerTestState<TEvent> IsHandledBy(IMessageHandler<TEvent> messageHandler, Action<MessageHandlerTestOperationInfo<TEvent>, MicroProcessorTestContext> configurator) =>
            MoveToReadyToRunMessageHandlerTestState(new EventOperation<TEvent>(messageHandler, configurator));
    }
}
