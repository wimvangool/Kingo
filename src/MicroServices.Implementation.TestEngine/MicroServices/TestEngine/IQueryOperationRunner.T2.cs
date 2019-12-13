using System;
using System.Threading.Tasks;

namespace Kingo.MicroServices.TestEngine
{
    /// <summary>
    /// When implemented by a class, represents a processor that can execute a specific query.
    /// </summary>
    /// <typeparam name="TRequest">Type of the request of the query.</typeparam>
    /// <typeparam name="TResponse">Type of the response of the query.</typeparam>
    public interface IQueryOperationRunner<TRequest, TResponse>
    {
        /// <summary>
        /// Executes the specified <paramref name="query"/> with the specified <paramref name="request"/>.
        /// </summary>
        /// <param name="query">Query to execute.</param>
        /// <param name="request">Request to execute.</param>        
        /// <exception cref="ArgumentNullException">
        /// <paramref name="request"/> or <paramref name="query"/> is <c>null</c>.
        /// </exception>   
        Task ExecuteAsync(IQuery<TRequest, TResponse> query, TRequest request);
    }
}
