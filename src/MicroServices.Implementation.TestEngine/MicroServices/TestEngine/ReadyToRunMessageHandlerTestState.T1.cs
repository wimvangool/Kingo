using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Kingo.Reflection;

namespace Kingo.MicroServices.TestEngine
{
    internal sealed class ReadyToRunMessageHandlerTestState<TMessage> : MicroProcessorTestState, IReadyToRunMessageHandlerTestState<TMessage>
    {
        private readonly MicroProcessorTest _test;
        private readonly IEnumerable<MicroProcessorTestOperation> _givenOperations;
        private readonly MessageHandlerTestOperation<TMessage> _whenOperation;

        public ReadyToRunMessageHandlerTestState(MicroProcessorTest test, IEnumerable<MicroProcessorTestOperation> givenOperations, MessageHandlerTestOperation<TMessage> whenOperation)
        {
            _test = test;
            _givenOperations = givenOperations;
            _whenOperation = whenOperation;
        }

        protected override MicroProcessorTest Test =>
            _test;

        public override string ToString() =>
            $"Ready to process message of type '{typeof(TMessage).FriendlyName()}' with message handler of type '{_whenOperation.MessageHandlerType.FriendlyName()}'...";

        public async Task ThenOutputIs<TException>(Action<TMessage, TException, MicroProcessorTestContext> assertMethod = null) where TException : Exception =>
            (await RunMessageHandlerTestAsync()).AssertOutputIsException(assertMethod);

        public async Task ThenOutputIsMessageStream(Action<TMessage, MessageStream, MicroProcessorTestContext> assertMethod = null) =>
            (await RunMessageHandlerTestAsync()).AssertOutputIsMessageStream(assertMethod);

        private async Task<VerifyingMessageHandlerTestOutputState<TMessage>> RunMessageHandlerTestAsync() =>
            await Test.MoveToState(this, new RunningMessageHandlerTestState<TMessage>(_test, _givenOperations)).RunAsync(_whenOperation);
    }
}
