using System;

namespace Kingo.Messaging.Constraints
{
    /// <summary>
    /// This exception is thrown when an attempt to invoke an indexer on a instance failed
    /// because the indexer was not found or the specified index values were not valid.
    /// </summary>
    [Serializable]
    public sealed class IndexerInvocationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IndexerInvocationException" /> class.
        /// </summary>
        public IndexerInvocationException() {}

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexerInvocationException" /> class.
        /// </summary>
        /// <param name="message">Message of the exception.</param>
        public IndexerInvocationException(string message)
            : base(message) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexerInvocationException" /> class.
        /// </summary>
        /// <param name="message">Message of the exception.</param>
        /// <param name="innerException">Cause of this exception.</param>
        public IndexerInvocationException(string message, Exception innerException)
            : base(message, innerException) {}        
    }
}