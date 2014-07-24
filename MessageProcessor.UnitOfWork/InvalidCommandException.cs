using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using YellowFlare.MessageProcessing.Resources;

namespace YellowFlare.MessageProcessing
{
    /// <summary>
    /// This exception or any derived type is thrown if a command was found to be invalid
    /// when executing it.
    /// </summary>
    [Serializable]
    public class InvalidCommandException : RequestExecutionException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidCommandException" /> class.
        /// </summary>
        /// <param name="request">The invalid command.</param>      
        /// <exception cref="ArgumentNullException">
        /// <paramref name="request"/> is <c>null</c>.
        /// </exception> 
        public InvalidCommandException(object request)
            : base(request, CreateMessage(request)) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidCommandException" /> class.
        /// </summary>
        /// <param name="request">The invalid command.</param>   
        /// <param name="inner">Cause of this exception.</param>    
        /// <exception cref="ArgumentNullException">
        /// <paramref name="request"/> is <c>null</c>.
        /// </exception> 
        public InvalidCommandException(object request, Exception inner)
            : base(request, CreateMessage(request), inner) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidCommandException" /> class.
        /// </summary>
        /// <param name="request">The invalid command.</param>
        /// <param name="message">Message of the exception.</param>        
        /// <exception cref="ArgumentNullException">
        /// <paramref name="request"/> is <c>null</c>.
        /// </exception> 
        public InvalidCommandException(object request, string message)
            : base(request, message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidCommandException" /> class.
        /// </summary>
        /// <param name="request">The invalid command.</param>
        /// <param name="message">Message of the exception.</param>
        /// <param name="inner">Cause of this exception.</param>       
        /// <exception cref="ArgumentNullException">
        /// <paramref name="request"/> is <c>null</c>.
        /// </exception> 
        public InvalidCommandException(object request, string message, Exception inner)
            : base(request, message, inner) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidCommandException" /> class.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected InvalidCommandException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }        

        private static string CreateMessage(object request)
        {
            if (request == null)
            {
                return null;
            }
            var requestType = request.GetType().Name;

            var errorInfo = request as IDataErrorInfo;
            if (errorInfo == null)
            {
                return string.Format(ExceptionMessages.InvalidCommandException_Message, requestType);    
            }
            return string.Format(ExceptionMessages.InvalidCommandException_MessageWithErrors, requestType, Environment.NewLine + errorInfo.Error);
        }
    }
}
