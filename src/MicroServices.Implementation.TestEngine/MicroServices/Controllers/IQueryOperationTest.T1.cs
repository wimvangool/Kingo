using System.Threading.Tasks;

namespace Kingo.MicroServices.Controllers
{
    /// <summary>
    /// When implemented by a class, represents a test that executes a query with a <see cref="IMicroProcessor" />.
    /// </summary>    
    /// <typeparam name="TResponse">Type of the response of the test.</typeparam>
    public interface IReadOperationTest<TResponse> : IMicroProcessorOperationTest
    {
        /// <summary>
        /// Executes this test by executing a specific query using the specified <paramref name="processor"/>.
        /// </summary>
        /// <param name="processor">The processor to execute the query with.</param>
        /// <param name="context">The context in which the test is running.</param>                
        Task WhenAsync(IQueryOperationTestProcessor<TResponse> processor, MicroProcessorOperationTestContext context);

        /// <summary>
        /// Verifies the <paramref name="result"/> of this test.
        /// </summary>        
        /// <param name="result">The result of this test.</param>
        /// <param name="context">The context in which the test is running.</param>
        void Then(IQueryOperationTestResult<TResponse> result, MicroProcessorOperationTestContext context);
    }
}
