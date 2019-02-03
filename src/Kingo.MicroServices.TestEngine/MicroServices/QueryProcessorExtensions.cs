using System;
using System.Threading.Tasks;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Contains extension methods for instance that implement the <see cref="IQueryProcessor{TResponse}"/> or
    /// <see cref="IQueryProcessor{TRequest, TResponse}"/> interface.
    /// </summary>
    public static class QueryProcessorExtensions
    {
        #region [====== ExecuteAsync (1) ======]

        /// <summary>
        /// Executes the specified <paramref name="query"/>.
        /// </summary>
        /// <param name="processor">Processor that will execute the specified <paramref name="query"/>.</param>
        /// <param name="query">Query to execute.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="query"/> is <c>null</c>.
        /// </exception>
        public static Task ExecuteAsync<TResponse>(this IQueryProcessor<TResponse> processor, Func<QueryContext, TResponse> query) =>
            processor.ExecuteAsync(QueryDecorator<TResponse>.Decorate(query));

        /// <summary>
        /// Executes the specified <paramref name="query"/>.
        /// </summary>
        /// <param name="processor">Processor that will execute the specified <paramref name="query"/>.</param>
        /// <param name="query">Query to execute.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="query"/> is <c>null</c>.
        /// </exception>
        public static Task ExecuteAsync<TResponse>(this IQueryProcessor<TResponse> processor, Func<QueryContext, Task<TResponse>> query) =>
            processor.ExecuteAsync(QueryDecorator<TResponse>.Decorate(query));

        #endregion

        #region [====== ExecuteAsync (2) ======]

        /// <summary>
        /// Executes the specified <paramref name="query"/>.
        /// </summary>
        /// <param name="processor">Processor that will execute the specified <paramref name="query"/>.</param>
        /// <param name="request">Request message of the query.</param>
        /// <param name="query">Query to execute.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="query"/> is <c>null</c>.
        /// </exception>
        public static Task ExecuteAsync<TRequest, TResponse>(this IQueryProcessor<TRequest, TResponse> processor, TRequest request, Func<TRequest, QueryContext, TResponse> query) =>
            processor.ExecuteAsync(request, QueryDecorator<TRequest, TResponse>.Decorate(query));

        /// <summary>
        /// Executes the specified <paramref name="query"/>.
        /// </summary>
        /// <param name="processor">Processor that will execute the specified <paramref name="query"/>.</param>
        /// <param name="request">Request message of the query.</param>
        /// <param name="query">Query to execute.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="query"/> is <c>null</c>.
        /// </exception>
        public static Task ExecuteAsync<TRequest, TResponse>(this IQueryProcessor<TRequest, TResponse> processor, TRequest request, Func<TRequest, QueryContext, Task<TResponse>> query) =>
            processor.ExecuteAsync(request, QueryDecorator<TRequest, TResponse>.Decorate(query));

        #endregion
    }
}
