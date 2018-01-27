using System;
using System.Threading.Tasks;
using Kingo.Threading;

namespace Kingo.Messaging
{
    /// <summary>
    /// Represents a <see cref="IMessageHandler{T}" /> instance that is able to provide access to its own attributes.
    /// </summary>
    /// <typeparam name="TMessage">Type of the message that is handled by this handler.</typeparam>
    public abstract class MessageHandler<TMessage> : MessageHandlerOrQuery, IMessageHandler<TMessage>
    {        
        internal MessageHandler(ITypeAttributeProvider typeAttributeProvider, IMethodAttributeProvider methodAttributeProvider) :
            base(typeAttributeProvider, methodAttributeProvider) { }

        Task IMessageHandler<TMessage>.HandleAsync(TMessage message, IMicroProcessorContext context) =>
            HandleAsync(message, context);

        /// <inheritdoc />
        public abstract Task<HandleAsyncResult> HandleAsync(TMessage message, IMicroProcessorContext context);                              

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
