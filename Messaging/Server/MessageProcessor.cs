using System.ComponentModel.Resources;
using System.Threading;

namespace System.ComponentModel.Server
{
    /// <summary>
    /// Represents a handler of arbitrary messages.
    /// </summary>    
    public abstract class MessageProcessor : IMessageProcessor
    {
        private readonly IMessageProcessorBus _domainEventBus;                
        private readonly ThreadLocal<MessagePointer> _currentMessagePointer;                            

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageProcessor" /> class.
        /// </summary>        
        protected MessageProcessor()
        {
            _domainEventBus = new MessageProcessorBus(this);
            _currentMessagePointer = new ThreadLocal<MessagePointer>();
        }        

        /// <inheritdoc />
        public IMessageProcessorBus DomainEventBus
        {
            get { return _domainEventBus; }
        }

        /// <inheritdoc />
        public MessagePointer MessagePointer
        {
            get { return _currentMessagePointer.Value; }
            private set { _currentMessagePointer.Value = value; }
        }

        /// <summary>
        /// Returns the <see cref="MessageHandlerFactory" /> of this processor.
        /// </summary>
        protected abstract MessageHandlerFactory MessageHandlerFactory
        {
            get;
        }

        #region [====== Commands ======]

        /// <inheritdoc />
        public int Execute<TCommand>(TCommand message) where TCommand : class, IRequestMessage<TCommand>
        {
            return Handle(message, message);
        }

        /// <inheritdoc />
        public int Execute<TCommand>(TCommand message, CancellationToken? token) where TCommand : class, IRequestMessage<TCommand>
        {
            return Handle(message, message, token);
        }

        /// <inheritdoc />
        public int Execute<TCommand>(TCommand message, IMessageHandler<TCommand> handler) where TCommand : class, IRequestMessage<TCommand>
        {
            return Handle(message, message, handler);
        }

        /// <inheritdoc />
        public int Execute<TCommand>(TCommand message, IMessageHandler<TCommand> handler, CancellationToken? token) where TCommand : class, IRequestMessage<TCommand>
        {
            return Handle(message, message, handler, token);
        }

        /// <inheritdoc />
        public int Execute<TCommand>(TCommand message, Action<TCommand> handler) where TCommand : class, IRequestMessage<TCommand>
        {
            return Handle(message, message, handler);
        }

        /// <inheritdoc />
        public int Execute<TCommand>(TCommand message, Action<TCommand> handler, CancellationToken? token) where TCommand : class, IRequestMessage<TCommand>
        {
            return Handle(message, message, handler, token);
        }

        #endregion

        #region [====== Events ======]

        /// <inheritdoc />
        public int Handle<TMessage>(TMessage message) where TMessage : class, IMessage<TMessage>
        {
            return Handle(message, null);
        }

        /// <inheritdoc />
        public int Handle<TMessage>(TMessage message, IMessageValidator<TMessage> validator) where TMessage : class, IMessage<TMessage>
        {
            return Handle(message, validator, null as CancellationToken?);
        }

        /// <inheritdoc />
        public virtual int Handle<TMessage>(TMessage message, IMessageValidator<TMessage> validator, CancellationToken? token) where TMessage : class, IMessage<TMessage>
        {            
            PushMessage(ref message, validator, token);

            try
            {                
                if (MessageHandlerFactory == null)
                {
                    return 0;
                }
                MessagePointer.ThrowIfCancellationRequested();
                int handlerCount = 0;

                using (var scope = new UnitOfWorkScope(DomainEventBus))
                {
                    foreach (var handler in MessageHandlerFactory.CreateMessageHandlersFor(message))
                    {
                        Decorate(handler).Handle(message);
                        handlerCount++;                        
                    }
                    scope.Complete();
                }
                MessagePointer.ThrowIfCancellationRequested();

                return handlerCount;
            }
            finally
            {
                PopMessage();
            }            
        }

        /// <inheritdoc />
        public int Handle<TMessage>(TMessage message, IMessageValidator<TMessage> validator, IMessageHandler<TMessage> handler) where TMessage : class, IMessage<TMessage>
        {
            return Handle(message, validator, handler, null);
        }

