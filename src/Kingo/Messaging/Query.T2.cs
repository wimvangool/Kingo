using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kingo.Threading;

namespace Kingo.Messaging
{
    /// <summary>
    /// Represents a <see cref="IQuery{T, S}" /> instance that is able to provide access to its own attributes.
    /// </summary>
    /// <typeparam name="TMessageIn">Type of the request that is executed by this query.</typeparam>
    /// <typeparam name="TMessageOut">Type of the response that is returned by this query.</typeparam>
    public abstract class Query<TMessageIn, TMessageOut> : MessageHandlerOrQuery, IQuery<TMessageIn, TMessageOut>
    {
        internal Query(ITypeAttributeProvider typeAttributeProvider, IMethodAttributeProvider methodAttributeProvider) :
            base(typeAttributeProvider, methodAttributeProvider) { }

        async Task<TMessageOut> IQuery<TMessageIn, TMessageOut>.ExecuteAsync(TMessageIn message, IMicroProcessorContext context) =>
            (await ExecuteAsync(message, context)).Message;

        /// <inheritdoc />
        public abstract Task<ExecuteAsyncResult<TMessageOut>> ExecuteAsync(TMessageIn message, IMicroProcessorContext context);                

        #region [====== Delegate wrapping ======]

        private sealed class QueryDelegate<TIn, TOut> : IQuery<TIn, TOut>
        {
            private readonly Func<TIn, IMicroProcessorContext, Task<TOut>> _query;

            public QueryDelegate(Func<TIn, IMicroProcessorContext, Task<TOut>> query)
            {
                _query = query;
            }

            public Task<TOut> ExecuteAsync(TIn message, IMicroProcessorContext context) =>
             _query.Invoke(message, context);
        }

        /// <summary>
        /// Wraps the specified delegate into a <see cref="IQuery{T, S}" /> instance.
        /// </summary>
        /// <param name="query">The delegate to wrap.</param>
        /// <returns>
        /// <c>null</c> if <paramref name="query"/> is <c>null</c>; otherwise, a <see cref="IQuery{T, S}"/> instance
        /// that wraps the specified <paramref name="query"/>.
        /// </returns>
        public static IQuery<TMessageIn, TMessageOut> FromDelegate(Func<TMessageIn, IMicroProcessorContext, TMessageOut> query)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return FromDelegate((message, context) =>
            {
                return AsyncMethod.RunSynchronously(() => query.Invoke(message, context));
            });
        }

        /// <summary>
        /// Wraps the specified delegate into a <see cref="IQuery{T, S}" /> instance.
        /// </summary>
        /// <param name="query">The delegate to wrap.</param>
        /// <returns>
        /// <c>null</c> if <paramref name="query"/> is <c>null</c>; otherwise, a <see cref="IQuery{T, S}"/> instance
        /// that wraps the specified <paramref name="query"/>.
        /// </returns>
        public static IQuery<TMessageIn, TMessageOut> FromDelegate(Func<TMessageIn, IMicroProcessorContext, Task<TMessageOut>> query) =>
             query == null ? null : new QueryDelegate<TMessageIn, TMessageOut>(query);

        #endregion
    }
}
