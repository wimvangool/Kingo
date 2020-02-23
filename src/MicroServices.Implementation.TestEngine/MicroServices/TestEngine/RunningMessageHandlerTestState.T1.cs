using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kingo.MicroServices.TestEngine
{
    internal sealed class RunningMessageHandlerTestState<TMessage> : RunningTestState
    {
        public RunningMessageHandlerTestState(MicroProcessorTest test, IEnumerable<MicroProcessorTestOperation> operations) :
            base(test, operations) { }

        public Task<object> RunAsync(MessageHandlerTestOperation<TMessage> operation) =>
            throw new NotImplementedException();
    }
}
