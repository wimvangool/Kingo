using System.Runtime.Serialization;

namespace System.ComponentModel.Messaging
{
    /// <summary>
    /// This exception or any derived type is thrown if a request has failed due to non-technical reasons.
    /// </summary>
    [Serializable]
    public class RequestExecutionException : Exception
    {
        /// <summary>
        /// If specified, refers to the message that was associated with the failed request.
        /// </summary>
        public readonly object RequestMessage;        

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestExecutionException" /> class.
        /// </summary>
        /// <param name="request">Refers to the message that was associated with the failed request.</param>      
        /// <exception cref="ArgumentNullException">
        /// <paramref name="request"/> is <c>null</c>.
        /// </exception> 
        public RequestExecutionException(object request)
        {            
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }
            RequestMessage = request;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestExecutionException" /> class.
        /// </summary>
        /// <param name="request">Refers to the message that was associated with the failed request.</param> 
        /// <param name="message">Message of the exception.</param>             
        /// <exception cref="ArgumentNullException">
        /// <paramref name="request"/> is <c>null</c>.
        /// </exception>                  
        public RequestExecutionException(object request, string message) : base(message)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }
            RequestMessage = request;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestExecutionException" /> class.
        /// </summary>        
        /// <param name="request">Refers to the message that was associated with the failed request.</param>
        /// <param name="message">Message of the exception.</param>
        /// <param name="inner">Cause of this exception.</param>        
        /// <exception cref="ArgumentNullException">
        /// <paramref name="request"/> is <c>null</c>.
        /// </exception>         
        public RequestExecutionException(object request, string message, Exception inner)
            : base(message, inner)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }
            RequestMessage = request;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestExecutionException" /> class.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected RequestExecutionException(SerializationInfo info, StreamingContext context) : base(info, context) {}        
    }
}
