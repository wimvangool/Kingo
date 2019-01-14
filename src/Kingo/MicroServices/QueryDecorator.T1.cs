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
    /// <typeparam name="TResponse">Type of the response message.</typeparam>
    public sealed class QueryDecorator<TResponse> : Query<TResponse>
    {
        #region [====== ExecuteAsyncMethod ======]

        private sealed class ExecuteAsyncMethod : MessageHandlerOrQueryMethod<TResponse>
        {
            private readonly MethodAttributeProvider _attributeProvider;
            private readonly IQuery<TResponse> _query;
            private readonly QueryContext _context;

            public ExecuteAsyncMethod(IQuery<TResponse> query, QueryContext context, MethodAttributeProvider attributeProvider)
            {
                _attributeProvider = attributeProvider;
                _query = query;
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
                new ExecuteAsyncResult<TResponse>(await _query.ExecuteAsync(_context).ConfigureAwait(false));

            public override string ToString() =>
                $"{nameof(_query.ExecuteAsync)}({nameof(QueryContext)})";
        }

        #endregion

        private readonly TypeAttributeProvider _attributeProvider;
        private readonly ExecuteAsyncMethod _method;
        
        internal QueryDecorator(IQuery<TResponse> query, QueryContext context)
        {
            _attributeProvider = new TypeAttributeProvider(query.GetType());
            _method = new ExecuteAsyncMethod(query, context, MethodAttributeProvider.FromQuery(query));           
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

        private sealed class QueryDelegate<T> : IQuery<T>
        {
            private readonly Func<QueryContext, Task<T>> _query;

            public QueryDelegate(Func<QueryContext, Task<T>> query)
            {
                _query = query;
            }

            public Task<T> ExecuteAsync(QueryContext context) =>
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
        public static IQuery<TResponse> Decorate(Func<QueryContext, TResponse> query)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return Decorate(context =>
            {
                return AsyncMethod.Run(() => query.Invoke(context));
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
        public static IQuery<TResponse> Decorate(Func<QueryContext, Task<TResponse>> query) =>
            query == null ? null : new QueryDelegate<TResponse>(query);

        #endregion
    }
}
