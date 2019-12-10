using System.Threading.Tasks;

namespace Kingo.MicroServices.Controllers
{
    /// <summary>
    /// When implemented by a class, represents a test that either handles a message or executes a query through a <see cref="IMicroProcessor"/>.
    /// </summary>    
    public interface IMicroProcessorOperationTest
    {
        /// <summary>
        /// Prepares the test for execution.
        /// </summary>
        /// <param name="processor">
        /// Processor that can be used to run existing tests or handle messages as a means to setup a desired program state.
        /// </param>
        /// <param name="context">The context in which the test is running.</param>                
        Task GivenAsync(IMicroProcessorOperationRunner processor, MicroProcessorOperationTestContext context);        
    }
}
