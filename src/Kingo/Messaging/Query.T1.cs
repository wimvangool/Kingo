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
    public abstract class Query<TMessageOut> : IQuery<TMessageOut>, ITypeAttributeProvider, IMethodAttributeProvider, IMicroProcessorPipelineComponent
    {
        /// <summary>
        /// Returns the provider that is used to access all attributes declared on the <see cref="IQuery{T}" />.
        /// </summary>
        protected abstract ITypeAttributeProvider TypeAttributeProvider
        {
            get;
        }

        /// <summary>
        /// Returns the provider that is used to access all attributes declared on the <see cref="IQuery{T}.ExecuteAsync(IMicroProcessorContext)" /> method.
        /// </summary>
        protected abstract IMethodAttributeProvider MethodAttributeProvider
        {
            get;
        }

        /// <inheritdoc />
        public bool TryGetTypeAttributeOfType<TAttribute>(out TAttribute attribute) where TAttribute : class =>
            TypeAttributeProvider.TryGetTypeAttributeOfType(out attribute);

        /// <inheritdoc />
        public IEnumerable<TAttribute> GetTypeAttributesOfType<TAttribute>() where TAttribute : class =>
            TypeAttributeProvider.GetTypeAttributesOfType<TAttribute>();

        /// <inheritdoc />
        public bool TryGetMethodAttributeOfType<TAttribute>(out TAttribute attribute) where TAttribute : class =>
            MethodAttributeProvider.TryGetMethodAttributeOfType(out attribute);

        /// <inheritdoc />
        public IEnumerable<TAttribute> GetMethodAttributesOfType<TAttribute>() where TAttribute : class =>
            MethodAttributeProvider.GetMethodAttributesOfType<TAttribute>();

        async Task<TMessageOut> IQuery<TMessageOut>.ExecuteAsync(IMicroProcessorContext context) =>
            (await ExecuteAsync(context)).Message;

        /// <inheritdoc />
        public abstract Task<ExecuteAsyncResult<TMessageOut>> ExecuteAsync(IMicroProcessorContext context);

        /// <inheritdoc />
        public override string ToString() =>
            MicroProcessorPipelineStringBuilder.ToString(this);

        /// <inheritdoc />
        public abstract void Accept(IMicroProcessorPipelineVisitor visitor);

        #region [====== Delegate wrapping ======]

        private sealed class QueryDelegate<T> : IQuery<T>
        {
            private readonly Func<IMicroProcessorContext, Task<T>> _query;

            public QueryDelegate(Func<IMicroProcessorContext, Task<T>> query)
            {
                _query = query;
            }

            public Task<T> ExecuteAsync(IMicroProcessorContext context) => _query.Invoke(context);
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
        public static IQuery<TMessageOut> FromDelegate(Func<IMicroProcessorContext, Task<TMessageOut>> query) => query == null ? null : new QueryDelegate<TMessageOut>(query);

        #endregion
    }
}
