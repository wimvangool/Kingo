namespace System.ComponentModel.Server
{
    /// <summary>
    /// Serves as a base class for all <see cref="IMessageHandler{TMessage}">MessageHandlers</see>.
    /// </summary>
    /// <typeparam name="TMessage">Type of message to handle.</typeparam>
    public abstract class MessageHandler<TMessage> : IMessageHandler<TMessage> where TMessage : class
    {
        /// <inheritdoc />               
        public abstract void Handle(TMessage message);
    }
}
