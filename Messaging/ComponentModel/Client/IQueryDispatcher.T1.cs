using System.Threading;
using System.Threading.Tasks;

namespace System.ComponentModel.Client
{
    /// <summary>
    /// Represents a dispatcher that dispatches or executes queries.
    /// </summary>
    /// <typeparam name="TMessageOut">Type of the result of this query.</typeparam>
    public interface IQueryDispatcher<TMessageOut> : IRequestDispatcher where TMessageOut : class, IMessage<TMessageOut>
    {
        /// <summary>
        /// Occurs when an execution of this <see cref="IQueryDispatcher{T}" /> has succeeded.
        /// </summary>
        new event EventHandler<ExecutionSucceededEventArgs<TMessageOut>> ExecutionSucceeded;

        /// <summary>
        /// Executes the query synchronously.
        /// </summary>
        /// <param name="requestId">Identifier of the request.</param>        
        /// <returns>The result of this query.</returns>
        TMessageOut Execute(Guid requestId);                               

        /// <summary>
        /// Executes the query asynchronously.
        /// </summary>  
        /// <param name="requestId">Identifier of the request.</param>             
        /// <param name="token">
        /// Optional token that can be used to cancel the execution of this query.
        /// </param>        
        /// <returns>The task that is responsible for executing this query.</returns>    
        Task<TMessageOut> ExecuteAsync(Guid requestId, CancellationToken? token = null);
    }
}
