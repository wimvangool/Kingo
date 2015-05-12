using System.Runtime.Serialization;

namespace System.ComponentModel.Server.Domain
{
    /// <summary>
    /// This type of exception is thrown when a <see cref="Repository{T, S, U}" /> was unable to retrieve,
    /// insert, update or delete an <see cref="IAggregateRoot{T, S}" />.
    /// </summary>
    /// <typeparam name="TAggregate">Type of the aggregate.</typeparam>
    [Serializable]
    public abstract class RepositoryException<TAggregate> : RepositoryException where TAggregate : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryException" /> class.
        /// </summary>                
        protected RepositoryException() { }            

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryException" /> class.
        /// </summary>        
        /// <param name="message">Message of the exception.</param>        
        protected RepositoryException(string message)
            : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryException" /> class.
        /// </summary>        
        /// <param name="message">Message of the exception.</param>
        /// <param name="innerException">Cause of the exception.</param>        
        protected RepositoryException(string message, Exception innerException)
            : base(message, innerException) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryException" /> class.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected RepositoryException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        /// <inheritdoc />
        public override Type AggregateType
        {
            get { return typeof(TAggregate); }
        }
    }
}
