using System;

namespace Kingo.MicroServices
{
    /// <summary>
    /// When implemented by a class, represents the result of a test.
    /// </summary>
    public interface IMicroProcessorTestResult
    {
        /// <summary>
        /// Asserts that while running the test, an exception of type <typeparamref name="TException"/> was thrown.
        /// </summary>        
        /// <param name="assertion">Optional delegate to assert the details of the exception.</param>
        void IsExpectedException<TException>(Action<TException> assertion = null) where TException : Exception;
    }
}
