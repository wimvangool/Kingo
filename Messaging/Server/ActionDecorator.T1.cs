namespace System.ComponentModel.Server
{
    /// <summary>
    /// This type is used to support implicit type conversion from a <see cref="Action{TMessage}" /> to a
    /// <see cref="IMessageHandler{TMessage}" />.
    /// </summary>
    /// <typeparam name="TMessage">Type of the message that is handled by this handler.</typeparam>
    public sealed class ActionDecorator<TMessage> : IMessageHandler<TMessage> where TMessage : class
    {
        private readonly Action<TMessage> _handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionDecorator{TMessage}" /> class.
        /// </summary>
        /// <param name="handler">The handler to invoke.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="handler"/> is <c>null</c>.
        /// </exception>
        public ActionDecorator(Action<TMessage> handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }
            _handler = handler;
        }

        void IMessageHandler<TMessage>.Handle(TMessage message)
        {
            _handler.Invoke(message);
        }

        /// <summary>
        /// Implicitly converts <paramref name="handler"/> to an instance of <see cref="ActionDecorator{TMessage}" />.
        /// </summary>
        /// <param name="handler">The value to convert.</param>
        /// <returns>
        /// <c>null</c> if <paramref name="handler"/> is <c>null</c>;
        /// otherwise, a new instance of <see cref="ActionDecorator{TMessage}" />.
        /// </returns>
        public static implicit operator ActionDecorator<TMessage>(Action<TMessage> handler)
        {
            return handler == null ? null : new ActionDecorator<TMessage>(handler);
        }
    }
}
