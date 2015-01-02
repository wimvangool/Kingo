using System.Runtime.Serialization;

namespace System.ComponentModel
{
    /// <summary>
    /// This exception or any derived type is thrown if a request was found to be invalid
    /// when executing it.
    /// </summary>
    [Serializable]
    public class InvalidMessageException : FunctionalException
    {
        /// <summary>
        /// If specified, contains all the validation-errors of the <see cref="FunctionalException.FailedMessage" />.
        /// </summary>
        public readonly MessageErrorTree ErrorTree;

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidMessageException" /> class.
        /// </summary>
        /// <param name="failedMessage">The invalid request.</param>  
        /// <param name="message">Message of the exception.</param>     
        /// <exception cref="ArgumentNullException">
        /// <paramref name="failedMessage"/> is <c>null</c>.
        /// </exception> 
        public InvalidMessageException(object failedMessage, string message)
            : base(failedMessage, message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidMessageException" /> class.
        /// </summary>
        /// <param name="failedMessage">The invalid request.</param>  
        /// <param name="message">Message of the exception.</param>  
        /// <param name="inner">Cause of this exception.</param>    
        /// <exception cref="ArgumentNullException">
        /// <paramref name="failedMessage"/> is <c>null</c>.
        /// </exception> 
        public InvalidMessageException(object failedMessage, string message, Exception inner)
            : base(failedMessage, message, inner) { }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidMessageException" /> class.
        /// </summary>
        /// <param name="failedMessage">The invalid request.</param>  
        /// <param name="message">Message of the exception.</param> 
        /// <param name="errorTree">
        /// If specified, contains all the validation-errors of the <paramref name="failedMessage"/>.
        /// </param>
        public InvalidMessageException(object failedMessage, string message, MessageErrorTree errorTree)
            : base(failedMessage, message)
        {
            ErrorTree = errorTree;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidMessageException" /> class.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected InvalidMessageException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }                  
    }
}
