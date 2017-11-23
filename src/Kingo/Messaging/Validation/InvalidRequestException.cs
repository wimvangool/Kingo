using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Kingo.Messaging.Validation
{
    /// <summary>
    /// This exception or any derived type is thrown if a request was found to be invalid when executing it.
    /// </summary>
    [Serializable]
    public class InvalidRequestException : InternalProcessorException
    {            
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidRequestException" /> class.
        /// </summary>
        /// <param name="errors">The invalid request.</param>  
        /// <param name="message">Message of the exception.</param>     
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errors"/> is <c>null</c>.
        /// </exception> 
        public InvalidRequestException(ErrorInfo errors, string message) :
            base(message)
        {            
            Errors = errors ?? throw new ArgumentNullException(nameof(errors));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidRequestException" /> class.
        /// </summary>
        /// <param name="errors">The invalid request.</param>  
        /// <param name="message">Message of the exception.</param>  
        /// <param name="innerException">Cause of this exception.</param>    
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errors"/> is <c>null</c>.
        /// </exception> 
        public InvalidRequestException(ErrorInfo errors, string message, Exception innerException) :
            base(message, innerException)
        {            
            Errors = errors ?? throw new ArgumentNullException(nameof(errors));
        }               

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidRequestException" /> class.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        protected InvalidRequestException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Errors = (ErrorInfo) info.GetValue(nameof(Errors), typeof(ErrorInfo));
        }

        /// <inheritdoc />
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue(nameof(Errors), Errors);
        }

        /// <summary>
        /// Contains all validation errors.
        /// </summary>
        public ErrorInfo Errors
        {
            get;
        }
    }
}
