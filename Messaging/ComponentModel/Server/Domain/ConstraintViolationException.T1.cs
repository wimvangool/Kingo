using System.Runtime.Serialization;
using System.Security.Permissions;

namespace System.ComponentModel.Server.Domain
{
    /// <summary>
    /// This exception is thrown when a data-constraint is violated while inserting, updating or deleting
    /// an <see cref="IVersionedObject{T, K}" /> from a data store.
    /// </summary>
    [Serializable]
    public abstract class ConstraintViolationException<TAggregate> : ConstraintViolationException where TAggregate : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConstraintViolationException{T}" /> class.
        /// </summary>		
        protected ConstraintViolationException()
            : base(typeof(TAggregate)) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstraintViolationException{T}" /> class.
        /// </summary>		
        /// <param name="message">Message of the exception.</param>
        protected ConstraintViolationException(string message)
            : base(typeof(TAggregate), message) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstraintViolationException{T}" /> class.
        /// </summary>		
        /// <param name="message">Message of the exception.</param>
        /// <param name="innerException">Cause of this exception.</param>
        protected ConstraintViolationException(string message, Exception innerException)
            : base(typeof(TAggregate), message, innerException) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstraintViolationException{T}" /> class.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        protected ConstraintViolationException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }        
    }
}