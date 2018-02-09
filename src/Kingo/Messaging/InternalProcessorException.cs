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
    public class InternalProcessorException : Exception
    {        
        /// <summary>
        /// Initializes a new instance of the <see cref="InternalProcessorException" /> class.
        /// </summary>
        /// <param name="message">Message of the exception.</param>
        /// <param name="innerException">Cause of this exception.</param>
        public InternalProcessorException(string message = null, Exception innerException = null)
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
        /// Creates and returns this exception as a <see cref="BadRequestException"/>, indicating that
        /// the current exception occurred because of a bad client request.
        /// </summary>        
        /// <param name="message">Message describing the context of the newly created message.</param>
        /// <returns>A new <see cref="BadRequestException"/> with its <see cref="Exception.InnerException"/> set to this instance.</returns>        
        public virtual BadRequestException AsBadRequestException(string message) =>
            new BadRequestException(message, this);

        /// <summary>
        /// Creates and returns this exception as a <see cref="InternalServerErrorException"/>, indicating that
        /// the current exception occurred because of an internal server error.        
        /// </summary>        
        /// <param name="message">Message describing the context of the newly created message.</param>
        /// <returns>A new <see cref="InternalServerErrorException"/> with its <see cref="Exception.InnerException"/> set to this instance.</returns>        
        public virtual InternalServerErrorException AsInternalServerErrorException(string message) =>
            new InternalServerErrorException(message, this);
    }
}
