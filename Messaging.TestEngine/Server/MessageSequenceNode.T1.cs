namespace System.ComponentModel.Server
{
    /// <summary>
    /// Represents a sequence containing just a single message.
    /// </summary>
    /// <typeparam name="TMessage">Type of the message.</typeparam>
    public class MessageSequenceNode<TMessage> : MessageSequence where TMessage : class, IMessage<TMessage>
    {
        private readonly TMessage _message;
        private readonly IMessageValidator<TMessage> _validator;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageSequenceNode{TMessage}" /> class.
        /// </summary>
        /// <param name="message">The message of this sequence.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        public MessageSequenceNode(TMessage message)
            : this(message, message as IMessageValidator<TMessage>) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageSequenceNode{TMessage}" /> class.
        /// </summary>
        /// <param name="message">The message of this sequence.</param>
        /// <param name="validator">Optional validator that is used to validate the message.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        public MessageSequenceNode(TMessage message, IMessageValidator<TMessage> validator)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            _message = message;
            _validator = validator;
        }

        /// <summary>
        /// The message of this sequence.
        /// </summary>
        protected TMessage Message
        {
            get { return _message; }
        }

        /// <summary>
        /// Optional validator that is used to validate the message.
        /// </summary>
        protected IMessageValidator<TMessage> Validator
        {
            get { return _validator; }
        }

        /// <inheritdoc />
        public override void ProcessWith(IMessageProcessor processor)
        {
            if (processor == null)
            {
                throw new ArgumentNullException("processor");
            }
            processor.Handle(_message, _validator);
        }
    }
}
