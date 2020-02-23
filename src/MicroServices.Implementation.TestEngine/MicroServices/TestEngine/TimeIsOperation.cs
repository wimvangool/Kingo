using System;
using System.Threading.Tasks;
using Kingo.Clocks;

namespace Kingo.MicroServices.TestEngine
{
    internal sealed class TimeIsOperation : MicroProcessorTestOperation
    {
        private readonly DateTimeOffset _value;

        public TimeIsOperation(DateTimeOffset value)
        {
            _value = value;
        }

        public override Task RunAsync(RunningTestState state, MicroProcessorTestContext context) =>
            state.SetClockToSpecificTimeAsync(context, _value);
    }
}
