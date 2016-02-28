using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Kingo.Messaging.Domain
{

    /// <summary>
    /// This exception is thrown when an aggregate could not be updated because of a concurrency conflict.
    /// </summary>
    [Serializable]
    public class ConcurrencyException : Exception
    {
        private const string _AggregateKey = "_aggregate";
        private readonly object _aggregate;

        private const string _OriginalVersionKey = "_originalVersion";
        private readonly object _originalVersion;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrencyException" /> class.
        /// </summary>
        /// <param name="aggregate">The aggregate that could not be updated.</param>
        /// <param name="originalVersion">Version of the aggregate before it was updated.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="aggregate"/> or <paramref name="originalVersion"/> is <c>null</c>.
        /// </exception>
        public ConcurrencyException(object aggregate, object originalVersion)
        {
            if (aggregate == null)
            {
                throw new ArgumentNullException(nameof(aggregate));
            }
            if (originalVersion == null)
            {
                throw new ArgumentNullException(nameof(originalVersion));
            }
            _aggregate = aggregate;
            _originalVersion = originalVersion;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrencyException" /> class.
        /// </summary>
        /// <param name="aggregate">The aggregate that could not be updated.</param>
        /// <param name="originalVersion">Version of the aggregate before it was updated.</param>
        /// <param name="message">Message of the exception.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="aggregate"/> or <paramref name="originalVersion"/> is <c>null</c>.
        /// </exception>        
        public ConcurrencyException(object aggregate, object originalVersion, string message)
            : base(message)
        {
            if (aggregate == null)
            {
                throw new ArgumentNullException(nameof(aggregate));
            }
            if (originalVersion == null)
            {
                throw new ArgumentNullException(nameof(originalVersion));
            }
            _aggregate = aggregate;
            _originalVersion = originalVersion;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrencyException" /> class.
        /// </summary>        
        /// <param name="aggregate">The aggregate that could not be updated.</param>
        /// <param name="originalVersion">Version of the aggregate before it was updated.</param>
        /// <param name="message">Message of the exception.</param>
        /// <param name="innerException">Cause of this exception.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="aggregate"/> or <paramref name="originalVersion"/> is <c>null</c>.
        /// </exception>
        public ConcurrencyException(object aggregate, object originalVersion, string message, Exception innerException)
            : base(message, innerException)
        {
            if (aggregate == null)
            {
                throw new ArgumentNullException(nameof(aggregate));
            }
            if (originalVersion == null)
            {
                throw new ArgumentNullException(nameof(originalVersion));
            }
            _aggregate = aggregate;
            _originalVersion = originalVersion;
        }        

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrencyException" /> class.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        protected ConcurrencyException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _aggregate = info.GetValue(_AggregateKey, typeof(object));
            _originalVersion = info.GetValue(_OriginalVersionKey, typeof(object));
        }

        /// <inheritdoc />
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue(_AggregateKey, _aggregate);
            info.AddValue(_OriginalVersionKey, _originalVersion);
        }

        /// <summary>
        /// The aggregate that could not be updated.
        /// </summary>
        public object Aggregate
        {
            get { return _aggregate; }
        }

        /// <summary>
        /// Version of the aggregate before it was updated.
        /// </summary>
        public object OriginalVersion
        {
            get { return _originalVersion; }
        }
    }
}
