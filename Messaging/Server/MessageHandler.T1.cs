namespace System.ComponentModel.Server
{
    /// <summary>
    /// Serves as a base class for all <see cref="IMessageHandler{TMessage}">MessageHandlers</see>.
    /// </summary>
    /// <typeparam name="TMessage">Type of message to handle.</typeparam>
    public abstract class MessageHandler<TMessage> : IMessageHandler<TMessage> where TMessage : class
    {
        void IMessageHandler<TMessage>.Handle(TMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            Handle(message);
        }

        /// <summary>
        /// Handles the specified <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The message to handle.</param>        
        protected abstract void Handle(TMessage message);
    }
}
