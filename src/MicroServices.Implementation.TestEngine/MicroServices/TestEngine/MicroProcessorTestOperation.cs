using System.Threading.Tasks;

namespace Kingo.MicroServices.TestEngine
{
    internal abstract class MicroProcessorTestOperation
    {
        public abstract Task<MicroProcessorTestOperationId> RunAsync(RunningTestState state, MicroProcessorTestContext context);
    }
}
