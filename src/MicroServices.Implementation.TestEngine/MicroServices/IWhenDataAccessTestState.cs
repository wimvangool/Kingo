using System;

namespace Kingo.MicroServices
{
    /// <summary>
    /// When implemented by a class, represents a state in which the test-engine prepares
    /// to execute a request.
    /// </summary>
    public interface IWhenDataAccessTestState : IWhenBusinessLogicTestState
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
        IWhenRequestState Request();

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
        IWhenRequestState<TRequest> Request<TRequest>();
    }
}
