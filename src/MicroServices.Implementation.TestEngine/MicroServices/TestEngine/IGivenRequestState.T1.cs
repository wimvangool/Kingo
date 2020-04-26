using System;

namespace Kingo.MicroServices.TestEngine
{
    /// <summary>
    /// When implemented by a class, represents the state in which a request is scheduled
    /// to be executed by an <see cref="IQuery{TRequest, TResponse}" />.
    /// </summary>
    public interface IGivenRequestState<TRequest>
    {
        /// <summary>
        /// Schedules the (void) request to be executed by a <see cref="IQuery{TRequest, TResponse}"/> that
        /// returns a response of type <typeparamref name="TResponse"/>.
        /// </summary>
        /// <typeparam name="TResponse">Type of the response returned by the query.</typeparam>
        /// <returns>The state that can be used to execute the query.</returns>
        /// <exception cref="InvalidOperationException">
        /// The test-engine is not in a state where it can perform this operation.
        /// </exception>
        IGivenResponseState<TRequest, TResponse> Returning<TResponse>();
    }
}
