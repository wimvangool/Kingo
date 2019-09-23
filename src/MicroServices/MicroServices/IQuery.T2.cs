using System;
using System.Threading.Tasks;

namespace Kingo.MicroServices
{
    /// <summary>
    /// When implemented by a class, represents a query that accepts a message
    /// and returns a result in the form of a <typeparamref name="TResponse"/>.
    /// </summary>
    /// <typeparam name="TRequest">Type of the message that is consumed by this query.</typeparam>
    /// <typeparam name="TResponse">Type of the message that is returned by this query.</typeparam>
    public interface IQuery<in TRequest, TResponse>
    {
        /// <summary>
        /// Executes the query.
        /// </summary>
        /// <param name="message">Message containing the parameters of this query.</param>
        /// <param name="context">Context of the <see cref="IMicroProcessor" /> that is currently executing the query.</param>
        /// <returns>The result of this query.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        Task<TResponse> ExecuteAsync(TRequest message, IQueryOperationContext context);
    }
}
