using System;
using System.Threading.Tasks;
using Kingo.Threading;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Represents a decorator of queries.
    /// </summary>
    /// <typeparam name="TResponse">Type of the response of the query.</typeparam>
    public class QueryDecorator<TResponse> : IQuery<TResponse>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QueryDecorator{TResponse}" /> class.
        /// </summary>
        /// <param name="query">The query to decorate.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="query"/> is <c>null</c>.
        /// </exception>
        public QueryDecorator(IQuery<TResponse> query)
        {
            Query = query ?? throw new ArgumentNullException(nameof(query));
        }

        /// <summary>
        /// The query to decorate.
        /// </summary>
        protected IQuery<TResponse> Query
        {
            get;
        }

        /// <inheritdoc />
        public virtual Task<TResponse> ExecuteAsync(QueryOperationContext context) =>
            Query.ExecuteAsync(context);

        /// <inheritdoc />
        public override string ToString() =>
            Query.ToString();

        #region [====== Decorate (TResponse) ======]

        private sealed class QueryFunc : IQuery<TResponse>
        {
            private readonly Func<QueryOperationContext, TResponse> _query;

            public QueryFunc(Func<QueryOperationContext, TResponse> query)
            {
                _query = query;
            }

            public Task<TResponse> ExecuteAsync(QueryOperationContext context) =>
                AsyncMethod.Run(() => _query.Invoke(context));
        }

        /// <summary>
        /// Wraps the specified delegate into a <see cref="IQuery{T}" /> instance.
        /// </summary>
        /// <param name="query">The delegate to wrap.</param>
        /// <returns>
        /// <c>null</c> if <paramref name="query"/> is <c>null</c>; otherwise, a <see cref="IQuery{T}"/> instance
        /// that wraps the specified <paramref name="query"/>.
        /// </returns>
        public static IQuery<TResponse> Decorate(Func<QueryOperationContext, TResponse> query) =>
            query == null ? null : new QueryFunc(query);

        #endregion

        #region [====== Decorate (Task<TResponse>) ======]

        private sealed class QueryFuncAsync : IQuery<TResponse>
        {
            private readonly Func<QueryOperationContext, Task<TResponse>> _query;

            public QueryFuncAsync(Func<QueryOperationContext, Task<TResponse>> query)
            {
                _query = query;
            }

            public Task<TResponse> ExecuteAsync(QueryOperationContext context) =>
                _query.Invoke(context);
        }

        /// <summary>
        /// Wraps the specified delegate into a <see cref="IQuery{T}" /> instance.
        /// </summary>
        /// <param name="query">The delegate to wrap.</param>
        /// <returns>
        /// <c>null</c> if <paramref name="query"/> is <c>null</c>; otherwise, a <see cref="IQuery{T}"/> instance
        /// that wraps the specified <paramref name="query"/>.
        /// </returns>
        public static IQuery<TResponse> Decorate(Func<QueryOperationContext, Task<TResponse>> query) =>
            query == null ? null : new QueryFuncAsync(query);

        #endregion
    }
}
