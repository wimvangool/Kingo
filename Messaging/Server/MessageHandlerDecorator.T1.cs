namespace System.ComponentModel.Server
{
    /// <summary>
    /// Represents a decorator that can be used to construct a pipeline of <see cref="IMessageHandler{TMessage}">MessageHandlers</see>.
    /// </summary>
    /// <typeparam name="TMessage">Type of the message to handle.</typeparam>
    public class MessageHandlerDecorator<TMessage> : IMessageHandler<TMessage> where TMessage : class
    {
        private readonly IMessageHandler<TMessage> _nextHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageHandlerDecorator{TMessage}" /> class.
        /// </summary>
        /// <param name="nextHandler">The next handler to invoke.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="nextHandler"/> is <c>null</c>.
        /// </exception>
        public MessageHandlerDecorator(IMessageHandler<TMessage> nextHandler)
        {
            if (nextHandler == null)
            {
                throw new ArgumentNullException("nextHandler");
            }
            _nextHandler = nextHandler;
        }

        /// <summary>
        /// The next handler to invoke.
        /// </summary>
        protected IMessageHandler<TMessage> NextHandler
        {
            get { return _nextHandler; }
        }

        /// <inheritdoc />
        public virtual void Handle(TMessage message)
        {
            NextHandler.Handle(message);
        }
    }
}
