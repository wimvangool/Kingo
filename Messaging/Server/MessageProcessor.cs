using System.Threading;

namespace System.ComponentModel.Messaging.Server
{
    /// <summary>
    /// Represents a handler of arbitrary messages.
    /// </summary>    
    public abstract class MessageProcessor : IMessageProcessor
    {
        private readonly IMessageProcessorBus _domainEventBus;                
        private readonly ThreadLocal<MessagePointer> _currentMessage;                            

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageProcessor" /> class.
        /// </summary>        
        protected MessageProcessor()
        {
            _domainEventBus = new MessageProcessorBus(this);
            _currentMessage = new ThreadLocal<MessagePointer>();
        }        

        /// <inheritdoc />
        public IMessageProcessorBus DomainEventBus
        {
            get { return _domainEventBus; }
        }

        /// <inheritdoc />
        public MessagePointer Message
        {
            get { return _currentMessage.Value; }
            private set { _currentMessage.Value = value; }
        }

        /// <summary>
        /// Returns the <see cref="MessageHandlerFactory" /> of this processor.
        /// </summary>
        protected abstract MessageHandlerFactory MessageHandlerFactory
        {
            get;
        }        

        /// <inheritdoc />
        public void Process<TMessage>(TMessage message) where TMessage : class
        {
            Process(message, null as CancellationToken?);
        }

        /// <inheritdoc />
        public virtual void Process<TMessage>(TMessage message, CancellationToken? token) where TMessage : class
        {            
            PushMessage(message, token);

            try
            {                
                Message.ThrowIfCancellationRequested();

                using (var scope = new UnitOfWorkScope(DomainEventBus))
                {
                    foreach (var handler in MessageHandlerFactory.CreateMessageHandlersFor(message))
                    {
                        handler.Handle(message);

                        Message.ThrowIfCancellationRequested();
                    }
                    scope.Complete();
                }
            }
            finally
            {
                PopMessage();
            }            
        }

        /// <inheritdoc />
        public void Process<TMessage>(TMessage message, IMessageHandler<TMessage> handler) where TMessage : class
        {
            Process(message, handler, null);
        }

        /// <inheritdoc />
        public virtual void Process<TMessage>(TMessage message, IMessageHandler<TMessage> handler, CancellationToken? token) where TMessage : class
        {
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }
            PushMessage(message, token);

            try
            {
                Message.ThrowIfCancellationRequested();

                using (var scope = new UnitOfWorkScope(DomainEventBus))
                {
                    handler.Handle(message);

                    Message.ThrowIfCancellationRequested();
                    scope.Complete();
                }
            }
            finally
            {
                PopMessage();
            }            
        }

        /// <inheritdoc />
        public void Process<TMessage>(TMessage message, Action<TMessage> action) where TMessage : class
        {
            Process(message, action, null);
        }

        /// <inheritdoc />
        public virtual void Process<TMessage>(TMessage message, Action<TMessage> handler, CancellationToken? token) where TMessage : class
        {
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }
            PushMessage(message, token);

            try
            {
                Message.ThrowIfCancellationRequested();

                using (var scope = new UnitOfWorkScope(DomainEventBus))
                {
                    handler.Invoke(message);

                    Message.ThrowIfCancellationRequested();
                    scope.Complete();
                }
            }
            finally
            {
                PopMessage();
            }            
        }

        private void PushMessage(object message, CancellationToken? token)
        {
            Message = Message == null ? new MessagePointer(message, token) : Message.CreateChildPointer(message, token);            
        }

        private void PopMessage()
        {
            Message = Message.ParentPointer;
        }               
    }
}
