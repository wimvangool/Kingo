using System.Threading.Tasks;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Serves as a base-class for all test's that execute a query and return the resulting response.
    /// </summary>
    /// <typeparam name="TResponse">Type of the response returned by the query.</typeparam>
    public abstract class ExecuteQueryTest<TResponse> : MicroProcessorOperationTest, IExecuteQueryTest<TResponse>
    {
        Task IExecuteQueryTest<TResponse>.WhenAsync(IQueryProcessor<TResponse> processor, MicroProcessorOperationTestContext context) =>
            WhenAsync(processor, context);

        /// <summary>
        /// Executes this test by executing a specific query using the specified <paramref name="processor"/>.
        /// </summary>
        /// <param name="processor">The processor to execute the query with.</param>
        /// <param name="context">The context in which the test is running.</param>    
        protected abstract Task WhenAsync(IQueryProcessor<TResponse> processor, MicroProcessorOperationTestContext context);

        void IExecuteQueryTest<TResponse>.Then(IExecuteQueryResult<TResponse> result, MicroProcessorOperationTestContext context) =>
            Then(result, context);

        /// <summary>
        /// Verifies the <paramref name="result"/> of this test.
        /// </summary>
        /// <param name="context">The context in which the test is running.</param>
        /// <param name="result">The result of this test.</param>
        protected abstract void Then(IExecuteQueryResult<TResponse> result, MicroProcessorOperationTestContext context);
    }
}
