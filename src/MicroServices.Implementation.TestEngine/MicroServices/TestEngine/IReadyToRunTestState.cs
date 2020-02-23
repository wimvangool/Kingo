using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kingo.MicroServices.TestEngine
{
    /// <summary>
    /// When implemented by a class, represents a state in which the test-engine is ready to run the test
    /// and verify its output.
    /// </summary>
    public interface IReadyToRunTestState
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
    }
}
