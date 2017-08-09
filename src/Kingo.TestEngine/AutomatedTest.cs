using System;

namespace Kingo
{
    /// <summary>
    /// When implemented by a class, represents a test or collection of tests that can be executed automatically by a test framework.
    /// </summary>
    public abstract class AutomatedTest
    {
        /// <summary>
        /// Creates and returns a new exception that indicates that an assertion has failed.
        /// </summary>
        /// <param name="message">Message of the exception.</param>
        /// <param name="innerException">Optional cause of the exception.</param>
        /// <returns>
        /// A new exception that indicates that an assertion has failed with the specified <paramref name="message"/>
        /// and <paramref name="innerException"/> as inner exception.
        /// </returns>
        protected abstract Exception NewAssertFailedException(string message, Exception innerException = null);
    }
}
