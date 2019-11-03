using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Kingo.MicroServices
{
    /// <summary>
    /// This exception is thrown by a service when a technical failure prevented the processor from
    /// handling a message or executing a query correctly. This type semantically maps to HTTP response code <c>500</c>.
    /// </summary>
    [Serializable]
    public class InternalServerErrorException : MicroProcessorOperationException
    {                
        /// <summary>
        /// Initializes a new instance of the <see cref="InternalServerErrorException" /> class.
        /// </summary>        
        /// <param name="message">Message of the exception.</param>
        /// <param name="innerException">Cause of this exception.</param>        
        public InternalServerErrorException(string message = null, Exception innerException = null)
            : base(message, innerException) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InternalServerErrorException" /> class.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        protected InternalServerErrorException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        /// <summary>
        /// Returns a value between <c>500</c> and <c>599</c>.
        /// </summary>
        public override int ErrorCode =>
            500;

        /// <summary>
        /// Creates and returns a new <see cref="InternalServerErrorException" /> wrapping the specified <paramref name="innerException"/>.
        /// </summary>
        /// <param name="innerException">The exception to wrap.</param>
        /// <returns>A new <see cref="InternalServerErrorException" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="innerException"/> is <c>null</c>.
        /// </exception>
        public static InternalServerErrorException FromInnerException(Exception innerException) =>
            new InternalServerErrorException(NotNull(innerException).Message, innerException);

        private static Exception NotNull(Exception innerException) =>
            innerException ?? throw new ArgumentNullException(nameof(innerException));
    }
}
