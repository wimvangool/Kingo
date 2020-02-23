using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Kingo.Reflection;

namespace Kingo.MicroServices.TestEngine
{
    internal sealed class ReadyToRunMessageHandlerTestState<TMessage> : ReadyToRunTestState, IReadyToRunMessageHandlerTestState<TMessage>
    {
        private readonly MessageHandlerTestOperation<TMessage> _whenOperation;

        public ReadyToRunMessageHandlerTestState(MicroProcessorTest test, IEnumerable<MicroProcessorTestOperation> givenOperations, MessageHandlerTestOperation<TMessage> whenOperation) :
            base(test, givenOperations)
        {
            _whenOperation = whenOperation;
        }

        public override string ToString() =>
            $"Ready to process message of type '{typeof(TMessage).FriendlyName()}' with message handler of type '{_whenOperation.MessageHandlerType.FriendlyName()}'...";

        public Task ThenOutputIsMessageStream(Action<TMessage, MessageStream, MicroProcessorTestContext> assertMethod = null) =>
            throw new NotImplementedException();
    }
}
