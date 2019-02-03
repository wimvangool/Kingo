using System;
using System.Threading.Tasks;

namespace Kingo.MicroServices
{
    /// <summary>
    /// When implemented by a class, represents a processor that can execute a specific query.
    /// </summary>    
    /// <typeparam name="TResponse">Type of the response of the query.</typeparam>
    public interface IQueryProcessor<TResponse>
    {        
        /// <summary>
        /// Executes the specified <paramref name="query"/>.
        /// </summary>        
        /// <param name="query">Query to execute.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="query"/> is <c>null</c>.
        /// </exception>
        Task ExecuteAsync(IQuery<TResponse> query);
    }
}
