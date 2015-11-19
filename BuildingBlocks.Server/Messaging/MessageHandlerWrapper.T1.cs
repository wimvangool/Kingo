using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kingo.BuildingBlocks.Messaging
{
    /// <summary>
    /// Represents a wrapper of a message and it's handler to serve as a <see cref="IMessageHandler" />
    /// that can be used within a <see cref="IMessageProcessor" />'s pipeline.
    /// </summary>
    /// <typeparam name="TMessage">Type of message to handle.</typeparam>
    public sealed class MessageHandlerWrapper<TMessage> : IMessageHandler where TMessage : class, IMessage<TMessage>
    {
        private readonly TMessage _message;
        private readonly MessageHandlerInstance<TMessage> _handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageHandlerWrapper{TMessage}" /> class.
        /// </summary>
        /// <param name="message">The message to handle.</param>
        /// <param name="handler">The handler to invoke.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> or <paramref name="handler"/> is <c>null</c>.
        /// </exception>
        public MessageHandlerWrapper(TMessage message, IMessageHandler<TMessage> handler)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }
            _message = message;
            _handler = new MessageHandlerInstance<TMessage>(handler);
        }

        internal MessageHandlerWrapper(TMessage message, MessageHandlerInstance<TMessage> handler)
        {
            _message = message;
            _handler = handler;
        }

        /// <inheritdoc />
        public bool TryGetClassAttributeOfType<TAttribute>(out TAttribute attribute) where TAttribute : class
        {
            return _handler.TryGetClassAttributeOfType(out attribute);
        }

        /// <inheritdoc />
        public bool TryGetMethodAttributeOfType<TAttribute>(out TAttribute attribute) where TAttribute : class
        {
            return _handler.TryGetMethodAttributeOfType(out attribute);
        }

        /// <inheritdoc />
        public IEnumerable<TAttribute> GetClassAttributesOfType<TAttribute>() where TAttribute : class
        {
            return _handler.GetClassAttributesOfType<TAttribute>();
        }

        /// <inheritdoc />
        public IEnumerable<TAttribute> GetMethodAttributesOfType<TAttribute>() where TAttribute : class
        {
            return _handler.GetMethodAttributesOfType<TAttribute>();
        }

        /// <inheritdoc />
        public IMessage Message
        {
            get { return _message; }
        }        

        /// <inheritdoc />
        public Task InvokeAsync()
        {
            return _handler.HandleAsync(_message);
        }        
    }
}
