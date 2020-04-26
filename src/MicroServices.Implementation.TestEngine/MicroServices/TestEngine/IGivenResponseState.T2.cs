using System;

namespace Kingo.MicroServices.TestEngine
{
    /// <summary>
    /// Represents the state where a request has been scheduled to be executed by a <see cref="IQuery{TRequest, TResponse}"/>
    /// returning a response of type <typeparamref name="TResponse" />.
    /// </summary>
    /// <typeparam name="TRequest">Type of the request executed by the query.</typeparam>
    /// <typeparam name="TResponse">Type of the response returned by the query.</typeparam>
    public interface IGivenResponseState<TRequest, TResponse>
    {
        /// <summary>
        /// Schedules the request to be executed by a query of type <typeparamref name="TQuery" />.
        /// </summary>
        /// <typeparam name="TQuery">The query that will execute the request.</typeparam>
        /// <param name="request">Request to execute by the query.</param>
        /// <returns>The state that can be used to run the test and verify its output.</returns>
        /// <exception cref="InvalidOperationException">
        /// The test-engine is not in a state where it can perform this operation.
        /// </exception>
        void IsExecutedBy<TQuery>(TRequest request)
            where TQuery : class, IQuery<TRequest, TResponse>;

        /// <summary>
        /// Schedules the request to be executed by a query of type <typeparamref name="TQuery" />.
        /// </summary>
        /// <typeparam name="TQuery">The query that will execute the request.</typeparam>
        /// <param name="configurator">Delegate that will be used to configure the operation.</param>
        /// <returns>The state that can be used to run the test and verify its output.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="configurator"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The test-engine is not in a state where it can perform this operation.
        /// </exception>
        void IsExecutedBy<TQuery>(Action<QueryTestOperationInfo<TRequest>, MicroProcessorTestContext> configurator)
            where TQuery : class, IQuery<TRequest, TResponse>;

        /// <summary>
        /// Schedules the request to be executed by the specified <paramref name="query"/>.
        /// </summary>
        /// <param name="query">Query that will execute the (void) request.</param>
        /// <param name="configurator">Delegate that will be used to configure the operation.</param>
        /// <returns>The state that can be used to run the test and verify its output.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="query" /> or <paramref name="configurator"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The test-engine is not in a state where it can perform this operation.
        /// </exception>
        void IsExecutedBy(IQuery<TRequest, TResponse> query, Action<QueryTestOperationInfo<TRequest>, MicroProcessorTestContext> configurator);
    }
}
