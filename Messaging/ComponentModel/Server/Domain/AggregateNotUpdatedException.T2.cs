using System.Runtime.Serialization;
using System.Security.Permissions;

namespace System.ComponentModel.Server.Domain
{
    /// <summary>
    /// This type of exception is thrown when a <see cref="Repository{T, S, U}" /> was unable to update
    /// an <see cref="IAggregateRoot{T, S}" />.
    /// </summary>
    /// <typeparam name="TAggregate">Type of the aggregate.</typeparam>
    /// <typeparam name="TKey">Type of the key of the aggregate.</typeparam>
    [Serializable]
    public class AggregateNotUpdatedException<TAggregate, TKey> : RepositoryException<TAggregate>
        where TAggregate : class, IAggregateRoot<TKey>
        where TKey : struct, IEquatable<TKey>
    {
        private const string _AggregateKey = "_aggregate";
        private readonly IAggregateRoot<TKey> _aggregate;

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateNotUpdatedException{T, K}" /> class.
        /// </summary>
        /// <param name="aggregate">The aggregate that was not updated.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="aggregate"/> is <c>null</c>.
        /// </exception>
        public AggregateNotUpdatedException(IAggregateRoot<TKey> aggregate)
        {
            if (aggregate == null)
            {
                throw new ArgumentNullException("aggregate");
            }
            _aggregate = aggregate;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateNotUpdatedException{T, K}" /> class.
        /// </summary>
        /// <param name="aggregate">The aggregate that was not updated.</param>
        /// <param name="message">Message of the exception.</param>     
        /// <exception cref="ArgumentNullException">
        /// <paramref name="aggregate"/> is <c>null</c>.
        /// </exception>   
        public AggregateNotUpdatedException(IAggregateRoot<TKey> aggregate, string message)
            : base(message)
        {
            if (aggregate == null)
            {
                throw new ArgumentNullException("aggregate");
            }
            _aggregate = aggregate;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateNotUpdatedException{T, K}" /> class.
        /// </summary>
        /// <param name="aggregate">The aggregate that was not updated.</param>
        /// <param name="message">Message of the exception.</param>
        /// <param name="innerException">Cause of the exception.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="aggregate"/> is <c>null</c>.
        /// </exception>
        public AggregateNotUpdatedException(IAggregateRoot<TKey> aggregate, string message, Exception innerException)
            : base(message, innerException)
        {
            if (aggregate == null)
            {
                throw new ArgumentNullException("aggregate");
            }
            _aggregate = aggregate;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateNotUpdatedException{T, K}" /> class.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        protected AggregateNotUpdatedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _aggregate = (IAggregateRoot<TKey>) info.GetValue(_AggregateKey, typeof(IAggregateRoot<TKey>));
        }

        /// <inheritdoc />
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue(_AggregateKey, _aggregate);
        }

        /// <summary>
        /// The aggregate that was not updated.
        /// </summary>
        public IAggregateRoot<TKey> Aggregate
        {
            get { return _aggregate; }
        }
    }
}
