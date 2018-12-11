using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Kingo.MicroServices.Domain
{
    /// <summary>
    /// This exception is thrown when an attempt has been made to add an aggregate to a <see cref="IRepository{T, S}" />
    /// which already contains an aggregate with the same id.
    /// </summary>
    [Serializable]
    public class DuplicateKeyException : MessageHandlerException
    {        
        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateKeyException" /> class.
        /// </summary>
        /// <param name="aggregateId">Identifier of the aggregate.</param>
        /// <param name="message">Message of the exception.</param>
        /// <param name="innerException">Cause of this exception.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="aggregateId"/> is <c>null</c>.
        /// </exception>
        public DuplicateKeyException(object aggregateId, string message = null, Exception innerException = null) :
            base(message, innerException)
        {            
            AggregateId = aggregateId ?? throw new ArgumentNullException(nameof(aggregateId));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateKeyException" /> class.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        protected DuplicateKeyException(SerializationInfo info, StreamingContext context) :
            base(info, context)
        {
            AggregateId = info.GetValue(nameof(AggregateId), typeof(object));
        }

        /// <inheritdoc />
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue(nameof(AggregateId), AggregateId);
        }

        /// <summary>
        /// Identifier of the aggregate.
        /// </summary>
        public object AggregateId
        {
            get;
        }
    }
}
