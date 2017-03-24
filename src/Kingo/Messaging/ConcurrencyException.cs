using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Kingo.Messaging
{
    /// <summary>
    /// This exception is thrown by a <see cref="IUnitOfWork" /> when a concurrency conflict has occurred
    /// while flushing any changes.
    /// </summary>
    public class ConcurrencyException : InternalProcessorException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrencyException" /> class.
        /// </summary>
        public ConcurrencyException() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrencyException" /> class.
        /// </summary>
        /// <param name="message">Message of the exception.</param>
        public ConcurrencyException(string message)
            : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrencyException" /> class.
        /// </summary>
        /// <param name="message">Message of the exception.</param>
        /// <param name="inner">Cause of this exception.</param>
        public ConcurrencyException(string message, Exception inner)
            : base(message, inner) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrencyException" /> class.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        protected ConcurrencyException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        /// <inheritdoc />
        public override BadRequestException AsBadRequestException(object failedMessage, string message) =>
            new ConflictException(failedMessage, message, this);        
    }
}
