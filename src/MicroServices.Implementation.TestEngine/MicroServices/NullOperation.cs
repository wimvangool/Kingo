using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kingo.MicroServices
{
    internal sealed class NullOperation : MicroProcessorTestOperation
    {
        public override Task<MicroProcessorTestOperationId> RunAsync(RunningTestState state, Queue<MicroProcessorTestOperation> nextOperations, MicroProcessorTestContext context) =>
            Task.FromResult(MicroProcessorTestOperationId.Empty);
    }
}
