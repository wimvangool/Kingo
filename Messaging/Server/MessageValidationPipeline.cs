using System.ComponentModel.Resources;

namespace System.ComponentModel.Server
{
    /// <summary>
    /// Represents a pipeline that validates a message and throws an <see cref="InvalidMessageException" /> if
    /// a message is invalid.
    /// </summary>
    /// <typeparam name="TMessage">Type of the message to handle.</typeparam>
    public class MessageValidationPipeline<TMessage> : IMessageHandler<TMessage> where TMessage : class
    {
        private readonly IMessageHandler<TMessage> _handler;
        private readonly IMessageValidator<TMessage> _validator;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageValidationPipeline{TMessage}" /> class.
        /// </summary>
        /// <param name="handler">The next handler to invoke.</param>
        /// <param name="validator">
        /// The validator that is used to validate each message.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="handler"/> is <c>null</c>.
        /// </exception>
        public MessageValidationPipeline(IMessageHandler<TMessage> handler, IMessageValidator<TMessage> validator)
        {
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }
            _handler = handler;
            _validator = validator;
        }

        /// <summary>
        /// The next handler to invoke.
        /// </summary>
        protected IMessageHandler<TMessage> Handler
        {
            get { return _handler; }
        }

        /// <summary>
        /// The validator that is used to validate each message.
        /// </summary>
        protected IMessageValidator<TMessage> Validator
        {
            get { return _validator; }
        }

        /// <summary>
        /// Validates the specified <paramref name="message"/> before it is passed to the next <see cref="Handler"/>.
        /// </summary>
        /// <param name="message">The message to handle.</param>
        /// <exception cref="InvalidMessageException">
        /// <see cref="Validator"/> is not <c>null</c> and <paramref name="message"/> is invalid.
        /// </exception>
        public void Handle(TMessage message)
        {
            MessageErrorTree errors;

            if (Validator != null && Validator.IsNotValid(message, out errors))
            {
                throw new InvalidMessageException(message, ExceptionMessages.MessageProcessor_InvalidMessage, errors);                
            }            
            Handler.Handle(message);
        }
    }
}
