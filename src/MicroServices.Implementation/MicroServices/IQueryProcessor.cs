using System;
using System.Threading.Tasks;

namespace Kingo.MicroServices
{
    /// <summary>
    /// When implemented by a class, represents a processor of (internal) queries.
    /// </summary>
    public interface IQueryProcessor
    {
        /// <summary>
        /// Executes the specified <paramref name="query"/> and returns its result.
        /// </summary>
        /// <typeparam name="TResponse">Type of the response of the query.</typeparam>
        /// <param name="query">The query to execute.</param>
        /// <returns>The result of the query.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="query"/> is <c>null</c>.
        /// </exception>
        Task<TResponse> ExecuteQueryAsync<TResponse>(IQuery<TResponse> query);

        /// <summary>
        /// Executes the specified <paramref name="query"/> and returns its result.
        /// </summary>
        /// <typeparam name="TRequest">Type of the request of the query.</typeparam>
        /// <typeparam name="TResponse">Type of the response of the query.</typeparam>
        /// <param name="query">The query to execute.</param>
        /// <param name="message">Message containing the parameters of this query.</param>
        /// <returns>The result of the query.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="query"/> or <paramref name="message"/> is <c>null</c>.
        /// </exception>
        Task<TResponse> ExecuteQueryAsync<TRequest, TResponse>(IQuery<TRequest, TResponse> query, TRequest message);
    }
}
