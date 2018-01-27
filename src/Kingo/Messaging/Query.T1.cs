using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kingo.Threading;

namespace Kingo.Messaging
{
    /// <summary>
    /// Represents a <see cref="IQuery{T}" /> instance that is able to provide access to its own attributes.
    /// </summary>
    /// <typeparam name="TMessageOut">Type of the response that is returned by this query.</typeparam>
    public abstract class Query<TMessageOut> : MessageHandlerOrQuery, IQuery<TMessageOut>
    {        
        internal Query(ITypeAttributeProvider typeAttributeProvider, IMethodAttributeProvider methodAttributeProvider) :
            base(typeAttributeProvider, methodAttributeProvider) { }

        async Task<TMessageOut> IQuery<TMessageOut>.ExecuteAsync(IMicroProcessorContext context) =>
            (await ExecuteAsync(context)).Message;

        /// <inheritdoc />
        public abstract Task<ExecuteAsyncResult<TMessageOut>> ExecuteAsync(IMicroProcessorContext context);

        #region [====== Delegate wrapping ======]

        private sealed class QueryDelegate<T> : IQuery<T>
        {
            private readonly Func<IMicroProcessorContext, Task<T>> _query;

            public QueryDelegate(Func<IMicroProcessorContext, Task<T>> query)
            {
                _query = query;
            }

            public Task<T> ExecuteAsync(IMicroProcessorContext context) =>
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
        public static IQuery<TMessageOut> FromDelegate(Func<IMicroProcessorContext, TMessageOut> query)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return FromDelegate(context =>
            {
                return AsyncMethod.RunSynchronously(() => query.Invoke(context));
            });
        }

        /// <summary>
        /// Wraps the specified delegate into a <see cref="IQuery{T}" /> instance.
        /// </summary>
        /// <param name="query">The delegate to wrap.</param>
        /// <returns>
        /// <c>null</c> if <paramref name="query"/> is <c>null</c>; otherwise, a <see cref="IQuery{T}"/> instance
        /// that wraps the specified <paramref name="query"/>.
        /// </returns>
        public static IQuery<TMessageOut> FromDelegate(Func<IMicroProcessorContext, Task<TMessageOut>> query) =>
             query == null ? null : new QueryDelegate<TMessageOut>(query);

        #endregion
    }
}
