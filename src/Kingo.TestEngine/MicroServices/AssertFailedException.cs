using System;
using System.Runtime.Serialization;

namespace Kingo.MicroServices
{
    /// <summary>
    /// This exception is thrown by the test-engine when an assertion fails.
    /// </summary>
    [Serializable]
    public sealed class MicroProcessorTestFailedException : Exception
    {        
        /// <summary>
        /// Initializes a new instance of the <see cref="MicroProcessorTestFailedException" /> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="innerException">The cause of the exception.</param>
        public MicroProcessorTestFailedException(string message, Exception innerException = null)
            : base(message, innerException) { }

        private MicroProcessorTestFailedException(SerializationInfo info, StreamingContext context) :
            base(info, context) { }
    }
}
