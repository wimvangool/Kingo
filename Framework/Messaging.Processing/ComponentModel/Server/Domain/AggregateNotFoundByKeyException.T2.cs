using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Syztem.ComponentModel.Server.Domain
{
    /// <summary>
    /// This type of exception is thrown when a <see cref="Repository{T, S, U}" /> was unable to retrieve,
    /// an <see cref="IVersionedObject{T, S}" /> by its key.
    /// </summary>
    /// <typeparam name="TAggregate">Type of the aggregate.</typeparam>
    /// <typeparam name="TKey">Type of the key of the aggregate.</typeparam>
    [Serializable]
    public class AggregateNotFoundByKeyException<TAggregate, TKey> : AggregateNotFoundException<TAggregate>
        where TAggregate : class, IKeyedObject<TKey>
        where TKey : struct, IEquatable<TKey>
    {
        private const string _AggregateKeyKey = "_aggregateKey";
        private readonly TKey _aggregateKey;

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateNotFoundByKeyException{T, K}" /> class.
        /// </summary>
        /// <param name="aggregateKey">Key of the aggregate that was not found.</param>
        public AggregateNotFoundByKeyException(TKey aggregateKey)
        {
            _aggregateKey = aggregateKey;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateNotFoundByKeyException{T, K}" /> class.
        /// </summary>
        /// <param name="aggregateKey">Key of the aggregate that was not found.</param>
        /// <param name="message">Message of the exception.</param>        
        public AggregateNotFoundByKeyException(TKey aggregateKey, string message)
            : base(message)
        {
            _aggregateKey = aggregateKey;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateNotFoundByKeyException{T, K}" /> class.
        /// </summary>
        /// <param name="aggregateKey">Key of the aggregate that was not found.</param>
        /// <param name="message">Message of the exception.</param>
        /// <param name="innerException">Cause of the exception.</param>
        public AggregateNotFoundByKeyException(TKey aggregateKey, string message, Exception innerException)
            : base(message, innerException)
        {
            _aggregateKey = aggregateKey;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateNotFoundByKeyException{T, K}" /> class.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        protected AggregateNotFoundByKeyException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _aggregateKey = (TKey) info.GetValue(_AggregateKeyKey, typeof(TKey));
        }

        /// <inheritdoc />
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue(_AggregateKeyKey, _aggregateKey);
        }

        /// <summary>
        /// Key of the aggregate that was not found.
        /// </summary>
        public TKey AggregateKey
        {
            get { return _aggregateKey; }
        }
    }
}
