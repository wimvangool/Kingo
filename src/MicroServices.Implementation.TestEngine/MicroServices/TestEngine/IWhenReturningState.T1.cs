using System;

namespace Kingo.MicroServices.TestEngine
{
    /// <summary>
    /// Represents the state where a (void) request has been scheduled to be executed by a <see cref="IQuery{TResponse}"/>
    /// returning a response of type <typeparamref name="TResponse" />.
    /// </summary>
    /// <typeparam name="TResponse">Type of the response returned by the query.</typeparam>
    public interface IWhenReturningState<TResponse>
    {
        /// <summary>
        /// Schedules the (void) request by a query or type <typeparamref name="TQuery" />.
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
        IReadyToRunQueryTestState<TResponse> IsExecutedBy<TQuery>(Action<QueryTestOperationInfo, MicroProcessorTestContext> configurator)
            where TQuery : class, IQuery<TResponse>;

        /// <summary>
        /// Schedules the (void) request to be executed by the specified <paramref name="query"/>.
        /// </summary>
        /// <param name="configurator">Delegate that will be used to configure the operation.</param>
        /// <param name="query">Query that will execute the (void) request.</param>
        /// <returns>The state that can be used to run the test and verify its output.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="configurator"/> or <paramref name="query" /> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The test-engine is not in a state where it can perform this operation.
        /// </exception>
        IReadyToRunQueryTestState<TResponse> IsExecutedByQuery(Action<QueryTestOperationInfo, MicroProcessorTestContext> configurator, IQuery<TResponse> query);
    }
}
