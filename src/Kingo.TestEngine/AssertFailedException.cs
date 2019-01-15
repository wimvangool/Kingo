using System;
using System.Runtime.Serialization;

namespace Kingo
{
    /// <summary>
    /// This exception is thrown by the test-engine when an assertion fails.
    /// </summary>
    [Serializable]
    public sealed class AssertFailedException : Exception
    {        
        /// <summary>
        /// Initializes a new instance of the <see cref="AssertFailedException" /> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="innerException">The cause of the exception.</param>
        public AssertFailedException(string message, Exception innerException = null)
            : base(message, innerException) { }

        private AssertFailedException(SerializationInfo info, StreamingContext context) :
            base(info, context) { }
    }
}
