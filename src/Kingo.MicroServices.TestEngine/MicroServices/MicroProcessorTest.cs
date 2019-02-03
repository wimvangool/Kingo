using System.Threading.Tasks;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Serves as a base-class for all test's.
    /// </summary>    
    public abstract class MicroProcessorTest : IMicroProcessorTest
    {
        Task IMicroProcessorTest.GivenAsync(IMessageHandlerTestProcessor processor, MicroProcessorTestContext context) =>
            GivenAsync(processor, context);

        /// <summary>
        /// Prepares this test for execution.
        /// </summary>
        /// <param name="processor">
        /// Processor that can be used to run existing tests or handle messages as a means to setup a desired program state.
        /// </param>
        /// <param name="context">The context in which the test is running.</param>        
        protected virtual Task GivenAsync(IMessageHandlerTestProcessor processor, MicroProcessorTestContext context) =>
            Task.CompletedTask;

        

            
        


        


    }
}
