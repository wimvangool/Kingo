using System.ComponentModel.Messaging.Resources;
using System.Runtime.Serialization;

namespace System.ComponentModel.Messaging
{
    /// <summary>
    /// This exception or any derived type is thrown if a command was found to be invalid
    /// when executing it.
    /// </summary>
    [Serializable]
    public class InvalidRequestException : RequestExecutionException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidRequestException" /> class.
        /// </summary>
        /// <param name="request">The invalid command.</param>      
        /// <exception cref="ArgumentNullException">
        /// <paramref name="request"/> is <c>null</c>.
        /// </exception> 
        public InvalidRequestException(IDataErrorInfo request)
            : base(request, CreateMessage(request)) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidRequestException" /> class.
        /// </summary>
        /// <param name="request">The invalid command.</param>   
        /// <param name="inner">Cause of this exception.</param>    
        /// <exception cref="ArgumentNullException">
        /// <paramref name="request"/> is <c>null</c>.
        /// </exception> 
        public InvalidRequestException(IDataErrorInfo request, Exception inner)
            : base(request, CreateMessage(request), inner) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidRequestException" /> class.
        /// </summary>
        /// <param name="request">The invalid command.</param>
        /// <param name="message">Message of the exception.</param>        
        /// <exception cref="ArgumentNullException">
        /// <paramref name="request"/> is <c>null</c>.
        /// </exception> 
        public InvalidRequestException(object request, string message)
            : base(request, message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidRequestException" /> class.
        /// </summary>
        /// <param name="request">The invalid command.</param>
        /// <param name="message">Message of the exception.</param>
        /// <param name="inner">Cause of this exception.</param>       
        /// <exception cref="ArgumentNullException">
        /// <paramref name="request"/> is <c>null</c>.
        /// </exception> 
        public InvalidRequestException(object request, string message, Exception inner)
            : base(request, message, inner) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidRequestException" /> class.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected InvalidRequestException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }        

        private static string CreateMessage(IDataErrorInfo request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }                        
            return string.Format(ExceptionMessages.InvalidCommandException_MessageWithErrors, request.GetType().Name, Environment.NewLine + request.Error);
        }
    }
}
