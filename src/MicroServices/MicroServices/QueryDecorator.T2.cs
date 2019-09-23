using System;
using System.Threading.Tasks;
using Kingo.Reflection;
using Kingo.Threading;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Represents a decorator of queries.
    /// </summary>
    /// <typeparam name="TRequest">Type of the request of the query.</typeparam>
    /// <typeparam name="TResponse">Type of the response of the query.</typeparam>
    public class QueryDecorator<TRequest, TResponse> : IQuery<TRequest, TResponse>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QueryDecorator{TRequest, TResponse}" /> class.
        /// </summary>
        /// <param name="query">The query to decorate.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="query"/> is <c>null</c>.
        /// </exception>
        public QueryDecorator(IQuery<TRequest, TResponse> query)
        {
            Query = query ?? throw new ArgumentNullException(nameof(query));
        }

        /// <summary>
        /// The query to decorate.
        /// </summary>
        protected IQuery<TRequest, TResponse> Query
        {
            get;
        }

        /// <inheritdoc />
        public virtual Task<TResponse> ExecuteAsync(TRequest message, IQueryOperationContext context) =>
            Query.ExecuteAsync(message, context);

        /// <inheritdoc />
        public override string ToString() =>
            Query.ToString();

        #region [====== Decorate (TResponse) ======]

        private sealed class QueryFunc : IQuery<TRequest, TResponse>
        {
            private readonly Func<TRequest, IQueryOperationContext, TResponse> _query;

            public QueryFunc(Func<TRequest, IQueryOperationContext, TResponse> query)
            {
                _query = query ?? throw new ArgumentNullException(nameof(query));
            }

            public override string ToString() =>
                _query.GetType().FriendlyName();

            public Task<TResponse> ExecuteAsync(TRequest message, IQueryOperationContext context) =>
                AsyncMethod.Run(() => _query.Invoke(message, context));
        }

        /// <summary>
        /// Wraps the specified delegate into a <see cref="IQuery{T, S}" /> instance.
        /// </summary>
        /// <param name="query">The delegate to wrap.</param>
        /// <returns>
        /// A <see cref="IQuery{T, S}"/> that wraps the specified <paramref name="query"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="query"/> is <c>null</c>.
        /// </exception>
        public static IQuery<TRequest, TResponse> Decorate(Func<TRequest, IQueryOperationContext, TResponse> query) =>
            new QueryFunc(query);

        #endregion

        #region [====== Decorate (Task<TRequest, TResponse>) ======]

        private sealed class QueryFuncAsync : IQuery<TRequest, TResponse>
        {
            private readonly Func<TRequest, IQueryOperationContext, Task<TResponse>> _query;

            public QueryFuncAsync(Func<TRequest, IQueryOperationContext, Task<TResponse>> query)
            {
                _query = query ?? throw new ArgumentNullException(nameof(query));
            }

            public override string ToString() =>
                _query.GetType().FriendlyName();

            public Task<TResponse> ExecuteAsync(TRequest message, IQueryOperationContext context) =>
                _query.Invoke(message, context);
        }

        /// <summary>
        /// Wraps the specified delegate into a <see cref="IQuery{T, S}" /> instance.
        /// </summary>
        /// <param name="query">The delegate to wrap.</param>
        /// <returns>
        /// A <see cref="IQuery{T, S}"/> that wraps the specified <paramref name="query"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="query"/> is <c>null</c>.
        /// </exception>
        public static IQuery<TRequest, TResponse> Decorate(Func<TRequest, IQueryOperationContext, Task<TResponse>> query) =>
            new QueryFuncAsync(query);

        #endregion
    }
}
