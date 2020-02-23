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

        public override Task RunAsync(RunningTestState state, MicroProcessorTestContext context) =>
            state.ShiftClockBySpecificPeriodAsync(context, _value);
    }
}
