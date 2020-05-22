using System;

namespace Kingo.MicroServices
{
    /// <summary>
    /// When implemented by a class, represents the state in which a (void) request is scheduled to
    /// be executed by a <see cref="IQuery{TResponse}" />.
    /// </summary>
    public interface IWhenRequestState
    {
        /// <summary>
        /// Schedules the (void) request to be executed by a <see cref="IQuery{TResponse}"/> that
        /// returns a response of type <typeparamref name="TResponse"/>.
        /// </summary>
        /// <typeparam name="TResponse">Type of the response returned by the query.</typeparam>
        /// <returns>The state that can be used to execute the query.</returns>
        /// <exception cref="InvalidOperationException">
        /// The test-engine is not in a state where it can perform this operation.
        /// </exception>
        IWhenResponseState<TResponse> Returning<TResponse>();
    }
}
