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
        public static Task ExecuteAsync<TResponse>(this IQueryProcessor<TResponse> processor, Func<QueryOperationContext, TResponse> query) =>
            processor.ExecuteAsync(QueryDecorator<TResponse>.Decorate(query));

        /// <summary>
        /// Executes the specified <paramref name="query"/>.
        /// </summary>
        /// <param name="processor">Processor that will execute the specified <paramref name="query"/>.</param>
        /// <param name="query">Query to execute.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="query"/> is <c>null</c>.
        /// </exception>
        public static Task ExecuteAsync<TResponse>(this IQueryProcessor<TResponse> processor, Func<QueryOperationContext, Task<TResponse>> query) =>
            processor.ExecuteAsync(QueryDecorator<TResponse>.Decorate(query));

        #endregion

        #region [====== ExecuteAsync (2) ======]

        /// <summary>
        /// Executes the specified <paramref name="query"/>.
        /// </summary>
        /// <param name="processor">Processor that will execute the specified <paramref name="query"/>.</param>
        /// <param name="query">Query to execute.</param>
        /// <param name="request">Request message of the query.</param>        
        /// <exception cref="ArgumentNullException">
        /// <paramref name="query"/> is <c>null</c>.
        /// </exception>
        public static Task ExecuteAsync<TRequest, TResponse>(this IQueryProcessor<TRequest, TResponse> processor, Func<TRequest, QueryOperationContext, TResponse> query, TRequest request) =>
            processor.ExecuteAsync(QueryDecorator<TRequest, TResponse>.Decorate(query), request);

        /// <summary>
        /// Executes the specified <paramref name="query"/>.
        /// </summary>
        /// <param name="processor">Processor that will execute the specified <paramref name="query"/>.</param>
        /// <param name="query">Query to execute.</param>
        /// <param name="request">Request message of the query.</param>        
        /// <exception cref="ArgumentNullException">
        /// <paramref name="query"/> is <c>null</c>.
        /// </exception>
        public static Task ExecuteAsync<TRequest, TResponse>(this IQueryProcessor<TRequest, TResponse> processor, Func<TRequest, QueryOperationContext, Task<TResponse>> query, TRequest request) =>
            processor.ExecuteAsync(QueryDecorator<TRequest, TResponse>.Decorate(query), request);

        #endregion
    }
}
