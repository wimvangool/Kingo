using System.Threading.Tasks;

namespace Kingo.MicroServices.TestEngine
{
    internal sealed class NullOperation : MicroProcessorTestOperation
    {
        public override Task RunAsync(RunningTestState state, MicroProcessorTestContext context) =>
            Task.CompletedTask;
    }
}