        /// <inheritdoc />
        public virtual int Handle<TMessage>(TMessage message, IMessageValidator<TMessage> validator, IMessageHandler<TMessage> handler, CancellationToken? token) where TMessage : class, IMessage<TMessage>
        {
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }            
            PushMessage(ref message, validator, token);

            try
            {
                MessagePointer.ThrowIfCancellationRequested();

                using (var scope = new UnitOfWorkScope(DomainEventBus))
                {
                    Decorate(handler).Handle(message);                    
                    scope.Complete();
                }
                MessagePointer.ThrowIfCancellationRequested();

                return 1;
            }
            finally
            {
                PopMessage();
            }            
        }

        /// <inheritdoc />
        public int Handle<TMessage>(TMessage message, IMessageValidator<TMessage> validator, Action<TMessage> action) where TMessage : class, IMessage<TMessage>
        {
            return Handle(message, validator, action, null);
        }

        /// <inheritdoc />
        public virtual int Handle<TMessage>(TMessage message, IMessageValidator<TMessage> validator, Action<TMessage> handler, CancellationToken? token) where TMessage : class, IMessage<TMessage>
        {
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }            
            PushMessage(ref message, validator, token);

            try
            {
                MessagePointer.ThrowIfCancellationRequested();

                using (var scope = new UnitOfWorkScope(DomainEventBus))
                {
                    Decorate(handler).Handle(message);                    
                    scope.Complete();
                }
                MessagePointer.ThrowIfCancellationRequested();

                return 1;
            }
            finally
            {
                PopMessage();
            }            
        }

        private void PushMessage<TMessage>(ref TMessage message, IMessageValidator<TMessage> validator, CancellationToken? token) where TMessage : class, IMessage<TMessage>
        {
            FunctionalException exception;

            if (IsInvalidOrDenied(message, validator, out exception))
            {
                throw exception;
            }
            message = message.Copy();

            MessagePointer = MessagePointer == null ? new MessagePointer(message, token) : MessagePointer.CreateChildPointer(message, token);                        
        }

        private void PopMessage()
        {
            MessagePointer = MessagePointer.ParentPointer;
        }

        /// <summary>
        /// Checks whether the message can be processed or not. If the sender has insufficient rights or
        /// because the message is invalid, a <see cref="FunctionalException" /> is created that
        /// can be thrown by the caller.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message.</typeparam>
        /// <param name="message">The message to check.</param>
        /// <param name="validator">Optional validator of the message.</param>
        /// <param name="exception">
        /// If the message cannot be processed, this parameter will refer to an exception that can be thrown by the caller.
        /// </param>
        /// <returns><c>true</c> if the <paramref name="message"/> cannot be processed; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        /// <remarks>
        /// The default implementation only validates the message. If security checks need to be added, this method
        /// can be overridden.
        /// </remarks>
        protected virtual bool IsInvalidOrDenied<TMessage>(TMessage message, IMessageValidator<TMessage> validator, out FunctionalException exception) where TMessage : class, IMessage<TMessage>
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            MessageErrorTree errors;

            if (validator != null && validator.IsNotValid(message, out errors))
            {
                exception = new InvalidMessageException(message, ExceptionMessages.MessageProcessor_InvalidMessage, errors);
                return true;
            }
            exception = null;
            return false;
        }

        private IMessageHandler<TMessage> Decorate<TMessage>(Action<TMessage> handler) where TMessage : class
        {
            return Decorate(new ActionDecorator<TMessage>(handler));
        }

        /// <summary>
        /// Creates and returns a <see cref="IMessageHandler{TMessage}" /> pipeline.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message to handle.</typeparam>
        /// <param name="handler">The handler to decorate.</param>
        /// <returns>A pipeline that will handle a message.</returns>
        /// <remarks>
        /// The default implementation simply returns the specified <paramref name="handler"/>.
        /// </remarks>
        protected virtual IMessageHandler<TMessage> Decorate<TMessage>(IMessageHandler<TMessage> handler) where TMessage : class
        {
            return handler;
        }

        #endregion
    }
}
