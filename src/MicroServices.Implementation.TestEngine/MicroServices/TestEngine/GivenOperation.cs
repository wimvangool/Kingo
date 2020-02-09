using System.Threading.Tasks;

namespace Kingo.MicroServices.TestEngine
{
    internal abstract class GivenOperation
    {
        public abstract Task RunAsync(MicroProcessorTestRunner runner, MicroProcessorTestContext context);
    }
}
