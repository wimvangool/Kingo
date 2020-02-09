using System;
using System.Threading.Tasks;
using Kingo.Clocks;

namespace Kingo.MicroServices.TestEngine
{
    internal sealed class ShiftClockBySpecificPeriodOperation : GivenOperation
    {
        private readonly TimeSpan _value;

        public ShiftClockBySpecificPeriodOperation(TimeSpan value)
        {
            _value = value;
        }

        public override async Task RunAsync(MicroProcessorTestRunner runner, MicroProcessorTestContext context)
        {
            using (Clock.OverrideAsyncLocal(Clock.Current.Shift(_value)))
            {
                await runner.GivenAsync(context);
            }
        }
    }
}
