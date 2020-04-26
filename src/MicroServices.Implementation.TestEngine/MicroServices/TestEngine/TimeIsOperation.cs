using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kingo.MicroServices.TestEngine
{
    internal sealed class TimeIsOperation : MicroProcessorTestOperation
    {
        private readonly DateTimeOffset _value;

        public TimeIsOperation(DateTimeOffset value)
        {
            _value = value;
        }

        public override string ToString() =>
            $"Time = {_value}";

        public override Task<MicroProcessorTestOperationId> RunAsync(RunningTestState state, Queue<MicroProcessorTestOperation> nextOperations, MicroProcessorTestContext context) =>
            state.SetClockToSpecificTimeAsync(_value, nextOperations, context);
    }
}
