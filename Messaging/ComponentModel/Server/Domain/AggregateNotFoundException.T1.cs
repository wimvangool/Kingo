using System.Runtime.Serialization;

namespace System.ComponentModel.Server.Domain
{
    /// <summary>
    /// This type of exception is thrown when a <see cref="Repository{T, S, U}" /> was unable to retrieve,
    /// an <see cref="IAggregateRoot{T, S}" />.
    /// </summary>
    /// <typeparam name="TAggregate">Type of the aggregate.</typeparam>
    [Serializable]
    public class AggregateNotFoundException<TAggregate> : RepositoryException<TAggregate> where TAggregate : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateNotFoundException{T}" /> class.
        /// </summary>
        public AggregateNotFoundException() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateNotFoundException{T}" /> class.
        /// </summary>
        /// <param name="message">Message of the exception.</param>        
        public AggregateNotFoundException(string message)
            : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateNotFoundException{T}" /> class.
        /// </summary>
        /// <param name="message">Message of the exception.</param>
        /// <param name="innerException">Cause of the exception.</param>    
        public AggregateNotFoundException(string message, Exception innerException)
            : base(message, innerException) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateNotFoundException{T}" /> class.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected AggregateNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
