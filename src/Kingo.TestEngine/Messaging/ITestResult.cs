using System;
using System.Threading.Tasks;

namespace Kingo.Messaging
{
    /// <summary>
    /// when implemented by a class, represents a result that is expected from execution of a specific <see cref="ScenarioTest" />.
    /// </summary>
    public interface ITestResult
    {
        /// <summary>
        /// Asserts that the specified <typeparamref name="TException"/> is thrown when executing the scenario.
        /// </summary>
        /// <typeparam name="TException">Type of the expected exception.</typeparam>
        /// <param name="assertCallback">
        /// An optional delegate that can be used to assert the properties of the exception.
        /// </param>
        /// <returns>A task that represents the execution of the associated scenario.</returns>
        Task IsExceptionOfTypeAsync<TException>(Action<TException> assertCallback = null) where TException : ExternalProcessorException;
    }
}
