using System;
using System.Collections.Generic;
using System.Text;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Serves as a base-class for all test's that execute a query and return the resulting response.
    /// </summary>
    /// <typeparam name="TResponse">Type of the response returned by the query.</typeparam>
    public abstract class ExecuteQueryTest<TResponse> : MicroProcessorTest<IQuery<TResponse>>, IExecuteQueryTest<TResponse>
    {
        void IExecuteQueryTest<TResponse>.Then(MicroProcessorTestContext context, IExecuteQueryResult<TResponse> result) =>
            Then(context, result);

        /// <summary>
        /// Verifies the <paramref name="result"/> of this test.
        /// </summary>
        /// <param name="context">The context in which the test is running.</param>
        /// <param name="result">The result of this test.</param>
        protected abstract void Then(MicroProcessorTestContext context, IExecuteQueryResult<TResponse> result);
    }
}
