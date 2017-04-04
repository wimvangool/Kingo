using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Kingo.Messaging
{
    /// <summary>
    /// An exception of this type is to be thrown by application code when handling a message or executing a query.
    /// The <see cref="IMicroProcessor" /> will catch exceptions of this type and convert it to a <see cref="BadRequestException" />
    /// or <see cref="InternalServerErrorException" /> based on whether or was executing a request or handling an event.   
    /// </summary>
    [Serializable]
    public abstract class InternalProcessorException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InternalProcessorException" /> class.
        /// </summary>
        protected InternalProcessorException() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InternalProcessorException" /> class.
        /// </summary>
        /// <param name="message">Message of the exception.</param>
        protected InternalProcessorException(string message)
            : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InternalProcessorException" /> class.
        /// </summary>
        /// <param name="message">Message of the exception.</param>
        /// <param name="innerException">Cause of this exception.</param>
        protected InternalProcessorException(string message, Exception innerException)
            : base(message, innerException) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InternalProcessorException" /> class.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        protected InternalProcessorException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        /// <summary>
        /// Creates and returns this exception as a <see cref="BadRequestException"/> that is associated with the
        /// specified <paramref name="failedMessage"/>, indicating that the current exception occurred because of
        /// a bad client request.
        /// </summary>
        /// <param name="failedMessage">The message that was being handled the moment this exception was caught.</param>
        /// <param name="message">Message describing the context of the newly created message.</param>
        /// <returns>A new <see cref="BadRequestException"/> with its <see cref="Exception.InnerException"/> set to this instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="failedMessage"/> is <c>null</c>.
        /// </exception>
        public virtual BadRequestException AsBadRequestException(object failedMessage, string message) =>
            new BadRequestException(failedMessage, message, this);

        /// <summary>
        /// Creates and returns this exception as a <see cref="InternalServerErrorException"/> that is associated with the
        /// specified <paramref name="failedMessage"/>, indicating that the current exception occurred because of an internal
        /// server error.        
        /// </summary>
        /// <param name="failedMessage">The message that was being handled the moment this exception was caught.</param>
        /// <param name="message">Message describing the context of the newly created message.</param>
        /// <returns>A new <see cref="InternalServerErrorException"/> with its <see cref="Exception.InnerException"/> set to this instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="failedMessage"/> is <c>null</c>.
        /// </exception>
        public virtual InternalServerErrorException AsInternalServerErrorException(object failedMessage, string message) =>
            new InternalServerErrorException(failedMessage, Message, this);
    }
}
