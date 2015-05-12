using System.Runtime.Serialization;
using System.Security.Permissions;

namespace System.ComponentModel.Server.Domain
{
    /// <summary>
    /// This type of exception is thrown when a <see cref="Repository{T, S, U}" /> was unable to retrieve,
    /// insert, update or delete an <see cref="IAggregateRoot{T, S}" />.
    /// </summary>
    [Serializable]
    public abstract class RepositoryException : DomainException
    {        
        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryException" /> class.
        /// </summary>                
        internal RepositoryException() { }       

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryException" /> class.
        /// </summary>        
        /// <param name="message">Message of the exception.</param>        
        internal RepositoryException(string message)
            : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryException" /> class.
        /// </summary>        
        /// <param name="message">Message of the exception.</param>
        /// <param name="innerException">Cause of the exception.</param>        
        internal RepositoryException(string message, Exception innerException)
            : base(message, innerException) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryException" /> class.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        internal RepositoryException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }        

        /// <summary>
        /// Returns the <see cref="Type" /> of the aggregate.
        /// </summary>
        public abstract Type AggregateType
        {
            get;
        }        
    }
}
