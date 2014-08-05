using System.ComponentModel.Messaging.Resources;
using System.Runtime.Serialization;

namespace System.ComponentModel.Messaging.Server
{
    /// <summary>
    /// This type of exception is thrown when a <see cref="Repository{T, S, U}" /> was unable to retrieve
    /// an <see cref="IAggregate{T, S}" /> with a certain key.
    /// </summary>
    [Serializable]
    public class AggregateNotFoundException : DomainException
    {        
        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateNotFoundException" /> class.
        /// </summary>
        public AggregateNotFoundException() {}

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateNotFoundException" /> class.
        /// </summary>
        /// <param name="message">Message of the exception.</param>
        public AggregateNotFoundException(string message)
            : base(message) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateNotFoundException" /> class.
        /// </summary>
        /// <param name="message">Message of the exception.</param>
        /// <param name="inner">Cause of the exception.</param>
        public AggregateNotFoundException(string message, Exception inner)
            : base(message, inner) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateNotFoundException" /> class.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected AggregateNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context) {}

        /// <summary>
        /// Creates and returns a new <see cref="AggregateNotFoundException" /> that indicates that an aggregate
        /// of type <paramref name="aggregateType"/> with the specified <paramref name="key"/> was not found.
        /// </summary>
        /// <param name="key">Key of the aggregate.</param>
        /// <param name="aggregateType">Type of the aggregate.</param>
        /// <returns>
        /// A new instance of the <see cref="AggregateNotFoundException" /> class with an appropriate message.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="key"/> or <paramref name="aggregateType"/> is <c>null</c>.
        /// </exception>
        public static AggregateNotFoundException NewAggregateNotFoundException(object key, Type aggregateType)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }
            if (aggregateType == null)
            {
                throw new ArgumentNullException("aggregateType");
            }
            var messageFormat = ExceptionMessages.AggregateNotFoundException_Message;
            var message = string.Format(messageFormat, aggregateType.Name, key);
            return new AggregateNotFoundException(message);
        }
    }
}
