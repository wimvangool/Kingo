using System;
using System.Threading.Tasks;

namespace Kingo.MicroServices.TestEngine
{
    /// <summary>
    /// When implemented by a class, represents a state in which the test-engine is able to run a specific test
    /// verifying the behavior of a certain <see cref="IQuery{TResponse}" /> operation.
    /// </summary>
    /// <typeparam name="TResponse">Type of the request executed by the query.</typeparam>
    public interface IReadyToRunQueryTestState<out TResponse>
    {
        /// <summary>
        /// Runs the test and expects the operation to throw an exception of type <typeparamref name="TException" />.
        /// </summary>
        /// <typeparam name="TException">Type of the expected exception.</typeparam>
        /// <param name="assertMethod">
        /// Optional delegate that will be used to assert the properties of the exception.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// The test-engine is not in a state where it can perform this operation.
        /// </exception>
        /// <exception cref="TestFailedException">
        /// The operation did not throw the expected exception or the specified <paramref name="assertMethod" />
        /// failed on asserting the properties of the exception.
        /// </exception>
        Task ThenOutputIs<TException>(Action<TException, MicroProcessorTestContext> assertMethod = null)
            where TException : MicroProcessorOperationException;

        /// <summary>
        /// Runs the test and expects the operation to publish and/or send a bunch of messages.
        /// </summary>
        /// <param name="assertMethod">
        /// Optional delegate that will be used to assert the messages that were published or sent and their
        /// properties.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// The test-engine is not in a state where it can perform this operation.
        /// </exception>
        /// <exception cref="TestFailedException">
        /// The operation threw an exception or the specified <paramref name="assertMethod" />
        /// failed on asserting the (properties of the) messages.
        /// </exception>
        Task ThenOutputIsResponse(Action<TResponse, MicroProcessorTestContext> assertMethod = null);
    }
}
