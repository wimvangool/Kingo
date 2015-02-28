namespace System.ComponentModel.Server
{
    /// <summary>
    /// When implemented by a class, represents a module in a <see cref="IMessageHandler{TMessage}" />'s
    /// pipeline.
    /// </summary>
    public interface IMessageHandlerModule : IDisposable
    {
        /// <summary>
        /// Invokes the specified <paramref name="handler"/> while adding specific pipeline logic.
        /// </summary>
        /// <param name="handler">The handler to invoke.</param>
        /// <exception cref="ObjectDisposedException">
        /// This instance has been disposed.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="handler"/> is <c>null</c>.
        /// </exception>
        void Invoke(IMessageHandler handler);
    }
}
