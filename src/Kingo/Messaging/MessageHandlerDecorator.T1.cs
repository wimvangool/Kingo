using System;
using System.Threading.Tasks;
using Kingo.Threading;

namespace Kingo.Messaging
{    
    /// <summary>
    /// Represents a decorator of message handlers or message handler delegates that provides access to its declared attributes, if present.
    /// </summary>
    /// <typeparam name="TMessage">Type of the message to handle.</typeparam>
    public sealed class MessageHandlerDecorator<TMessage> : MessageHandler
    {        
        private readonly IMessageHandler<TMessage> _handler;
        private readonly TMessage _message;
        private readonly TypeAttributeProvider _typeAttributeProvider;
        private readonly MethodAttributeProvider _methodAttributeProvider;
        
        internal MessageHandlerDecorator(IMessageHandler<TMessage> handler, TMessage message) :
            this(handler, message, new TypeAttributeProvider(handler.GetType()), Messaging.MethodAttributeProvider.FromMessageHandler(handler)) { }

        internal MessageHandlerDecorator(IMessageHandler<TMessage> handler, TMessage message, Type handlerType, Type interfaceType) :
            this(handler, message, new TypeAttributeProvider(handlerType), Messaging.MethodAttributeProvider.FromMessageHandler(handlerType, interfaceType)) { }

        private MessageHandlerDecorator(IMessageHandler<TMessage> handler, TMessage message, TypeAttributeProvider typeAttributeProvider, MethodAttributeProvider methodAttributeProvider)
        {
            _handler = handler;
            _message = message;
            _typeAttributeProvider = typeAttributeProvider;
            _methodAttributeProvider = methodAttributeProvider;
        }

        /// <inheritdoc />
        protected override ITypeAttributeProvider TypeAttributeProvider =>
            _typeAttributeProvider;

        /// <inheritdoc />
        protected override IMethodAttributeProvider MethodAttributeProvider =>
            _methodAttributeProvider;

        /// <inheritdoc />
        public override async Task<InvokeAsyncResult<IMessageStream>> InvokeAsync(MicroProcessorContext context)
        {
            await _handler.HandleAsync(_message, context);

            return Yield(context);
        }

        /// <inheritdoc />
        public override string ToString() =>
            MicroProcessorPipeline.ToString(_handler);

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
        public static IMessageHandler<TMessage> Decorate(Action<TMessage, IMicroProcessorContext> handler)
        {
            if (handler == null)
            {
                return null;
            }
            return Decorate((message, context) =>
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
        public static IMessageHandler<TMessage> Decorate(Func<TMessage, IMicroProcessorContext, Task> handler) =>
            handler == null ? null : new MessageHandlerDelegate<TMessage>(handler);

        #endregion
    }
}
