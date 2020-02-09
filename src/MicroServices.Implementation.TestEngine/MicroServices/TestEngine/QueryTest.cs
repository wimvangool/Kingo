using System;

namespace Kingo.MicroServices.TestEngine
{
    /// <summary>
    /// Serves as a base-class for all tests that verify the behavior of a query.
    /// </summary>
    public abstract class QueryTest : MicroProcessorTest
    {
        /// <summary>
        /// Schedules a (void) request to be executed or handled by a <see cref="IQuery{TResponse}"/>,
        /// of which the behavior will be recorded and verified.
        /// </summary>
        /// <returns>
        /// The state that can be used to specify the type of response that is to be returned by the query.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// The test-engine is not in a state where it can perform this operation.
        /// </exception>
        protected IWhenRequestState WhenRequest() =>
            State.WhenRequest();

        /// <summary>
        /// Schedules a request to be executed or handled by a <see cref="IQuery{TRequest, TResponse}"/>,
        /// of which the behavior will be recorded and verified.
        /// </summary>
        /// <typeparam name="TRequest">Type of the request executed by the query.</typeparam>
        /// <returns>
        /// The state that can be used to specify the type of response that is to be returned by the query.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// The test-engine is not in a state where it can perform this operation.
        /// </exception>
        protected IWhenRequestState<TRequest> When<TRequest>() =>
            State.WhenRequest<TRequest>();
    }
}
