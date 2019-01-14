using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Kingo.Threading;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Represents a decorator of queries or query delegates that provides access to its declared attributes, if present.
    /// </summary>
    /// <typeparam name="TRequest">Type of the request-message.</typeparam>
    /// <typeparam name="TResponse">Type of the response-message.</typeparam>
    public sealed class QueryDecorator<TRequest, TResponse> : Query<TResponse>
    {
        #region [====== ExecuteAsyncMethod ======]

        private sealed class ExecuteAsyncMethod : MessageHandlerOrQueryMethod<TResponse>
        {
            private readonly MethodAttributeProvider _attributeProvider;
            private readonly IQuery<TRequest, TResponse> _query;
            private readonly TRequest _message;
            private readonly QueryContext _context;

            public ExecuteAsyncMethod(IQuery<TRequest, TResponse> query, TRequest message, QueryContext context, MethodAttributeProvider attributeProvider)
            {
                _attributeProvider = attributeProvider;
                _query = query;
                _message = message;
                _context = context;
            }

            public override MethodInfo Info =>
                _attributeProvider.Target;

            public override bool TryGetAttributeOfType<TAttribute>(out TAttribute attribute) =>
                _attributeProvider.TryGetAttributeOfType(out attribute);

            public override IEnumerable<TAttribute> GetAttributesOfType<TAttribute>() =>
                _attributeProvider.GetAttributesOfType<TAttribute>();

            public QueryContext Context =>
                _context;

            public override async Task<InvokeAsyncResult<TResponse>> InvokeAsync() =>
                new ExecuteAsyncResult<TResponse>(await _query.ExecuteAsync(_message, _context).ConfigureAwait(false));
            
            public override string ToString() =>
                $"{nameof(_query.ExecuteAsync)}({typeof(TRequest).FriendlyName()}, {nameof(QueryContext)})";
        }

        #endregion

        private readonly TypeAttributeProvider _attributeProvider;
        private readonly ExecuteAsyncMethod _method;
                
        internal QueryDecorator(IQuery<TRequest, TResponse> query, TRequest message, QueryContext context)            
        {
            _attributeProvider = new TypeAttributeProvider(query.GetType());
            _method = new ExecuteAsyncMethod(query, message, context, MicroServices.MethodAttributeProvider.FromQuery(query));
        }

        /// <inheritdoc />
        public override string ToString() =>
            Type.FriendlyName();

        #region [====== IAttributeProvider<Type> ======]

        /// <inheritdoc />
        public override Type Type =>
            _attributeProvider.Target;

        /// <inheritdoc />
        public override bool TryGetAttributeOfType<TAttribute>(out TAttribute attribute) =>
            _attributeProvider.TryGetAttributeOfType(out attribute);

        /// <inheritdoc />
        public override IEnumerable<TAttribute> GetAttributesOfType<TAttribute>() =>
            _attributeProvider.GetAttributesOfType<TAttribute>();

        #endregion

        #region [====== IMessageHandlerOrQuery<TResponse> ======]

        /// <inheritdoc />
        public override QueryContext Context =>
            _method.Context;

        /// <inheritdoc />
        public override MessageHandlerOrQueryMethod<TResponse> Method =>
            _method;

        #endregion    

        #region [====== Delegate wrapping ======]

        private sealed class QueryDelegate<TIn, TOut> : IQuery<TIn, TOut>
        {
            private readonly Func<TIn, QueryContext, Task<TOut>> _query;

            public QueryDelegate(Func<TIn, QueryContext, Task<TOut>> query)
            {
                _query = query;
            }

            public Task<TOut> ExecuteAsync(TIn message, QueryContext context) =>
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
        public static IQuery<TRequest, TResponse> Decorate(Func<TRequest, QueryContext, TResponse> query)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return Decorate((message, context) =>
            {
                return AsyncMethod.Run(() => query.Invoke(message, context));
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
        public static IQuery<TRequest, TResponse> Decorate(Func<TRequest, QueryContext, Task<TResponse>> query) =>
            query == null ? null : new QueryDelegate<TRequest, TResponse>(query);

        #endregion
    }
}
