using System;
using System.Threading.Tasks;
using Kingo.Threading;

namespace Kingo.Messaging
{
    /// <summary>
    /// Represents a decorator of queries or query delegates that provides access to its declared attributes, if present.
    /// </summary>
    /// <typeparam name="TMessageIn">Type of the request-message.</typeparam>
    /// <typeparam name="TMessageOut">Type of the response-message.</typeparam>
    public sealed class QueryDecorator<TMessageIn, TMessageOut> : Query<TMessageOut>
    {        
        private readonly IQuery<TMessageIn, TMessageOut> _query;
        private readonly TMessageIn _message;
        private readonly TypeAttributeProvider _typeAttributeProvider;
        private readonly MethodAttributeProvider _methodAttributeProvider;
        
        public QueryDecorator(IQuery<TMessageIn, TMessageOut> query, TMessageIn message)            
        {            
            _query = query;
            _message = message;
            _typeAttributeProvider = new TypeAttributeProvider(query.GetType());
            _methodAttributeProvider = Messaging.MethodAttributeProvider.FromQuery(query);
        }

        protected override ITypeAttributeProvider TypeAttributeProvider =>
            _typeAttributeProvider;

        protected override IMethodAttributeProvider MethodAttributeProvider =>
            _methodAttributeProvider;

        public override async Task<InvokeAsyncResult<TMessageOut>> InvokeAsync(MicroProcessorContext context) =>
            new ExecuteAsyncResult<TMessageOut>(await _query.ExecuteAsync(_message, context));

        public override string ToString() =>
            MicroProcessorPipeline.ToString(_query);

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
        public static IQuery<TMessageIn, TMessageOut> Decorate(Func<TMessageIn, IMicroProcessorContext, TMessageOut> query)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return Decorate((message, context) =>
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
        public static IQuery<TMessageIn, TMessageOut> Decorate(Func<TMessageIn, IMicroProcessorContext, Task<TMessageOut>> query) =>
            query == null ? null : new QueryDelegate<TMessageIn, TMessageOut>(query);

        #endregion
    }
}
