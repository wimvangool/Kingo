using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Kingo.BuildingBlocks.Messaging
{
    /// <summary>
    /// This exception or any derived type is thrown if a request was found to be invalid
    /// when executing it.
    /// </summary>
    [Serializable]
    public class InvalidMessageException : FunctionalException
    {
        private const string _ErrorTreeKey = "_errorInfo";
        private readonly ErrorInfo _errorInfo;

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
        /// <param name="innerException">Cause of this exception.</param>    
        /// <exception cref="ArgumentNullException">
        /// <paramref name="failedMessage"/> is <c>null</c>.
        /// </exception> 
        public InvalidMessageException(object failedMessage, string message, Exception innerException)
            : base(failedMessage, message, innerException) { }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidMessageException" /> class.
        /// </summary>
        /// <param name="failedMessage">The invalid request.</param>  
        /// <param name="message">Message of the exception.</param> 
        /// <param name="errorInfo">
        /// If specified, contains all the validation-errors of the <paramref name="failedMessage"/>.
        /// </param>
        public InvalidMessageException(object failedMessage, string message, ErrorInfo errorInfo)
            : base(failedMessage, message)
        {
            _errorInfo = errorInfo;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidMessageException" /> class.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        protected InvalidMessageException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _errorInfo = (ErrorInfo) info.GetValue(_ErrorTreeKey, typeof(ErrorInfo));
        }

        /// <inheritdoc />
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue(_ErrorTreeKey, _errorInfo);
        }

        /// <summary>
        /// If specified, contains all the validation-errors of the <see cref="FunctionalException.FailedMessage" />.
        /// </summary>
        public ErrorInfo ErrorInfo
        {
            get { return _errorInfo; }
        }
    }
}
