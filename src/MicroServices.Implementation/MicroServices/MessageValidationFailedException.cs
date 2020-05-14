using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Kingo.Collections.Generic;
using Kingo.Reflection;
using static Kingo.Ensure;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Represents an exception that is thrown when a <see cref="IMessage" /> is considered invalid because it has an unexpected
    /// <see cref="MessageKind" /> or because its <see cref="IMessage.Content" /> is not valid.
    /// </summary>
    [Serializable]
    public class MessageValidationFailedException : InternalOperationException
    {
        private readonly IMessage _validatedMessage;
        private readonly ValidationResult[] _validationErrors;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageValidationFailedException" /> class.
        /// </summary>
        /// <param name="validatedMessage">The message that is considered invalid.</param>
        /// <param name="validationErrors">The collection of validation-errors.</param>
        /// <param name="message">Message of the exception.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="validatedMessage"/> or <paramref name="validationErrors"/> is <c>null</c>.
        /// </exception>
        public MessageValidationFailedException(IMessage validatedMessage, IEnumerable<ValidationResult> validationErrors, string message) : base(message)
        {
            _validatedMessage = IsNotNull(validatedMessage, nameof(validatedMessage));
            _validationErrors = IsNotNull(validationErrors, nameof(validationErrors)).WhereNotNull().ToArray();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageValidationFailedException" /> class.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        protected MessageValidationFailedException(SerializationInfo info, StreamingContext context) :
            base(info, context)
        {
            _validatedMessage = (IMessage) info.GetValue(nameof(ValidatedMessage), typeof(IMessage));
            _validationErrors = (ValidationResult[]) info.GetValue(nameof(ValidationErrors), typeof(ValidationResult[]));
        }

        /// <inheritdoc />
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue(nameof(ValidatedMessage), ValidatedMessage);
            info.AddValue(nameof(ValidationErrors), ValidationErrors);
        }

        /// <summary>
        /// The message that is considered invalid.
        /// </summary>
        public IMessage ValidatedMessage =>
            _validatedMessage;

        /// <summary>
        /// The collection of validation-errors.
        /// </summary>
        public IReadOnlyList<ValidationResult> ValidationErrors =>
            _validationErrors;

        /// <inheritdoc />
        public override string ToString() =>
            $"{GetType().FriendlyName()} ({ValidatedMessage.Content.GetType().FriendlyName()}: {ValidationErrors.Count} error(s))";

        /// <inheritdoc />
        protected override bool IsBadRequest(MicroProcessorOperationStackTrace operationStackTrace) =>
            IsBadRequest(ValidatedMessage);

        private static bool IsBadRequest(IMessage message) =>
            message.Direction == MessageDirection.Input && (message.Kind == MessageKind.Request || message.Kind == MessageKind.Command);

        /// <inheritdoc />
        protected override BadRequestException ToBadRequestException(MicroProcessorOperationStackTrace operationStackTrace) =>
            new BadRequestMessageException(operationStackTrace, _validationErrors, Message, this);
    }
}
