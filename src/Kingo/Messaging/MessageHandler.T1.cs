using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kingo.Threading;

namespace Kingo.Messaging
{
    /// <summary>
    /// Represents a <see cref="IMessageHandler{T}" /> instance that is able to provide access to its own attributes.
    /// </summary>
    /// <typeparam name="TMessage">Type of the message that is handled by this handler.</typeparam>
    public abstract class MessageHandler<TMessage> : IMessageHandler<TMessage>, ITypeAttributeProvider, IMethodAttributeProvider, IMicroProcessorPipelineComponent
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

        Task IMessageHandler<TMessage>.HandleAsync(TMessage message, IMicroProcessorContext context) =>
            HandleAsync(message, context);

        /// <inheritdoc />
        public abstract Task<HandleAsyncResult> HandleAsync(TMessage message, IMicroProcessorContext context);              

        /// <inheritdoc />
        public override string ToString() =>
            MicroProcessorPipelineStringBuilder.ToString(this);

        /// <inheritdoc />
        public abstract void Accept(IMicroProcessorPipelineVisitor visitor);

        #region [====== Delegate wrapping ======]

        private sealed class MessageHandlerDelegate<T> : IMessageHandler<T>
        {
            private readonly Func<T, IMicroProcessorContext, Task> _handler;

            public MessageHandlerDelegate(Func<T, IMicroProcessorContext, Task> handler)
            {
                _handler = handler;
            }

            public Task HandleAsync(T message, IMicroProcessorContext context) =>
                _handler.Invoke(message, context);
        }

        /// <summary>
        /// Wraps the specified delegate into a <see cref="IMessageHandler{T}" /> instance.
        /// </summary>
        /// <param name="handler">The delegate to wrap.</param>
        /// <returns>
        /// <c>null</c> if <paramref name="handler"/> is <c>null</c>; otherwise, a <see cref="IMessageHandler{T}"/> instance
        /// that wraps the specified <paramref name="handler"/>.
        /// </returns>
        public static IMessageHandler<TMessage> FromDelegate(Action<TMessage, IMicroProcessorContext> handler)
        {
            if (handler == null)
            {
                return null;
            }
            return FromDelegate((message, context) =>
            {
                return AsyncMethod.RunSynchronously(() => handler.Invoke(message, context));
            });
        }

        /// <summary>
        /// Wraps the specified delegate into a <see cref="IMessageHandler{T}" /> instance.
        /// </summary>
        /// <param name="handler">The delegate to wrap.</param>
        /// <returns>
        /// <c>null</c> if <paramref name="handler"/> is <c>null</c>; otherwise, a <see cref="IMessageHandler"/> instance
        /// that wraps the specified <paramref name="handler"/>.
        /// </returns>
        public static IMessageHandler<TMessage> FromDelegate(Func<TMessage, IMicroProcessorContext, Task> handler) =>
            handler == null ? null : new MessageHandlerDelegate<TMessage>(handler);

        #endregion
    }
}
