using System;
using System.Collections.Generic;
using System.Text;

namespace Kingo.MicroServices
{
    /// <summary>
    /// When implemented by a class, represents a test that executes a query with a <see cref="IMicroProcessor" />.
    /// </summary>
    /// <typeparam name="TRequest">Type of the request of the test.</typeparam>
    /// <typeparam name="TResponse">Type of the response of the test.</typeparam>
    public interface IExecuteQueryTest<in TRequest, TResponse> : IMicroProcessorTest<IQuery<TRequest, TResponse>>
    {
        /// <summary>
        /// Verifies the <paramref name="result"/> of this test.
        /// </summary>
        /// <param name="request">Request that was executed by the query.</param>
        /// <param name="context">The context in which the test is running.</param>
        /// <param name="result">The result of this test.</param>                
        void Then(TRequest request, MicroProcessorTestContext context, IExecuteQueryResult<TResponse> result);
    }
}
