using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kingo.MicroServices
{
    internal sealed class TimeHasPassedOperation : MicroProcessorTestOperation
    {
        private readonly TimeSpan _offset;

        public TimeHasPassedOperation(TimeSpan offset)
        {
            _offset = offset;
        }

        public override string ToString() =>
            $"Time += {_offset}";

        public override Task<MicroProcessorTestOperationId> RunAsync(RunningTestState state, Queue<MicroProcessorTestOperation> nextOperations, MicroProcessorTestContext context) =>
            state.ShiftClockBySpecificOffsetAsync(_offset, nextOperations, context);
    }
}
