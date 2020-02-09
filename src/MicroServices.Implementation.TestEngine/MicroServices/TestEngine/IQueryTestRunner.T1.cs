using System;
using System.Threading.Tasks;

namespace Kingo.MicroServices.TestEngine
{
    /// <summary>
    /// When implemented by a class, represents a test-runner that is able to run a specific test verifying
    /// the behavior of a certain <see cref="IQuery{TResponse}" /> operation.
    /// </summary>
    /// <typeparam name="TResponse">Type of the response returned by the query.</typeparam>
    public interface IQueryTestRunner<out TResponse>
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
            where TException : Exception;

        /// <summary>
        /// Runs the test and expects the operation to return a specific response.
        /// </summary>
        /// <param name="assertMethod">
        /// Optional delegate that will be used to assert the properties of the response.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// The test-engine is not in a state where it can perform this operation.
        /// </exception>
        /// <exception cref="TestFailedException">
        /// The operation threw an exception or the specified <paramref name="assertMethod" />
        /// failed on asserting the (properties of the) response.
        /// </exception>
        Task ThenOutputIsResponse(Action<TResponse, MicroProcessorTestContext> assertMethod = null);
    }
}
