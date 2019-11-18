using System.Threading.Tasks;

namespace Kingo.MicroServices.Controllers
{
    /// <summary>
    /// When implemented by a class, represents a test that executes a query with a <see cref="IMicroProcessor" />.
    /// </summary>
    /// <typeparam name="TRequest">Type of the request of the test.</typeparam>
    /// <typeparam name="TResponse">Type of the response of the test.</typeparam>
    public interface IQueryOperationTest<TRequest, TResponse> : IMicroProcessorOperationTest
    {
        /// <summary>
        /// Executes this test by executing a specific query using the specified <paramref name="processor"/>.
        /// </summary>
        /// <param name="processor">The processor to execute the query with.</param>
        /// <param name="context">The context in which the test is running.</param>                
        Task WhenAsync(IQueryOperationTestProcessor<TRequest, TResponse> processor, MicroProcessorOperationTestContext context);

        /// <summary>
        /// Verifies the <paramref name="result"/> of this test.
        /// </summary>
        /// <param name="request">Request that was executed by the query.</param>        
        /// <param name="result">The result of this test.</param>
        /// <param name="context">The context in which the test is running.</param>
        void Then(TRequest request, IQueryOperationTestResult<TResponse> result, MicroProcessorOperationTestContext context);
    }
}
