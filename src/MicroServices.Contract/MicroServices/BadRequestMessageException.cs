using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using static Kingo.Ensure;

namespace Kingo.MicroServices
{
    /// <summary>
    /// This exception is thrown when the input-message was not valid.
    /// </summary>
    [Serializable]
    public class BadRequestMessageException : BadRequestException
    {
        private readonly ValidationResult[] _validationErrors;

        /// <summary>
        /// Initializes a new instance of the <see cref="BadRequestMessageException" /> class.
        /// </summary>
        /// <param name="operationStackTrace">The stack trace of the processor at the time the exception was thrown.</param>
        /// <param name="validationErrors">The collection of validation-errors.</param>
        /// <param name="message">Message of the exception.</param>
        /// <param name="innerException">Cause of this exception.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="operationStackTrace"/> or <paramref name="validationErrors"/> is <c>null</c>.
        /// </exception>
        public BadRequestMessageException(MicroProcessorOperationStackTrace operationStackTrace, IEnumerable<ValidationResult> validationErrors, string message = null, Exception innerException = null) :
            base(operationStackTrace, message, innerException)
        {
            _validationErrors = IsNotNull(validationErrors, nameof(validationErrors)).ToArray();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BadRequestException" /> class.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        protected BadRequestMessageException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            _validationErrors = (ValidationResult[]) info.GetValue(nameof(ValidationErrors), typeof(ValidationResult[]));
        }

        /// <inheritdoc />
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue(nameof(ValidationErrors), ValidationErrors);
        }

        /// <summary>
        /// The collection of validation-errors.
        /// </summary>
        public IReadOnlyCollection<ValidationResult> ValidationErrors =>
            _validationErrors;
    }
}
