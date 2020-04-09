using System;
using System.Collections.Generic;
using System.Text;
using Kingo.Reflection;
using static Kingo.MicroServices.TestEngine.MicroProcessorTestContext;

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

        public IReadyToRunMessageHandlerTestState<TMessage> IsExecutedBy<TMessageHandler>(TMessage message) where TMessageHandler : class, IMessageHandler<TMessage> =>
            IsExecutedBy<TMessageHandler>(ToConfigurator(message));

        public IReadyToRunMessageHandlerTestState<TMessage> IsExecutedBy<TMessageHandler>(Action<MessageHandlerTestOperationInfo<TMessage>, MicroProcessorTestContext> configurator) where TMessageHandler : class, IMessageHandler<TMessage> =>
            MoveToReadyToRunMessageHandlerTestState(new CommandOperation<TMessage, TMessageHandler>(configurator));

        public IReadyToRunMessageHandlerTestState<TMessage> IsExecutedBy(IMessageHandler<TMessage> messageHandler, Action<MessageHandlerTestOperationInfo<TMessage>, MicroProcessorTestContext> configurator) =>
            MoveToReadyToRunMessageHandlerTestState(new CommandOperation<TMessage>(messageHandler, configurator));

        public IReadyToRunMessageHandlerTestState<TMessage> IsHandledBy<TMessageHandler>(TMessage message) where TMessageHandler : class, IMessageHandler<TMessage> =>
            IsHandledBy<TMessageHandler>(ToConfigurator(message));

        public IReadyToRunMessageHandlerTestState<TMessage> IsHandledBy<TMessageHandler>(Action<MessageHandlerTestOperationInfo<TMessage>, MicroProcessorTestContext> configurator) where TMessageHandler : class, IMessageHandler<TMessage> =>
            MoveToReadyToRunMessageHandlerTestState(new EventOperation<TMessage, TMessageHandler>(configurator));

        public IReadyToRunMessageHandlerTestState<TMessage> IsHandledBy(IMessageHandler<TMessage> messageHandler, Action<MessageHandlerTestOperationInfo<TMessage>, MicroProcessorTestContext> configurator) =>
            MoveToReadyToRunMessageHandlerTestState(new EventOperation<TMessage>(messageHandler, configurator));

        private IReadyToRunMessageHandlerTestState<TMessage> MoveToReadyToRunMessageHandlerTestState(MessageHandlerTestOperation<TMessage> whenOperation) =>
            Test.MoveToState(this, new ReadyToRunMessageHandlerTestState<TMessage>(_test, _givenOperations, whenOperation));
    }
}
