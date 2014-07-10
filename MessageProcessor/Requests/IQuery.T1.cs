using System;
using System.Threading;
using System.Threading.Tasks;

namespace YellowFlare.MessageProcessing.Requests
{
    /// <summary>
    /// Represents a query or read-operation.
    /// </summary>
    /// <typeparam name="TResult">Type of the result of this query.</typeparam>
    public interface IQuery<TResult> : IRequest
    {
        /// <summary>
        /// Occurs when an execution of this <see cref="IQuery{T}" /> has succeeded.
        /// </summary>
        new event EventHandler<ExecutionSucceededEventArgs<TResult>> ExecutionSucceeded;

        /// <summary>
        /// Executes this query synchronously.
        /// </summary>
        /// <returns>The result of this query.</returns>
        TResult Execute();

        /// <summary>
        /// Executes this query synchronously.
        /// </summary>
        /// <param name="cache">
        /// Optional cache-parameter that can be used by this query to get it's result from cache.
        /// </param>
        /// <returns>The result of this query.</returns>
        TResult Execute(QueryCache cache);

        /// <summary>
        /// Executes this query asynchronously.
        /// </summary>        
        /// <returns>The task that is responsible for executing this query.</returns>        
        Task<TResult> ExecuteAsync();

        /// <summary>
        /// Executes this query asynchronously.
        /// </summary>        
        /// <param name="token">
        /// Optional token that can be used to cancel the execution of this query.
        /// </param>
        /// <returns>The task that is responsible for executing this query.</returns>        
        Task<TResult> ExecuteAsync(CancellationToken? token);

        /// <summary>
        /// Executes this query asynchronously.
        /// </summary>        
        /// <param name="cache">
        /// When specified, the query will first consult the cache for a cached result, and if not found, will
        /// store the result before the task is completed.
        /// </param>
        /// <returns>The task that is responsible for executing this query.</returns>    
        Task<TResult> ExecuteAsync(QueryCache cache);

        /// <summary>
        /// Executes this query asynchronously.
        /// </summary>  
        /// <param name="cache">
        /// When specified, the query will first consult the cache for a cached result, and if not found, will
        /// store the result before the task is completed.
        /// </param>      
        /// <param name="token">
        /// Optional token that can be used to cancel the execution of this query.
        /// </param>
        /// <returns>The task that is responsible for executing this query.</returns>    
        Task<TResult> ExecuteAsync(QueryCache cache, CancellationToken? token);
    }
}
