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
    /// <typeparam name="TMessageIn">Type of the request-message.</typeparam>
    /// <typeparam name="TMessageOut">Type of the response-message.</typeparam>
    public sealed class QueryDecorator<TMessageIn, TMessageOut> : Query<TMessageOut>
    {
        #region [====== ExecuteAsyncMethod ======]

        private sealed class ExecuteAsyncMethod : MessageHandlerOrQueryMethod<TMessageOut>
        {
            private readonly MethodAttributeProvider _attributeProvider;
            private readonly IQuery<TMessageIn, TMessageOut> _query;
            private readonly TMessageIn _message;
            private readonly QueryContext _context;

            public ExecuteAsyncMethod(IQuery<TMessageIn, TMessageOut> query, TMessageIn message, QueryContext context, MethodAttributeProvider attributeProvider)
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

            public override async Task<InvokeAsyncResult<TMessageOut>> InvokeAsync() =>
                new ExecuteAsyncResult<TMessageOut>(await _query.ExecuteAsync(_message, _context));
        }

        #endregion

        private readonly TypeAttributeProvider _attributeProvider;
        private readonly ExecuteAsyncMethod _method;
                
        internal QueryDecorator(IQuery<TMessageIn, TMessageOut> query, TMessageIn message, QueryContext context)            
        {
            _attributeProvider = new TypeAttributeProvider(query.GetType());
            _method = new ExecuteAsyncMethod(query, message, context, MicroServices.MethodAttributeProvider.FromQuery(query));
        }

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

        #region [====== IMessageHandlerOrQuery<TMessageOut> ======]

        /// <inheritdoc />
        public override QueryContext Context =>
            _method.Context;

        /// <inheritdoc />
        public override MessageHandlerOrQueryMethod<TMessageOut> Method =>
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
        public static IQuery<TMessageIn, TMessageOut> Decorate(Func<TMessageIn, QueryContext, TMessageOut> query)
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
        public static IQuery<TMessageIn, TMessageOut> Decorate(Func<TMessageIn, QueryContext, Task<TMessageOut>> query) =>
            query == null ? null : new QueryDelegate<TMessageIn, TMessageOut>(query);

        #endregion
    }
}
