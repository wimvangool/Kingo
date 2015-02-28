namespace System.ComponentModel.Server
{
    /// <summary>
    /// When implemented by a class, represents a <see cref="IQuery{TMessageIn, TMessageOut}" /> ready to be executed.
    /// </summary>
    /// <typeparam name="TMessageOut">Type of the result of the query.</typeparam>
    public interface IQuery<out TMessageOut> where TMessageOut : class, IMessage<TMessageOut>
    {
        /// <summary>
        /// Message containing the parameters of the query.
        /// </summary>
        IMessage MessageIn
        {
            get;
        }

        /// <summary>
        /// Indicates whether or not it is allowed to consult the cache for
        /// any previously stored result of the query.
        /// </summary>
        bool AllowCacheRead
        {
            get;
        }

        /// <summary>
        /// Indicates whether or not it is allowed to store any retrieved result
        /// into the cache after the query has been executed.
        /// </summary>
        bool AllowCacheWrite
        {
            get;
        }

        /// <summary>
        /// Invokes the query and returns its result.
        /// </summary>
        /// <returns>The result of the query.</returns>
        TMessageOut Invoke();
    }
}
