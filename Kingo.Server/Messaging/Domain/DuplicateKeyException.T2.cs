using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Kingo.Messaging.Domain
{
    /// <summary>
    /// This exception is thrown when ....
    /// </summary>
    [Serializable]
    public sealed class DuplicateKeyException<TKey> : DomainException      
        where TKey : struct, IEquatable<TKey>
    {
        private const string _KeyKey = "_key";
        private readonly TKey _key;        

        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateKeyException{T}" /> class.
        /// </summary>		
        /// <param name="key">The key that was already assigned to another aggregate.</param>
        /// <param name="message">Message of the exception.</param>
        public DuplicateKeyException(TKey key, string message)
            : base(message)
        {
            _key = key;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateKeyException{T}" /> class.
        /// </summary>		
        /// <param name="key">The key that was already assigned to another aggregate.</param>
        /// <param name="message">Message of the exception.</param>
        /// <param name="innerException">Cause of this exception.</param>
        public DuplicateKeyException(TKey key, string message, Exception innerException)
            : base(message, innerException)
        {
            _key = key;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateKeyException{T}" /> class.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        private DuplicateKeyException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _key = (TKey) info.GetValue(_KeyKey, typeof(TKey));
        }

        /// <inheritdoc />
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue(_KeyKey, _key);
        }
    }
}