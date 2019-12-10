using System;
using System.Threading.Tasks;

namespace Kingo.MicroServices.Controllers
{
    /// <summary>
    /// Contains extension methods for instance that implement the <see cref="IQueryOperationRunner{TResponse}"/> or
    /// <see cref="IQueryOperationRunner{TRequest,TResponse}"/> interface.
    /// </summary>
    public static class QueryOperationRunnerExtensions
    {
        #region [====== ExecuteAsync (1) ======]

        /// <summary>
        /// Executes the specified <paramref name="query"/>.
        /// </summary>
        /// <param name="runner">Runner that will execute the specified <paramref name="query"/>.</param>
        /// <param name="query">Query to execute.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="query"/> is <c>null</c>.
        /// </exception>
        public static Task ExecuteAsync<TResponse>(this IQueryOperationRunner<TResponse> runner, Func<IQueryOperationContext, TResponse> query) =>
            NotNull(runner).ExecuteAsync(QueryDecorator<TResponse>.Decorate(query));

        /// <summary>
        /// Executes the specified <paramref name="query"/>.
        /// </summary>
        /// <param name="runner">Processor that will execute the specified <paramref name="query"/>.</param>
        /// <param name="query">Query to execute.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="query"/> is <c>null</c>.
        /// </exception>
        public static Task ExecuteAsync<TResponse>(this IQueryOperationRunner<TResponse> runner, Func<IQueryOperationContext, Task<TResponse>> query) =>
            NotNull(runner).ExecuteAsync(QueryDecorator<TResponse>.Decorate(query));

        #endregion

        #region [====== ExecuteAsync (2) ======]

        /// <summary>
        /// Executes the specified <paramref name="query"/>.
        /// </summary>
        /// <param name="runner">Runner that will execute the specified <paramref name="query"/>.</param>
        /// <param name="query">Query to execute.</param>
        /// <param name="request">Request message of the query.</param>        
        /// <exception cref="ArgumentNullException">
        /// <paramref name="query"/> is <c>null</c>.
        /// </exception>
        public static Task ExecuteAsync<TRequest, TResponse>(this IQueryOperationRunner<TRequest, TResponse> runner, Func<TRequest, IQueryOperationContext, TResponse> query, TRequest request) =>
            NotNull(runner).ExecuteAsync(QueryDecorator<TRequest, TResponse>.Decorate(query), request);

        /// <summary>
        /// Executes the specified <paramref name="query"/>.
        /// </summary>
        /// <param name="runner">Runner that will execute the specified <paramref name="query"/>.</param>
        /// <param name="query">Query to execute.</param>
        /// <param name="request">Request message of the query.</param>        
        /// <exception cref="ArgumentNullException">
        /// <paramref name="query"/> is <c>null</c>.
        /// </exception>
        public static Task ExecuteAsync<TRequest, TResponse>(this IQueryOperationRunner<TRequest, TResponse> runner, Func<TRequest, IQueryOperationContext, Task<TResponse>> query, TRequest request) =>
            NotNull(runner).ExecuteAsync(QueryDecorator<TRequest, TResponse>.Decorate(query), request);

        #endregion

        private static TRunner NotNull<TRunner>(TRunner runner) where TRunner : class =>
            runner ?? throw new ArgumentNullException(nameof(runner));
    }
}
