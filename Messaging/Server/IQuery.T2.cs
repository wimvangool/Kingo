namespace System.ComponentModel.Server
{
    /// <summary>
    /// When implemented by a class, represents a query that accepts a <see cref="IRequestMessage{TMessageIn}" />
    /// and returns a result in the form of a <see cref="IMessage{TMesageOut}" />.
    /// </summary>
    /// <typeparam name="TMessageIn">Type of the message that is consumed by this query.</typeparam>
    /// <typeparam name="TMessageOut">Type of the message that is returned by this query.</typeparam>
    public interface IQuery<in TMessageIn, out TMessageOut>
        where TMessageIn : class, IRequestMessage<TMessageIn>
        where TMessageOut : class, IMessage<TMessageOut>
    {
        /// <summary>
        /// Executes the query.
        /// </summary>
        /// <param name="message">Message containing the parameters of this query.</param>
        /// <returns>The result of this query.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        TMessageOut Execute(TMessageIn message);
    }
}
