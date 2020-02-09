using System.Threading.Tasks;

namespace Kingo.MicroServices.TestEngine
{
    internal sealed class GivenNullOperation : GivenOperation
    {
        public override Task RunAsync(MicroProcessorTestRunner runner, MicroProcessorTestContext context) =>
            Task.CompletedTask;
    }
}
