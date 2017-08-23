using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Kingo.Messaging
{
    /// <summary>
    /// This exception is thrown by a <see cref="IMicroProcessor" /> when a query failed to execute because the
    /// requested data or resource was not found. This type semantically maps to HTTP response code <c>404</c>.
    /// </summary>
    [Serializable]
    public class NotFoundException : BadRequestException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotFoundException" /> class.
        /// </summary>
        /// <param name="failedMessage">The message that could not be processed.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="failedMessage"/> is <c>null</c>.
        /// </exception>
        public NotFoundException(object failedMessage)
            : base(failedMessage) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotFoundException" /> class.
        /// </summary>
        /// <param name="failedMessage">The message that could not be processed.</param>
        /// <param name="message">Message of the exception.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="failedMessage"/> is <c>null</c>.
        /// </exception>
        public NotFoundException(object failedMessage, string message)
            : base(failedMessage, message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotFoundException" /> class.
        /// </summary>
        /// <param name="failedMessage">The message that could not be processed.</param>
        /// <param name="message">Message of the exception.</param>
        /// <param name="innerException">Cause of this exception.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="failedMessage"/> is <c>null</c>.
        /// </exception>
        public NotFoundException(object failedMessage, string message, Exception innerException)
            : base(failedMessage, message, innerException) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotFoundException" /> class.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected NotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
