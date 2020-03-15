using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Kingo.MicroServices
{
    /// <summary>
    /// This exception is thrown by a service when it failed to handle a message or execute a query.
    /// Any derived type of this class semantically maps to a <c>4xx</c> or <c>5xx</c> HTTP status code.
    /// </summary>
    [Serializable]
    public abstract class MicroProcessorOperationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MicroProcessorOperationException" /> class.
        /// </summary>
        /// <param name="message">Message of the exception.</param>
        /// <param name="innerException">Cause of this exception.</param>
        /// <param name="operationStackTrace">The stack trace of the processor at the time the exception was thrown.</param>
        internal MicroProcessorOperationException(string message = null, Exception innerException = null, MicroProcessorOperationStackTrace operationStackTrace = null) :
            base(message, innerException)
        {
            OperationStackTrace = operationStackTrace ?? MicroProcessorOperationStackTrace.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MicroProcessorOperationException" /> class.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        internal MicroProcessorOperationException(SerializationInfo info, StreamingContext context) :
            base(info, context)
        {
            OperationStackTrace = (MicroProcessorOperationStackTrace)info.GetValue(nameof(OperationStackTrace), typeof(MicroProcessorOperationStackTrace));
        }

        /// <inheritdoc />
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue(nameof(OperationStackTrace), OperationStackTrace);
        }

        /// <summary>
        /// The stack trace of the processor at the time the exception was thrown.
        /// </summary>
        public MicroProcessorOperationStackTrace OperationStackTrace
        {
            get;
        }

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
