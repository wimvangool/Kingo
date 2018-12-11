using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Kingo.Threading;

namespace Kingo.MicroServices
{    
    /// <summary>
    /// Represents a decorator of message handlers or message handler delegates that provides access to its declared attributes, if present.
    /// </summary>
    /// <typeparam name="TMessage">Type of the message to handle.</typeparam>
    public sealed class MessageHandlerDecorator<TMessage> : MessageHandler
    {
        #region [====== HandleAsyncMethod ======]

        private sealed class HandleAsyncMethod : MessageHandlerOrQueryMethod<MessageStream>
        {            
            private readonly MethodAttributeProvider _attributeProvider;
            private readonly IMessageHandler<TMessage> _handler;
            private readonly TMessage _message;
            private readonly MessageHandlerContext _context;

            public HandleAsyncMethod(IMessageHandler<TMessage> handler, TMessage message, MessageHandlerContext context, MethodAttributeProvider attributeProvider)
            {
                _attributeProvider = attributeProvider;
                _handler = handler;
                _message = message;
                _context = context;
            }

            public override MethodInfo Info =>
                _attributeProvider.Target;

            public override bool TryGetAttributeOfType<TAttribute>(out TAttribute attribute) =>
                _attributeProvider.TryGetAttributeOfType(out attribute);

            public override IEnumerable<TAttribute> GetAttributesOfType<TAttribute>() =>
                _attributeProvider.GetAttributesOfType<TAttribute>();

            public MessageHandlerContext Context =>
                _context;

            public override async Task<InvokeAsyncResult<MessageStream>> InvokeAsync()
            {
                await _handler.HandleAsync(_message, _context);
                return new HandleAsyncResult(_context.EventBus);
            }
        }

        #endregion

        private readonly TypeAttributeProvider _attributeProvider;
        private readonly HandleAsyncMethod _method;
        
        internal MessageHandlerDecorator(IMessageHandler<TMessage> handler, TMessage message, MessageHandlerContext context) :
            this(handler, message, context, new TypeAttributeProvider(handler.GetType()), MethodAttributeProvider.FromMessageHandler(handler)) { }

        [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature)]
        internal MessageHandlerDecorator(IMessageHandler<TMessage> handler, TMessage message, MessageHandlerContext context, Type handlerType, Type interfaceType) :
            this(handler, message, context, new TypeAttributeProvider(handlerType), MethodAttributeProvider.FromMessageHandler(handlerType, interfaceType)) { }

        private MessageHandlerDecorator(IMessageHandler<TMessage> handler, TMessage message, MessageHandlerContext context, TypeAttributeProvider typeAttributeProvider, MethodAttributeProvider methodAttributeProvider)
        {
            _attributeProvider = typeAttributeProvider;
            _method = new HandleAsyncMethod(handler, message, context, methodAttributeProvider);
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

        #region [====== IMessageHandlerOrQuery<MessageStream> ======]

        /// <inheritdoc />
        public override MessageHandlerContext Context =>
            _method.Context;

        /// <inheritdoc />
        public override MessageHandlerOrQueryMethod<MessageStream> Method
        {
            get;
        }        

        #endregion        

        #region [====== Delegate wrapping ======]

        private sealed class MessageHandlerDelegate<T> : IMessageHandler<T>
        {
            private readonly Func<T, MessageHandlerContext, Task> _handler;

            public MessageHandlerDelegate(Func<T, MessageHandlerContext, Task> handler)
            {
                _handler = handler;
            }

            public Task HandleAsync(T message, MessageHandlerContext context) =>
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
        public static IMessageHandler<TMessage> Decorate(Action<TMessage, MessageHandlerContext> handler)
        {
            if (handler == null)
            {
                return null;
            }
            return Decorate((message, context) => AsyncMethod.Run(() => handler.Invoke(message, context)));
        }

        /// <summary>
        /// Wraps the specified delegate into a <see cref="IMessageHandler{T}" /> instance.
        /// </summary>
        /// <param name="handler">The delegate to wrap.</param>
        /// <returns>
        /// <c>null</c> if <paramref name="handler"/> is <c>null</c>; otherwise, a <see cref="IMessageHandler"/> instance
        /// that wraps the specified <paramref name="handler"/>.
        /// </returns>
        public static IMessageHandler<TMessage> Decorate(Func<TMessage, MessageHandlerContext, Task> handler) =>
            handler == null ? null : new MessageHandlerDelegate<TMessage>(handler);

        #endregion
    }
}
