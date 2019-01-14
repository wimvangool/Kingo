using System;
using System.Collections.Generic;
using System.Text;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Serves as a base-class for all test's that execute a query and return the resulting response.
    /// </summary>
    /// <typeparam name="TRequest">Type of the request executed by the query.</typeparam>
    /// <typeparam name="TResponse">Type of the response returned by the query.</typeparam>
    public abstract class ExecuteQueryTest<TRequest, TResponse> : MicroProcessorTest<IQuery<TRequest, TResponse>>, IExecuteQueryTest<TRequest, TResponse>
    {
        void IExecuteQueryTest<TRequest, TResponse>.Then(TRequest request, MicroProcessorTestContext context, IExecuteQueryResult<TResponse> result) =>
            Then(request, context, result);

        /// <summary>
        /// Verifies the <paramref name="result"/> of this test.
        /// </summary>
        /// <param name="request">Request that was executed by the query.</param>
        /// <param name="context">The context in which the test is running.</param>
        /// <param name="result">The result of this test.</param>
        protected abstract void Then(TRequest request, MicroProcessorTestContext context, IExecuteQueryResult<TResponse> result);
    }
}
