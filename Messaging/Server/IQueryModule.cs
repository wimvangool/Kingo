namespace System.ComponentModel.Server
{
    /// <summary>
    /// When implemented by a class, represents a module in a <see cref="IQuery{TMessageIn, TMessageOut}" />'s pipeline.
    /// </summary>
    public interface IQueryModule : IDisposable
    {
        /// <summary>
        /// Executes the specified <paramref name="query"/> while adding specific pipeline logic.
        /// </summary>
        /// <typeparam name="TMessageOut">Type of the result of <paramref name="query"/>.</typeparam>
        /// <param name="query">The handler to execute.</param>
        /// <returns>The result of the <paramref name="query"/>.</returns>
        /// <exception cref="ObjectDisposedException">
        /// This instance has been disposed.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="query"/> is <c>null</c>.
        /// </exception>
        TMessageOut Invoke<TMessageOut>(IQuery<TMessageOut> query) where TMessageOut : class, IMessage<TMessageOut>;
    }
}
