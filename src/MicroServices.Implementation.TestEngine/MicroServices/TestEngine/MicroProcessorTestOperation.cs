using System.Threading.Tasks;

namespace Kingo.MicroServices.TestEngine
{
    internal abstract class MicroProcessorTestOperation
    {
        public abstract Task RunAsync(RunningTestState state, MicroProcessorTestContext context);
    }
}
