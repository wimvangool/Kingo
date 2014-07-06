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
        TResult Execute(IQueryCache cache);

        /// <summary>
        /// Executes this query asynchronously, using the specified <paramref name="dispatcher"/> to
        /// post messages back to another thread such as the UI-thread.
        /// </summary>
        /// <param name="dispatcher">
        /// The dispatcher that is used to post messages to another thread such as the UI-thread.
        /// </param>
        /// <returns>The task that is responsible for executing this query.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="dispatcher"/> is <c>null</c>.
        /// </exception>
        Task<TResult> ExecuteAsync(IDispatcher dispatcher);

        /// <summary>
        /// Executes this query asynchronously, using the specified <paramref name="dispatcher"/> to
        /// post messages back to another thread such as the UI-thread.
        /// </summary>
        /// <param name="dispatcher">
        /// The dispatcher that is used to post messages to another thread such as the UI-thread.
        /// </param>
        /// <param name="token">
        /// Optional token that can be used to cancel the execution of this query.
        /// </param>
        /// <returns>The task that is responsible for executing this query.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="dispatcher"/> is <c>null</c>.
        /// </exception>
        Task<TResult> ExecuteAsync(IDispatcher dispatcher, CancellationToken? token);

        Task<TResult> ExecuteAsync(IDispatcher dispatcher, IQueryCache cache);

        Task<TResult> ExecuteAsync(IDispatcher dispatcher, IQueryCache cache, CancellationToken? token);
    }
}
