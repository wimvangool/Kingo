using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Kingo.Reflection;
using Kingo.Threading;

namespace Kingo.MicroServices
{    
    /// <summary>
    /// Represents a decorator of message handlers.
    /// </summary>
    /// <typeparam name="TMessage">Type of the message to handle.</typeparam>
    public class MessageHandlerDecorator<TMessage> : IMessageHandler<TMessage>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageHandlerDecorator{T}" /> class.
        /// </summary>
        /// <param name="messageHandler">The message handler to decorate.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="messageHandler"/> is <c>null</c>.
        /// </exception>
        public MessageHandlerDecorator(IMessageHandler<TMessage> messageHandler)
        {
            MessageHandler = messageHandler ?? throw new ArgumentNullException(nameof(messageHandler));
        }

        /// <summary>
        /// The message handler to decorate.
        /// </summary>
        protected IMessageHandler<TMessage> MessageHandler
        {
            get;
        }

        /// <inheritdoc />
        public virtual Task HandleAsync(TMessage message, MessageHandlerOperationContext context) =>
            MessageHandler.HandleAsync(message, context);

        /// <inheritdoc />
        public override string ToString() =>
            MessageHandler.ToString();

        #region [====== Decorate (Action) ======]      

        private sealed class MessageHandlerAction : IMessageHandler<TMessage>
        {
            private readonly Action<TMessage, MessageHandlerOperationContext> _messageHandler;

            public MessageHandlerAction(Action<TMessage, MessageHandlerOperationContext> messageHandler)
            {
                _messageHandler = messageHandler;
            }

            public Task HandleAsync(TMessage message, MessageHandlerOperationContext context) =>
                AsyncMethod.Run(() => _messageHandler.Invoke(message, context));
        }

        /// <summary>
        /// Wraps the specified delegate into a <see cref="IMessageHandler{T}" /> instance.
        /// </summary>
        /// <param name="messageHandler">The delegate to wrap.</param>
        /// <returns>
        /// <c>null</c> if <paramref name="messageHandler"/> is <c>null</c>; otherwise, a <see cref="IMessageHandler{T}"/> instance
        /// that wraps the specified <paramref name="messageHandler"/>.
        /// </returns>
        public static IMessageHandler<TMessage> Decorate(Action<TMessage, MessageHandlerOperationContext> messageHandler) =>
            messageHandler == null ? null : new MessageHandlerAction(messageHandler);

        #endregion

        #region [====== Decorate (Func) ======]

        private sealed class MessageHandlerFunc : IMessageHandler<TMessage>
        {
            private readonly Func<TMessage, MessageHandlerOperationContext, Task> _messageHandler;

            public MessageHandlerFunc(Func<TMessage, MessageHandlerOperationContext, Task> messageHandler)
            {
                _messageHandler = messageHandler;
            }

            public Task HandleAsync(TMessage message, MessageHandlerOperationContext context) =>
                _messageHandler.Invoke(message, context);
        }

        /// <summary>
        /// Wraps the specified delegate into a <see cref="IMessageHandler{T}" /> instance.
        /// </summary>
        /// <param name="messageHandler">The delegate to wrap.</param>
        /// <returns>
        /// <c>null</c> if <paramref name="messageHandler"/> is <c>null</c>; otherwise, a <see cref="IMessageHandler{T}"/> instance
        /// that wraps the specified <paramref name="messageHandler"/>.
        /// </returns>
        public static IMessageHandler<TMessage> Decorate(Func<TMessage, MessageHandlerOperationContext, Task> messageHandler) =>
            messageHandler == null ? null : new MessageHandlerFunc(messageHandler);

        #endregion
    }
}
