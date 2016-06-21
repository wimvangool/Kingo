using System;

namespace Kingo
{
    /// <summary>
    /// When implemented by a class, represents the implementation of a test engine.
    /// </summary>
    public interface ITestEngine
    {
        #region [====== FailTest ======]                

        /// <summary>
        /// Marks the specified <paramref name="test"/> as failed by throwing a test-engine specific exception.
        /// </summary>
        /// <param name="test">The test that failed.</param>
        /// <param name="innerException">Optional Exception that was the root-cause of the failure.</param>   
        /// <exception cref="ArgumentNullException">
        /// <paramref name="test"/> is <c>null</c>.
        /// </exception>     
        /// <exception cref="Exception">
        /// Always (by design).
        /// </exception>
        void FailTest(Test test, Exception innerException = null);

        /// <summary>
        /// Creates and returns a new <see cref="Exception" /> that will be thrown to mark the failure of a test.
        /// </summary>
        /// <param name="errorMessage">The reason why the scenario failed.</param>
        /// <param name="innerException">Optional Exception that was the root-cause of the failure.</param>
        /// <returns>A new <see cref="Exception" />-instance with the specfied <paramref name="errorMessage"/>.</returns>
        Exception NewTestFailedException(string errorMessage, Exception innerException = null);

        #endregion
    }
}
