using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Kingo.Messaging
{
    /// <summary>
    /// This exception is thrown when a certain required claim was not found.
    /// </summary>
    public class ClaimNotFoundException : InternalProcessorException
    {
        private const string _ClaimType = nameof(_ClaimType);

        /// <summary>
        /// Initializes a new instance of the <see cref="ClaimNotFoundException" /> class.
        /// </summary>
        /// <param name="claimType">The type of the claim that could not be found.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="claimType" /> is <c>null</c>.
        /// </exception>
        public ClaimNotFoundException(string claimType)
        {
            ClaimType = claimType ?? throw new ArgumentNullException(nameof(claimType));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClaimNotFoundException" /> class.
        /// </summary>
        /// <param name="claimType">The type of the claim that could not be found.</param>
        /// <param name="message">Message of the exception.</param>        
        /// <exception cref="ArgumentNullException">
        /// <paramref name="claimType" /> is <c>null</c>.
        /// </exception>
        public ClaimNotFoundException(string claimType, string message)
            : base(message)
        {
            ClaimType = claimType ?? throw new ArgumentNullException(nameof(claimType));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClaimNotFoundException" /> class.
        /// </summary>
        /// <param name="claimType">The type of the claim that could not be found.</param>
        /// <param name="message">Message of the exception.</param>
        /// <param name="innerException">Cause of this exception.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="claimType" /> is <c>null</c>.
        /// </exception>
        public ClaimNotFoundException(string claimType, string message, Exception innerException)
            : base(message, innerException)
        {
            ClaimType = claimType ?? throw new ArgumentNullException(nameof(claimType));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClaimNotFoundException" /> class.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        protected ClaimNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            ClaimType = info.GetString(_ClaimType);
        }

        /// <inheritdoc />
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue(_ClaimType, ClaimType);
        }

        /// <summary>
        /// The type of the claim that could not be found.
        /// </summary>
        public string ClaimType
        {
            get;
        }

        /// <summary>
        /// Creates and returns a <see cref="UnauthorizedRequestException"/> that is associated with the
        /// specified <paramref name="failedMessage"/>, indicating that the current exception occurred because of
        /// a bad client request.
        /// </summary>
        /// <param name="failedMessage">The message that was being handled the moment this exception was caught.</param>
        /// <param name="message">Message describing the context of the newly created message.</param>
        /// <returns>A new <see cref="BadRequestException"/> with its <see cref="Exception.InnerException"/> set to this instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="failedMessage"/> is <c>null</c>.
        /// </exception>
        public override BadRequestException AsBadRequestException(object failedMessage, string message) =>
            new UnauthorizedRequestException(failedMessage, message, this);
    }
}
