namespace System.ComponentModel
{
    /// <summary>
    /// When implemented by a class, this type abstracts away the unit testing framework that is beging used.
    /// </summary>
    public interface IUnitTestFramework
    {
        /// <summary>
        /// Fails a test, usually by throwing an <see cref="Exception" />.
        /// </summary>        
        void FailTest();

        /// <summary>
        /// Fails a test, usually by throwing an <see cref="Exception" /> with a specific message.
        /// </summary>
        /// <param name="message">The reason why the test failed.</param>
        void FailTest(string message);

        /// <summary>
        /// Fails a test, usually by throwing an <see cref="Exception" /> with a specific message.
        /// </summary>
        /// <param name="message">The reason why the test failed.</param>
        /// <param name="parameters">Arguments that are to be embedded in the <paramref name="message"/>.</param>
        void FailTest(string message, params object[] parameters);
    }
}
