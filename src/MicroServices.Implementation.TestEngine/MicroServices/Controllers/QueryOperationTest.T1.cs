using System.Threading.Tasks;

namespace Kingo.MicroServices.Controllers
{
    /// <summary>
    /// Serves as a base-class for all test's that execute a query and return the resulting response.
    /// </summary>
    /// <typeparam name="TResponse">Type of the response returned by the query.</typeparam>
    public abstract class QueryOperationTest<TResponse> : MicroProcessorOperationTest, IReadOperationTest<TResponse>
    {
        #region [====== WhenAsync ======]

        Task IReadOperationTest<TResponse>.WhenAsync(IQueryOperationRunner<TResponse> runner, MicroProcessorOperationTestContext context) =>
            WhenAsync(runner, context);

        /// <summary>
        /// Executes this test by executing a specific query using the specified <paramref name="runner"/>.
        /// </summary>
        /// <param name="runner">The runner that will execute the query.</param>
        /// <param name="context">The context in which the test is running.</param>    
        protected abstract Task WhenAsync(IQueryOperationRunner<TResponse> runner, MicroProcessorOperationTestContext context);

        #endregion

        #region [====== Then ======]

        void IReadOperationTest<TResponse>.Then(IQueryOperationTestResult<TResponse> result, MicroProcessorOperationTestContext context) =>
            Then(result, context);

        /// <summary>
        /// Verifies the <paramref name="result"/> of this test.
        /// </summary>
        /// <param name="context">The context in which the test is running.</param>
        /// <param name="result">The result of this test.</param>
        protected abstract void Then(IQueryOperationTestResult<TResponse> result, MicroProcessorOperationTestContext context);

        #endregion
    }
}
