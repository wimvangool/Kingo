using System;
using System.Runtime.Serialization;

namespace YellowFlare.MessageProcessing
{
    /// <summary>
    /// This exception or any derived type is thrown if a request is considered invalid, could
    /// not be executed because of insufficient rights or caused a certain business rule violation.
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
        public RequestExecutionException() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestExecutionException" /> class.
        /// </summary>
        /// <param name="requestMessage">
        /// If specified, refers to the message that was associated with the failed request.
        /// </param>       
        public RequestExecutionException(object requestMessage)
        {            
            RequestMessage = requestMessage;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestExecutionException" /> class.
        /// </summary>
        /// <param name="message">Message of the exception.</param>   
        /// <param name="requestMessage">
        /// If specified, refers to the message that was associated with the failed request.
        /// </param>                    
        public RequestExecutionException(string message, object requestMessage) : base(message)
        {            
            RequestMessage = requestMessage;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestExecutionException" /> class.
        /// </summary>        
        /// <param name="message">Message of the exception.</param>
        /// <param name="inner">Cause of this exception.</param>
        /// <param name="requestMessage">
        /// If specified, refers to the message that was associated with the failed request.
        /// </param>        
        public RequestExecutionException(string message, Exception inner, object requestMessage)
            : base(message, inner)
        {            
            RequestMessage = requestMessage;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestExecutionException" /> class.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected RequestExecutionException(SerializationInfo info, StreamingContext context) : base(info, context) {}        
    }
}
