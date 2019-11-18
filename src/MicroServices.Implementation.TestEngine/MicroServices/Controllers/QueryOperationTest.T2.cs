using System.Threading.Tasks;

namespace Kingo.MicroServices.Controllers
{
    /// <summary>
    /// Serves as a base-class for all test's that execute a query and return the resulting response.
    /// </summary>
    /// <typeparam name="TRequest">Type of the request executed by the query.</typeparam>
    /// <typeparam name="TResponse">Type of the response returned by the query.</typeparam>
    public abstract class QueryOperationTest<TRequest, TResponse> : MicroProcessorOperationTest, IQueryOperationTest<TRequest, TResponse>
    {
        Task IQueryOperationTest<TRequest, TResponse>.WhenAsync(IQueryOperationTestProcessor<TRequest, TResponse> processor, MicroProcessorOperationTestContext context) =>
            WhenAsync(processor, context);

        /// <summary>
        /// Executes this test by executing a specific query using the specified <paramref name="processor"/>.
        /// </summary>
        /// <param name="processor">The processor to execute the query with.</param>
        /// <param name="context">The context in which the test is running.</param> 
        protected abstract Task WhenAsync(IQueryOperationTestProcessor<TRequest, TResponse> processor, MicroProcessorOperationTestContext context);

        void IQueryOperationTest<TRequest, TResponse>.Then(TRequest request, IQueryOperationTestResult<TResponse> result, MicroProcessorOperationTestContext context) =>
            Then(request, result, context);

        /// <summary>
        /// Verifies the <paramref name="result"/> of this test.
        /// </summary>
        /// <param name="request">Request that was executed by the query.</param>        
        /// <param name="result">The result of this test.</param>
        /// <param name="context">The context in which the test is running.</param>
        protected abstract void Then(TRequest request, IQueryOperationTestResult<TResponse> result, MicroProcessorOperationTestContext context);
    }
}
