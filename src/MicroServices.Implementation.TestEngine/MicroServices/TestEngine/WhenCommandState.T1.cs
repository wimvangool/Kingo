using System;
using System.Collections.Generic;
using System.Text;
using Kingo.Reflection;
using static Kingo.MicroServices.TestEngine.MicroProcessorTestContext;

namespace Kingo.MicroServices.TestEngine
{
    internal sealed class WhenCommandState<TCommand> : WhenCommandOrEventState<TCommand>, IWhenCommandState<TCommand>
    {
        public WhenCommandState(MicroProcessorTest test, MicroProcessorTestOperationQueue givenOperations) :
            base(test, givenOperations) { }

        public override string ToString() =>
            ToString("command-handler");

        public IReadyToRunMessageHandlerTestState<TCommand> IsExecutedBy<TMessageHandler>(TCommand message) where TMessageHandler : class, IMessageHandler<TCommand> =>
            IsExecutedBy<TMessageHandler>(ConfigureMessage(message));

        public IReadyToRunMessageHandlerTestState<TCommand> IsExecutedBy<TMessageHandler>(Action<MessageHandlerTestOperationInfo<TCommand>, MicroProcessorTestContext> configurator) where TMessageHandler : class, IMessageHandler<TCommand> =>
            MoveToReadyToRunMessageHandlerTestState(new CommandOperation<TCommand, TMessageHandler>(configurator));

        public IReadyToRunMessageHandlerTestState<TCommand> IsExecutedBy(IMessageHandler<TCommand> messageHandler, Action<MessageHandlerTestOperationInfo<TCommand>, MicroProcessorTestContext> configurator) =>
            MoveToReadyToRunMessageHandlerTestState(new CommandOperation<TCommand>(messageHandler, configurator));
    }
}
