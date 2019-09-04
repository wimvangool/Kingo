using System;
using System.Threading.Tasks;
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

        private sealed class MessageHandlerAction : IMessageHandler<TMessage>, IEquatable<MessageHandlerAction>
        {
            private readonly Action<TMessage, MessageHandlerOperationContext> _messageHandler;

            public MessageHandlerAction(Action<TMessage, MessageHandlerOperationContext> messageHandler)
            {
                _messageHandler = messageHandler ?? throw new ArgumentNullException(nameof(messageHandler));
            }

            public override bool Equals(object obj) =>
                Equals(obj as MessageHandlerAction);

            public bool Equals(MessageHandlerAction other)
            {
                if (ReferenceEquals(other, null))
                {
                    return false;
                }
                if (ReferenceEquals(other, this))
                {
                    return true;
                }
                return Equals(_messageHandler, other._messageHandler);
            }

            public override int GetHashCode() =>
                _messageHandler.GetHashCode();

            public override string ToString() =>
                _messageHandler.GetType().FriendlyName();

            public Task HandleAsync(TMessage message, MessageHandlerOperationContext context) =>
                AsyncMethod.Run(() => _messageHandler.Invoke(message, context));            
        }

        /// <summary>
        /// Wraps the specified delegate into a <see cref="IMessageHandler{T}" /> instance.
        /// </summary>
        /// <param name="messageHandler">The delegate to wrap.</param>
        /// <returns>
        /// A <see cref="IMessageHandler{TMessage}"/> that wraps the specified <paramref name="messageHandler"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="messageHandler"/> is <c>null</c>.
        /// </exception>
        public static IMessageHandler<TMessage> Decorate(Action<TMessage, MessageHandlerOperationContext> messageHandler) =>
            new MessageHandlerAction(messageHandler);

        #endregion

        #region [====== Decorate (Func) ======]

        private sealed class MessageHandlerFunc : IMessageHandler<TMessage>, IEquatable<MessageHandlerFunc>
        {
            private readonly Func<TMessage, MessageHandlerOperationContext, Task> _messageHandler;

            public MessageHandlerFunc(Func<TMessage, MessageHandlerOperationContext, Task> messageHandler)
            {
                _messageHandler = messageHandler ?? throw new ArgumentNullException(nameof(messageHandler));
            }

            public override bool Equals(object obj) =>
                Equals(obj as MessageHandlerFunc);

            public bool Equals(MessageHandlerFunc other)
            {
                if (ReferenceEquals(other, null))
                {
                    return false;
                }
                if (ReferenceEquals(other, this))
                {
                    return true;
                }
                return Equals(_messageHandler, other._messageHandler);
            }

            public override int GetHashCode() =>
                _messageHandler.GetHashCode();

            public override string ToString() =>
                _messageHandler.GetType().FriendlyName();

            public Task HandleAsync(TMessage message, MessageHandlerOperationContext context) =>
                _messageHandler.Invoke(message, context);
        }

        /// <summary>
        /// Wraps the specified delegate into a <see cref="IMessageHandler{T}" /> instance.
        /// </summary>
        /// <param name="messageHandler">The delegate to wrap.</param>
        /// <returns>
        /// A <see cref="IMessageHandler{TMessage}"/> that wraps the specified <paramref name="messageHandler"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="messageHandler"/> is <c>null</c>.
        /// </exception>
        public static IMessageHandler<TMessage> Decorate(Func<TMessage, MessageHandlerOperationContext, Task> messageHandler) =>
            new MessageHandlerFunc(messageHandler);

        #endregion
    }
}
