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
        /// Executes this test by executing a specific query using the specified <paramref name="runner"/>.
        /// </summary>
        /// <param name="runner">The runner that will execute the query.</param>
        /// <param name="context">The context in which the test is running.</param>                
        Task WhenAsync(IQueryOperationRunner<TResponse> runner, MicroProcessorOperationTestContext context);

        /// <summary>
        /// Verifies the <paramref name="result"/> of this test.
        /// </summary>        
        /// <param name="result">The result of this test.</param>
        /// <param name="context">The context in which the test is running.</param>
        void Then(IQueryOperationTestResult<TResponse> result, MicroProcessorOperationTestContext context);
    }
}
