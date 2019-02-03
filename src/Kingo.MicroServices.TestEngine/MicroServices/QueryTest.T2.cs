using System.Threading.Tasks;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Serves as a base-class for all test's that execute a query and return the resulting response.
    /// </summary>
    /// <typeparam name="TRequest">Type of the request executed by the query.</typeparam>
    /// <typeparam name="TResponse">Type of the response returned by the query.</typeparam>
    public abstract class QueryTest<TRequest, TResponse> : MicroProcessorTest, IQueryTest<TRequest, TResponse>
    {
        Task IQueryTest<TRequest, TResponse>.WhenAsync(IQueryProcessor<TRequest, TResponse> processor, MicroProcessorTestContext context) =>
            WhenAsync(processor, context);

        /// <summary>
        /// Executes this test by executing a specific query using the specified <paramref name="processor"/>.
        /// </summary>
        /// <param name="processor">The processor to execute the query with.</param>
        /// <param name="context">The context in which the test is running.</param> 
        protected abstract Task WhenAsync(IQueryProcessor<TRequest, TResponse> processor, MicroProcessorTestContext context);

        void IQueryTest<TRequest, TResponse>.Then(TRequest request, IQueryResult<TResponse> result, MicroProcessorTestContext context) =>
            Then(request, result, context);

        /// <summary>
        /// Verifies the <paramref name="result"/> of this test.
        /// </summary>
        /// <param name="request">Request that was executed by the query.</param>        
        /// <param name="result">The result of this test.</param>
        /// <param name="context">The context in which the test is running.</param>
        protected abstract void Then(TRequest request, IQueryResult<TResponse> result, MicroProcessorTestContext context);
    }
}
