using System;
using System.Threading.Tasks;
using Kingo.Threading;

namespace Kingo.Messaging
{
    /// <summary>
    /// Represents a decorator of queries or query delegates that provides access to its declared attributes, if present.
    /// </summary>
    /// <typeparam name="TMessageOut">Type of the response message.</typeparam>
    public sealed class QueryDecorator<TMessageOut> : Query<TMessageOut>
    {        
        private readonly IQuery<TMessageOut> _query;
        private readonly TypeAttributeProvider _typeAttributeProvider;
        private readonly MethodAttributeProvider _methodAttributeProvider;
        
        internal QueryDecorator(IQuery<TMessageOut> query)
        {            
            _query = query;
            _typeAttributeProvider = new TypeAttributeProvider(query.GetType());
            _methodAttributeProvider = Messaging.MethodAttributeProvider.FromQuery(query);
        }

        protected override ITypeAttributeProvider TypeAttributeProvider =>
            _typeAttributeProvider;

        protected override IMethodAttributeProvider MethodAttributeProvider =>
            _methodAttributeProvider;

        public override async Task<InvokeAsyncResult<TMessageOut>> InvokeAsync(MicroProcessorContext context) =>
            new ExecuteAsyncResult<TMessageOut>(await _query.ExecuteAsync(context));

        public override string ToString() =>
            MicroProcessorPipeline.ToString(_query);

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
        public static IQuery<TMessageOut> Decorate(Func<IMicroProcessorContext, TMessageOut> query)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return Decorate(context =>
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
        public static IQuery<TMessageOut> Decorate(Func<IMicroProcessorContext, Task<TMessageOut>> query) =>
            query == null ? null : new QueryDelegate<TMessageOut>(query);

        #endregion
    }
}
