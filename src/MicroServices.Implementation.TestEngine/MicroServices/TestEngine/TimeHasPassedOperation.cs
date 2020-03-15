using System;
using System.Threading.Tasks;
using Kingo.Clocks;

namespace Kingo.MicroServices.TestEngine
{
    internal sealed class TimeHasPassedOperation : MicroProcessorTestOperation
    {
        private readonly TimeSpan _value;

        public TimeHasPassedOperation(TimeSpan value)
        {
            _value = value;
        }

        public override string ToString() =>
            $"Time += {_value}";

        public override Task<MicroProcessorTestOperationId> RunAsync(RunningTestState state, MicroProcessorTestContext context) =>
            state.ShiftClockBySpecificPeriodAsync(context, _value);
    }
}
