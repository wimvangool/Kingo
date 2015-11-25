using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Kingo.Messaging.Domain
{
    /// <summary>
    /// This exception is thrown when a value is considered invalid inside the domain model.
    /// </summary>
    /// <typeparam name="TValue">Type of the value.</typeparam>
    [Serializable]
    public abstract class InvalidValueException<TValue> : BusinessRuleViolationException
    {
        private const string _ValueKey = "_value";
        private readonly TValue _value;

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidValueException{T}" /> class.
        /// </summary>
        /// <param name="value">The invalid value.</param>
        protected InvalidValueException(TValue value)
        {
            _value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidValueException{T}" /> class.
        /// </summary>
        /// <param name="value">The invalid value.</param>
        /// <param name="message">Message of the exception.</param>
        protected InvalidValueException(TValue value, string message)
            : base(message)
        {
            _value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidValueException{T}" /> class.
        /// </summary>
        /// <param name="value">The invalid value.</param>
        /// <param name="message">Message of the exception.</param>
        /// <param name="innerException">Cause of this exception.</param>
        protected InvalidValueException(TValue value, string message, Exception innerException)
            : base(message, innerException)
        {
            _value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionalException" /> class.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        protected InvalidValueException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _value = (TValue) info.GetValue(_ValueKey, typeof(TValue));
        }

        /// <inheritdoc />
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue(_ValueKey, _value);
        }
    }
}
