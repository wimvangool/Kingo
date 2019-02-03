using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Kingo.MicroServices
{
    /// <summary>
    /// This exception is thrown when a certain required claim was not found.
    /// </summary>
    public class ClaimNotFoundException : MessageHandlerException
    {                
        /// <summary>
        /// Initializes a new instance of the <see cref="ClaimNotFoundException" /> class.
        /// </summary>
        /// <param name="claimType">The type of the claim that could not be found.</param>
        /// <param name="message">Message of the exception.</param>
        /// <param name="innerException">Cause of this exception.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="claimType" /> is <c>null</c>.
        /// </exception>
        public ClaimNotFoundException(string claimType, string message = null, Exception innerException = null)
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
            ClaimType = info.GetString(nameof(ClaimType));
        }

        /// <inheritdoc />
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue(nameof(ClaimType), ClaimType);
        }

        /// <summary>
        /// The type of the claim that could not be found.
        /// </summary>
        public string ClaimType
        {
            get;
        }

        /// <summary>
        /// Creates and returns a <see cref="UnauthorizedRequestException"/>, indicating that the current
        /// exception occurred because of a unauthorized client request.
        /// </summary>        
        /// <param name="message">Message describing the context of the newly created message.</param>
        /// <returns>A new <see cref="BadRequestException"/> with its <see cref="Exception.InnerException"/> set to this instance.</returns>        
        public override BadRequestException AsBadRequestException(string message) =>
            new UnauthorizedRequestException(message, this);
    }
}
