using System.ComponentModel.Resources;

namespace System.ComponentModel.Server
{
    /// <summary>
    /// Represents a module that validates a message and throws an <see cref="InvalidMessageException" /> if
    /// a message is invalid.
    /// </summary>
    /// <typeparam name="TMessage">Type of the message to handle.</typeparam>
    public class MessageValidationModule<TMessage> : MessageHandlerModule<TMessage> where TMessage : class, IMessage
    {
        #region [====== DefaultValidator ======]

        private sealed class DefaultValidator<T> : IMessageValidator<T> where T : class, IMessage
        {
            public bool TryGetValidationErrors(T message, out ValidationErrorTree errorTree)
            {
                return message.TryGetValidationErrors(out errorTree);
            }
        }

        #endregion

        private readonly IMessageHandler<TMessage> _handler;
        private readonly IMessageValidator<TMessage> _validator;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageValidationModule{TMessage}" /> class.
        /// </summary>
        /// <param name="handler">The next handler to invoke.</param>        
        /// <exception cref="ArgumentNullException">
        /// <paramref name="handler"/> is <c>null</c>.
        /// </exception>
        public MessageValidationModule(IMessageHandler<TMessage> handler)
            : this(handler, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageValidationModule{TMessage}" /> class.
        /// </summary>
        /// <param name="handler">The next handler to invoke.</param>
        /// <param name="validator">
        /// The validator that is used to validate each message.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="handler"/> is <c>null</c>.
        /// </exception>
        public MessageValidationModule(IMessageHandler<TMessage> handler, IMessageValidator<TMessage> validator)
        {
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }
            _handler = handler;
            _validator = validator ?? new DefaultValidator<TMessage>();
        }

        /// <inheritdoc />
        protected override IMessageHandler<TMessage> Handler
        {
            get { return _handler; }
        }

        /// <summary>
        /// The validator that is used to validate each message.
        /// </summary>
        protected virtual IMessageValidator<TMessage> Validator
        {
            get { return _validator; }
        }

        /// <summary>
        /// Validates the specified <paramref name="message"/> before it is passed to the next <see cref="Handler"/>.
        /// </summary>
        /// <param name="message">The message to handle.</param>  
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>      
        /// <exception cref="InvalidMessageException">
        /// <paramref name="message"/> is invalid.
        /// </exception>
        public override void Handle(TMessage message)
        {            
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            ValidationErrorTree errors;

            if (Validator.TryGetValidationErrors(message, out errors))
            {
                throw new InvalidMessageException(message, ExceptionMessages.MessageProcessor_InvalidMessage, errors);                
            }            
            Handler.Handle(message);
        }
    }
}
