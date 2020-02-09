using System;
using System.Threading.Tasks;
using Kingo.Clocks;

namespace Kingo.MicroServices.TestEngine
{
    internal sealed class SetClockToSpecificTimeOperation : GivenOperation
    {
        private readonly DateTimeOffset _value;

        public SetClockToSpecificTimeOperation(DateTimeOffset value)
        {
            _value = value;
        }

        public override async Task RunAsync(MicroProcessorTestRunner runner, MicroProcessorTestContext context)
        {
            using (Clock.OverrideAsyncLocal(Clock.Current.SetToSpecificTime(_value)))
            {
                await runner.GivenAsync(context);
            }
        }
    }
}
