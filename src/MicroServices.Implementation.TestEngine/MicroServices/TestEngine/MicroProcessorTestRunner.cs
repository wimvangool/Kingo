using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kingo.MicroServices.TestEngine
{
    internal abstract class MicroProcessorTestRunner
    {
        private readonly Queue<GivenOperation> _operations;

        protected MicroProcessorTestRunner(IEnumerable<GivenOperation> operations)
        {
            _operations = new Queue<GivenOperation>(operations);
        }

        public async Task GivenAsync(MicroProcessorTestContext context)
        {
            while (_operations.Count > 0)
            {
                await _operations.Dequeue().RunAsync(this, context);
            }
        }

        
    }
}
