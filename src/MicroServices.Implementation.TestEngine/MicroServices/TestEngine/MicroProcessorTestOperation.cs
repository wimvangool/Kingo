using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kingo.MicroServices.TestEngine
{
    internal abstract class MicroProcessorTestOperation
    {
        public abstract Task<MicroProcessorTestOperationId> RunAsync(RunningTestState state, Queue<MicroProcessorTestOperation> nextOperations, MicroProcessorTestContext context);
    }
}
