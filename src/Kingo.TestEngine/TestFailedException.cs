using System;
using System.Runtime.Serialization;

namespace Kingo
{
    /// <summary>
    /// This exception is thrown by the test-engine when a test has failed.
    /// </summary>
    [Serializable]
    public sealed class TestFailedException : Exception
    {        
        /// <summary>
        /// Initializes a new instance of the <see cref="TestFailedException" /> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="innerException">The cause of the exception.</param>
        public TestFailedException(string message, Exception innerException = null)
            : base(message, innerException) { }

        private TestFailedException(SerializationInfo info, StreamingContext context) :
            base(info, context) { }
    }
}
