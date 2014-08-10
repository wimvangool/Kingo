using System.ComponentModel.Messaging.Server;
using System.Runtime.Caching;
using System.Threading;
using System.Threading.Tasks;

namespace System.ComponentModel.Messaging.Client
{
    /// <summary>
    /// Represents a dispatcher that dispatches or executes queries.
    /// </summary>
    /// <typeparam name="TResult">Type of the result of this query.</typeparam>
    public interface IQueryDispatcher<TResult> : IRequestDispatcher
    {
        /// <summary>
        /// Occurs when an execution of this <see cref="IQueryDispatcher{T}" /> has succeeded.
        /// </summary>
        new event EventHandler<ExecutionSucceededEventArgs<TResult>> ExecutionSucceeded;

        /// <summary>
        /// Executes the query synchronously.
        /// </summary>
        /// <returns>The result of this query.</returns>
        TResult Execute();

        /// <summary>
        /// Executes the query synchronously.
        /// </summary>
        /// <param name="cache">
        /// Optional cache-parameter that can be used by this query to get it's result from cache.
        /// </param>
        /// <returns>The result of this query.</returns>
        TResult Execute(ObjectCache cache);

        /// <summary>
        /// Executes the query asynchronously.
        /// </summary>      
        /// <param name="executionId">Identifier of the asynchronous task.</param>     
        /// <returns>The task that is responsible for executing this query.</returns>        
        Task<TResult> ExecuteAsync(Guid executionId);        

        /// <summary>
        /// Executes the query asynchronously.
        /// </summary>   
        /// <param name="executionId">Identifier of the asynchronous task.</param>        
        /// <param name="cache">
        /// When specified, the query will first consult the cache for a cached result, and if not found, will
        /// store the result before the task is completed.
        /// </param>
        /// <returns>The task that is responsible for executing this query.</returns>    
        Task<TResult> ExecuteAsync(Guid executionId, ObjectCache cache);

        /// <summary>
        /// Executes the query asynchronously.
        /// </summary>  
        /// <param name="executionId">Identifier of the asynchronous task.</param>   
        /// <param name="cache">
        /// When specified, the query will first consult the cache for a cached result, and if not found, will
        /// store the result before the task is completed.
        /// </param>      
        /// <param name="token">
        /// Optional token that can be used to cancel the execution of this query.
        /// </param>
        /// <param name="reporter">
        /// Optional reporter that can be used to report back the progress the task has made.
        /// </param>
        /// <returns>The task that is responsible for executing this query.</returns>    
        Task<TResult> ExecuteAsync(Guid executionId, ObjectCache cache, CancellationToken? token, IProgressReporter reporter);
    }
}
