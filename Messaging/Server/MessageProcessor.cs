using System.Threading;

namespace System.ComponentModel.Server
{
    /// <summary>
    /// Represents a handler of arbitrary messages.
    /// </summary>    
    public abstract class MessageProcessor : IMessageProcessor
    {        
        #region [====== MessageDispatcherPipeline ======]

        private sealed class MessageDispatcherPipeline<TMessage> : IMessageHandler<TMessage> where TMessage : class
        {            
            private readonly IMessageHandler<TMessage> _handler;
            private readonly MessageProcessor _processor;

            internal MessageDispatcherPipeline(IMessageHandler<TMessage> handler, MessageProcessor processor)
            {                
                _handler = handler;
                _processor = processor;
            } 
           
            public void Handle(TMessage message)
            {
                _processor.MessagePointer.ThrowIfCancellationRequested();

                using (var scope = new UnitOfWorkScope(_processor.DomainEventBus))
                {
                    HandleMessage(message);                    

                    scope.Complete();
                }
                _processor.MessagePointer.ThrowIfCancellationRequested(); 
            }

            private void HandleMessage(TMessage message)
            {
                if (_handler == null)
                {
                    if (_processor.MessageHandlerFactory == null)
                    {
                        return;
                    }
                    foreach (var handler in _processor.MessageHandlerFactory.CreateMessageHandlersFor(message))
                    {
                        HandleMessage(message, handler);
                    }
                }
                else
                {
                    HandleMessage(message, _handler);
                }
            }

            private void HandleMessage(TMessage message, IMessageHandler<TMessage> handler)
            {                
                _processor.CreatePerMessageHandlerPipeline(handler).Handle(message);
                _processor.MessagePointer.ThrowIfCancellationRequested();
            }
        }

        #endregion        

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
        public void Execute<TCommand>(TCommand message) where TCommand : class, IRequestMessage<TCommand>
        {
            Handle(message, message);
        }

        /// <inheritdoc />
        public void Execute<TCommand>(TCommand message, CancellationToken? token) where TCommand : class, IRequestMessage<TCommand>
        {
            Handle(message, message, token);
        }

        /// <inheritdoc />
        public void Execute<TCommand>(TCommand message, Action<TCommand> handler) where TCommand : class, IRequestMessage<TCommand>
        {
            Handle(message, message, handler);
        }

        /// <inheritdoc />
        public void Execute<TCommand>(TCommand message, Action<TCommand> handler, CancellationToken? token) where TCommand : class, IRequestMessage<TCommand>
        {
            Handle(message, message, handler, token);
        }

        /// <inheritdoc />
        public void Execute<TCommand>(TCommand message, IMessageHandler<TCommand> handler) where TCommand : class, IRequestMessage<TCommand>
        {
            Handle(message, message, handler);
        }

        /// <inheritdoc />
        public void Execute<TCommand>(TCommand message, IMessageHandler<TCommand> handler, CancellationToken? token) where TCommand : class, IRequestMessage<TCommand>
        {
            Handle(message, message, handler, token);
        }

        #endregion

        #region [====== Events ======]

        /// <inheritdoc />
        public void Handle<TMessage>(TMessage message) where TMessage : class, IMessage<TMessage>
        {
            Handle(message, null, NoMessageHandler<TMessage>(), null);
        }

        /// <inheritdoc />
        public void Handle<TMessage>(TMessage message, IMessageValidator<TMessage> validator) where TMessage : class, IMessage<TMessage>
        {
            Handle(message, validator, NoMessageHandler<TMessage>(), null);
        }

        /// <inheritdoc />
        public void Handle<TMessage>(TMessage message, IMessageValidator<TMessage> validator, CancellationToken? token) where TMessage : class, IMessage<TMessage>
        {
            Handle(message, validator, NoMessageHandler<TMessage>(), token);
        }

        /// <inheritdoc />
        public void Handle<TMessage>(TMessage message, IMessageValidator<TMessage> validator, Action<TMessage> handler) where TMessage : class, IMessage<TMessage>
        {
            Handle(message, validator, ToMessageHandler(handler), null);
        }

        /// <inheritdoc />
        public void Handle<TMessage>(TMessage message, IMessageValidator<TMessage> validator, Action<TMessage> handler, CancellationToken? token) where TMessage : class, IMessage<TMessage>
        {
            Handle(message, validator, ToMessageHandler(handler), token);             
        }

        /// <inheritdoc />
        public void Handle<TMessage>(TMessage message, IMessageValidator<TMessage> validator, IMessageHandler<TMessage> handler) where TMessage : class, IMessage<TMessage>
        {
            Handle(message, validator, handler, null);
        }

        /// <inheritdoc />
        public void Handle<TMessage>(TMessage message, IMessageValidator<TMessage> validator, IMessageHandler<TMessage> handler, CancellationToken? token) where TMessage : class, IMessage<TMessage>
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            PushMessage(ref message, token);

            try
            {
                handler = new MessageDispatcherPipeline<TMessage>(handler, this);
                handler = CreatePerMessagePipeline(handler, validator);
                handler.Handle(message);
            }
            finally
            {
                PopMessage();
            }                   
        }

        private void PushMessage<TMessage>(ref TMessage message, CancellationToken? token) where TMessage : class, IMessage<TMessage>
        {                                    
            MessagePointer = MessagePointer == null ?
                new MessagePointer(message = message.Copy(), token) :
                MessagePointer.CreateChildPointer(message = message.Copy(), token);                        
        }

        private void PopMessage()
        {
            MessagePointer = MessagePointer.ParentPointer;
        }        

        /// <summary>
        /// Creates and returns a <see cref="IMessageHandler{TMessage}" /> pipeline on top of all handlers that will
        /// be invoked for a specific message.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message to handle.</typeparam>
        /// <param name="handler">The handler to decorate.</param>
        /// <param name="validator">Optional validator of the message.</param>
        /// <returns>A pipeline that will handle a message.</returns>        
        protected virtual IMessageHandler<TMessage> CreatePerMessagePipeline<TMessage>(IMessageHandler<TMessage> handler, IMessageValidator<TMessage> validator) where TMessage : class
        {
            return new MessageValidationPipeline<TMessage>(handler, validator);
        }

        /// <summary>
        /// Creates and returns a <see cref="IMessageHandler{TMessage}" /> pipeline on top of each handler that will be
        /// invoked for a specific message.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message to handle.</typeparam>
        /// <param name="handler">The handler to decorate.</param>
        /// <returns>A pipeline that will handle a message.</returns>
        /// <remarks>
        /// The default implementation simply returns the specified <paramref name="handler"/>.
        /// </remarks>
        protected virtual IMessageHandler<TMessage> CreatePerMessageHandlerPipeline<TMessage>(IMessageHandler<TMessage> handler) where TMessage : class
        {
            return handler;
        }

        private static IMessageHandler<TMessage> ToMessageHandler<TMessage>(Action<TMessage> handler) where TMessage : class
        {
            return handler == null ? null : new ActionDecorator<TMessage>(handler);
        }

        private static IMessageHandler<TMessage> NoMessageHandler<TMessage>() where TMessage : class
        {
            return null;
        }

        #endregion
    }
}
