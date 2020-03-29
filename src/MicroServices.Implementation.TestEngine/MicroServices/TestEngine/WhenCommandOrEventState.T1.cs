using System;
using System.Collections.Generic;
using System.Text;
using Kingo.Reflection;

namespace Kingo.MicroServices.TestEngine
{
    internal sealed class WhenCommandOrEventState<TMessage> : MicroProcessorTestState, IWhenCommandOrEventState<TMessage>
    {
        private readonly MicroProcessorTest _test;
        private readonly MicroProcessorTestOperationQueue _givenOperations;

        public WhenCommandOrEventState(MicroProcessorTest test, MicroProcessorTestOperationQueue givenOperations)
        {
            _test = test;
            _givenOperations = givenOperations;
        }

        protected override MicroProcessorTest Test =>
            _test;

        public override string ToString() =>
            $"Configuring a message handler of type '{typeof(IMessageHandler<TMessage>).FriendlyName()}'...";

        public IReadyToRunMessageHandlerTestState<TMessage> IsExecutedBy<TMessageHandler>(Action<MessageHandlerTestOperationInfo<TMessage>, MicroProcessorTestContext> configurator) where TMessageHandler : class, IMessageHandler<TMessage> =>
            MoveToReadyToRunMessageHandlerTestState(new CommandOperation<TMessage, TMessageHandler>(configurator));

        public IReadyToRunMessageHandlerTestState<TMessage> IsExecutedByCommandHandler(Action<MessageHandlerTestOperationInfo<TMessage>, MicroProcessorTestContext> configurator, IMessageHandler<TMessage> messageHandler) =>
            MoveToReadyToRunMessageHandlerTestState(new CommandOperation<TMessage>(configurator, messageHandler));

        public IReadyToRunMessageHandlerTestState<TMessage> IsHandledBy<TMessageHandler>(Action<MessageHandlerTestOperationInfo<TMessage>, MicroProcessorTestContext> configurator) where TMessageHandler : class, IMessageHandler<TMessage> =>
            MoveToReadyToRunMessageHandlerTestState(new EventOperation<TMessage, TMessageHandler>(configurator));

        public IReadyToRunMessageHandlerTestState<TMessage> IsHandledByEventHandler(Action<MessageHandlerTestOperationInfo<TMessage>, MicroProcessorTestContext> configurator, IMessageHandler<TMessage> messageHandler) =>
            MoveToReadyToRunMessageHandlerTestState(new EventOperation<TMessage>(configurator, messageHandler));

        private IReadyToRunMessageHandlerTestState<TMessage> MoveToReadyToRunMessageHandlerTestState(MessageHandlerTestOperation<TMessage> whenOperation) =>
            Test.MoveToState(this, new ReadyToRunMessageHandlerTestState<TMessage>(_test, _givenOperations, whenOperation));
    }
}
