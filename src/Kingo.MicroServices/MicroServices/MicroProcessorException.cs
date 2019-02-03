using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Kingo.MicroServices
{
    /// <summary>
    /// This exception is thrown by a <see cref="IMicroProcessor" /> when it failed to handle a message or execute a query.
    /// Any derived type of this class semantically maps to a <c>4xx</c> or <c>5xx</c> HTTP status code.
    /// </summary>
    [Serializable]
    public abstract class MicroProcessorException : Exception
    {                
        /// <summary>
        /// Initializes a new instance of the <see cref="MicroProcessorException" /> class.
        /// </summary>        
        /// <param name="message">Message of the exception.</param>
        /// <param name="innerException">Cause of this exception.</param>        
        internal MicroProcessorException(string message = null, Exception innerException = null)
            : base(message, innerException) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MicroProcessorException" /> class.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        internal MicroProcessorException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }          

        /// <summary>
        /// The ErrorCode that is associated with this Exception. This code typically corresponds with HttpStatusCodes,
        /// which implies the returned value is always somewhere between <c>400</c> and <c>599</c>, depending on the
        /// run-time type of the exception.
        /// </summary>
        public abstract int ErrorCode
        {
            get;            
        }
    }
}
