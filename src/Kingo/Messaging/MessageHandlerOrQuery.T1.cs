using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kingo.Messaging
{
    /// <summary>
    /// Represents a <see cref="IMessageHandler{T}" />, <see cref="IQuery{T}" /> or <see cref="IQuery{T, S} "/>.
    /// </summary>
    /// <typeparam name="TResult">Type of the resulting operation.</typeparam>
    public abstract class MessageHandlerOrQuery<TResult> : ITypeAttributeProvider, IMethodAttributeProvider
    {
        #region [====== ITypeAttributeProvider ======]

        /// <summary>
        /// Returns the provider that is used to access all attributes declared on the <see cref="IMessageHandler{T}" />.
        /// </summary>
        protected abstract ITypeAttributeProvider TypeAttributeProvider
        {
            get;
        }

        /// <inheritdoc />
        public bool TryGetTypeAttributeOfType<TAttribute>(out TAttribute attribute) where TAttribute : class =>
            TypeAttributeProvider.TryGetTypeAttributeOfType(out attribute);

        /// <inheritdoc />
        public IEnumerable<TAttribute> GetTypeAttributesOfType<TAttribute>() where TAttribute : class =>
            TypeAttributeProvider.GetTypeAttributesOfType<TAttribute>();

        #endregion

        #region [====== IMethodAttributeProvider ======]

        /// <summary>
        /// Returns the provider that is used to access all attributes declared on the <see cref="IMessageHandler{T}.HandleAsync(T, IMicroProcessorContext)" /> method.
        /// </summary>
        protected abstract IMethodAttributeProvider MethodAttributeProvider
        {
            get;
        }

        /// <inheritdoc />
        public bool TryGetMethodAttributeOfType<TAttribute>(out TAttribute attribute) where TAttribute : class =>
            MethodAttributeProvider.TryGetMethodAttributeOfType(out attribute);

        /// <inheritdoc />
        public IEnumerable<TAttribute> GetMethodAttributesOfType<TAttribute>() where TAttribute : class =>
            MethodAttributeProvider.GetMethodAttributesOfType<TAttribute>();

        #endregion

        /// <summary>
        /// Processes the current command, event or query asynchronously and returns the result.
        /// </summary>
        /// <param name="context">Context of the <see cref="IMicroProcessor" /> that is currently handling the message.</param>
        /// <returns>The result of the operation.</returns>
        public abstract Task<TResult> HandleMessageOrExecuteQueryAsync(IMicroProcessorContext context);        
    }
}
