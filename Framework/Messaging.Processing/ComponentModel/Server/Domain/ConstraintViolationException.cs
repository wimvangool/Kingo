using System.Runtime.Serialization;
using System.Security.Permissions;

namespace System.ComponentModel.Server.Domain
{
    /// <summary>
    /// This exception is thrown when a data-constraint is violated while inserting, updating or deleting
    /// an <see cref="IVersionedObject{T, K}" /> from a data store.
    /// </summary>
    [Serializable]
    public abstract class ConstraintViolationException : BusinessRuleViolationException
    {
        private const string _AggregateTypeKey = "_aggregateType";
        private readonly Type _aggregateType;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstraintViolationException" /> class.
        /// </summary>
        /// <param name="aggregateType">Type of the aggregate of which a constraint was violated.</param>
        internal ConstraintViolationException(Type aggregateType)
        {
            _aggregateType = aggregateType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstraintViolationException" /> class.
        /// </summary>
        /// <param name="aggregateType">Type of the aggregate of which a constraint was violated.</param>
        /// <param name="message">Message of the exception.</param>
        internal ConstraintViolationException(Type aggregateType, string message)
            : base(message)
        {
            _aggregateType = aggregateType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstraintViolationException" /> class.
        /// </summary>
        /// <param name="aggregateType">Type of the aggregate of which a constraint was violated.</param>
        /// <param name="message">Message of the exception.</param>
        /// <param name="innerException">Cause of this exception.</param>
        internal ConstraintViolationException(Type aggregateType, string message, Exception innerException)
            : base(message, innerException)
        {
            _aggregateType = aggregateType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstraintViolationException" /> class.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        internal ConstraintViolationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _aggregateType = (Type) info.GetValue(_AggregateTypeKey, typeof(Type));
        }

        /// <inheritdoc />
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue(_AggregateTypeKey, _aggregateType);
        }

        /// <summary>
        /// Type of the aggregate of which a constraint was violated.
        /// </summary>
        public Type AggregateType
        {
            get { return _aggregateType; }
        }               
    }
}
