using System;
using System.Threading.Tasks;

namespace Kingo.Messaging
{
    /// <summary>
    /// When implemented by a class, represents a query that accepts a message
    /// and returns a result in the form of a <typeparamref name="TMessageOut"/>.
    /// </summary>
    /// <typeparam name="TMessageIn">Type of the message that is consumed by this query.</typeparam>
    /// <typeparam name="TMessageOut">Type of the message that is returned by this query.</typeparam>
    public interface IQuery<in TMessageIn, TMessageOut> where TMessageIn : class
    {
        /// <summary>
        /// Executes the query.
        /// </summary>
        /// <param name="message">Message containing the parameters of this query.</param>
        /// <returns>The result of this query.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        Task<TMessageOut> ExecuteAsync(TMessageIn message);
    }
}
