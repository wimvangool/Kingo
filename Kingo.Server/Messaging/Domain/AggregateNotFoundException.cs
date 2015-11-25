using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Kingo.Messaging.Domain
{
    /// <summary>
    /// This type of exception is thrown when a <see cref="Repository{T, S, U}" /> was unable to retrieve,
    /// insert, update or delete an <see cref="IVersionedObject{T, S}" />.
    /// </summary>
    [Serializable]
    public abstract class AggregateNotFoundException : DomainException
    {
        private const string _AggregateTypeKey = "_aggregateType";
        private readonly Type _aggregateType;

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateNotFoundException" /> class.
        /// </summary>                
        /// <param name="aggregateType">Type of the aggregate that was not found.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="aggregateType" /> is <c>null</c>.
        /// </exception>
        protected AggregateNotFoundException(Type aggregateType)
        {
            if (aggregateType == null)
            {
                throw new ArgumentNullException("aggregateType");
            }
            _aggregateType = aggregateType;
        }       

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateNotFoundException" /> class.
        /// </summary>        
        /// <param name="aggregateType">Type of the aggregate that was not found.</param>        
        /// <param name="message">Message of the exception.</param>  
        /// <exception cref="ArgumentNullException">
        /// <paramref name="aggregateType" /> is <c>null</c>.
        /// </exception>      
        internal AggregateNotFoundException(Type aggregateType, string message)
            : base(message)
        {
            if (aggregateType == null)
            {
                throw new ArgumentNullException("aggregateType");
            }
            _aggregateType = aggregateType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateNotFoundException" /> class.
        /// </summary>        
        /// <param name="aggregateType">Type of the aggregate that was not found.</param>        
        /// <param name="message">Message of the exception.</param>
        /// <param name="innerException">Cause of the exception.</param>   
        /// <exception cref="ArgumentNullException">
        /// <paramref name="aggregateType" /> is <c>null</c>.
        /// </exception>     
        internal AggregateNotFoundException(Type aggregateType, string message, Exception innerException)
            : base(message, innerException)
        {
            if (aggregateType == null)
            {
                throw new ArgumentNullException("aggregateType");
            }
            _aggregateType = aggregateType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateNotFoundException" /> class.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        internal AggregateNotFoundException(SerializationInfo info, StreamingContext context)
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
        /// Returns the <see cref="Type" /> of the aggregate that was not found.
        /// </summary>
        public Type AggregateType
        {
            get { return _aggregateType; ;}
        }        
    }
}
