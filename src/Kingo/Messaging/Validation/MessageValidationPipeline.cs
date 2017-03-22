using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Kingo.Resources;

namespace Kingo.Messaging.Validation
{
    /// <summary>
    /// Represents a pipeline that validates all messages going through and throws an <see cref="InvalidMessageException" /> when
    /// a message contains valiation errors.
    /// </summary>
    public class MessageValidationPipeline : MicroProcessorPipeline
    {
        private readonly ConcurrentDictionary<Type, Func<object, ErrorInfo>> _validators;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageValidationPipeline" /> class.
        /// </summary>
        public MessageValidationPipeline()
        {
            _validators = new ConcurrentDictionary<Type, Func<object, ErrorInfo>>();
        }

        /// <summary>
        /// Registers the validator that is created through the specified <paramref name="validatorFactory"/> for messages
        /// of type <typeparamref name="TMessage"/>.
        /// </summary>
        /// <typeparam name="TMessage">Type of the messages to validate.</typeparam>
        /// <param name="validatorFactory">The delegate that will be used to created the validator.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="validatorFactory"/> is <c>null</c>.
        /// </exception>
        public void Register<TMessage>(Func<IMessageValidator<TMessage>> validatorFactory)
        {
            if (validatorFactory == null)
            {
                throw new ArgumentNullException(nameof(validatorFactory));
            }
            Register(validatorFactory.Invoke());
        }

        /// <summary>
        /// Registers the specified <paramref name="validator"/> for messages of type <typeparamref name="TMessage"/>.
        /// </summary>
        /// <typeparam name="TMessage">Type of the messages to validate.</typeparam>
        /// <param name="validator">The validator to register.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="validator"/> is <c>null</c>.
        /// </exception>
        public void Register<TMessage>(IMessageValidator<TMessage> validator)
        {
            if (validator == null)
            {
                throw new ArgumentNullException(nameof(validator));
            }
            Register(typeof(TMessage), message => validator.Validate((TMessage) message, true));            
        }
        
        private void Register(Type messageType, Func<object, ErrorInfo> validateMethod) =>
            _validators.AddOrUpdate(messageType, type => validateMethod, (type, oldMethod) => validateMethod);

        /// <summary>
        /// Validates the message and throws an <see cref="InvalidMessageException" /> is any validation errors are found.
        /// </summary>
        /// <typeparam name="TResult">Type of the result of the operation.</typeparam>
        /// <param name="handlerOrQuery">A message handler or query that will perform the operation.</param>
        /// <param name="context">Context of the <see cref="IMicroProcessor" /> that is currently handling the message.</param>
        /// <returns>The result of the operation.</returns>
        /// <exception cref="InvalidMessageException">
        /// The specified message is invalid.
        /// </exception>
        protected override async Task<TResult> HandleOrExecuteAsync<TResult>(MessageHandlerOrQuery<TResult> handlerOrQuery, IMicroProcessorContext context)
        {
            var messageToValidate = context.Messages.Current.Message;
            var errorInfo = Validate(messageToValidate);
            if (errorInfo.HasErrors)
            {
                throw NewInvalidMessageException(messageToValidate.GetType(), errorInfo);
            }
            return await handlerOrQuery.HandleMessageOrExecuteQueryAsync(context);
        }  

        private ErrorInfo Validate(object messageToValidate)
        {
            Func<object, ErrorInfo> validateMethod;

            if (TryGetRegisteredValidateMethod(messageToValidate.GetType(), out validateMethod))
            {
                return validateMethod.Invoke(validateMethod);
            }
            var message = messageToValidate as IMessage;
            if (message != null)
            {
                return message.Validate(true);
            }
            return ErrorInfo.Empty;
        }

        private bool TryGetRegisteredValidateMethod(Type messageType, out Func<object, ErrorInfo> validateMethod) =>
            _validators.TryGetValue(messageType, out validateMethod);

        private static InvalidMessageException NewInvalidMessageException(Type messageType, ErrorInfo errorInfo)
        {
            var messageFormat = ExceptionMessages.MessageValidationPipeline_InvalidMessage;
            var message = string.Format(messageFormat, messageType.FriendlyName(), errorInfo.ErrorCount);
            return new InvalidMessageException(errorInfo, message);
        }
    }
}
