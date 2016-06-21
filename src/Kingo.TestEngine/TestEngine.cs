using System;
using Kingo.Resources;

namespace Kingo
{
    /// <summary>
    /// Provides a base-class implementation of the <see cref="ITestEngine" /> interface.
    /// </summary>
    public abstract class TestEngine : ITestEngine
    {        
        /// <inheritdoc />
        public void FailTest(Test test, Exception innerException = null)
        {
            throw NewTestFailedException(CreateErrorMessage(test), innerException);
        }

        /// <summary>
        /// Creates and returns an error message for a failing test based on the <paramref name="test"/> and its parameters.
        /// </summary>
        /// <param name="test">The test that failed.</param>
        /// <returns>An error messages that describes which test failed.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="test"/> is <c>null</c>.
        /// </exception>
        protected virtual string CreateErrorMessage(Test test)
        {
            if (test == null)
            {
                throw new ArgumentNullException(nameof(test));
            }
            return string.Format(ExceptionMessages.TestEngine_TestFailed, test);
        }

        /// <inheritdoc />
        public abstract Exception NewTestFailedException(string errorMessage, Exception innerException = null);
    }
}
