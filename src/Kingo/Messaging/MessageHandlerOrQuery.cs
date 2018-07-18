using System.Collections.Generic;

namespace Kingo.Messaging
{
    /// <summary>
    /// Serves as a base class for any message handler or query that is able to process a message and provides access
    /// to its own attributes.
    /// </summary>
    public abstract class MessageHandlerOrQuery : ITypeAttributeProvider, IMethodAttributeProvider
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
    }
}
