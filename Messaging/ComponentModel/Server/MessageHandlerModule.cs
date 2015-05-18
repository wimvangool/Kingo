namespace System.ComponentModel.Server
{
    /// <summary>
    /// Serves as the base-class for all modules in a <see cref="IMessageHandler{TMessage}" /> pipeline.
    /// </summary>
    public abstract class MessageHandlerModule
    {
        /// <summary>
        /// Invokes the specified <paramref name="handler"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="handler"/> is <c>null</c>.
        /// </exception>
        public abstract void Invoke(IMessageHandler handler);      
    }
}
