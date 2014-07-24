using System;
using System.Threading;
using YellowFlare.MessageProcessing.Resources;

namespace YellowFlare.MessageProcessing.Server
{
    /// <summary>
    /// Represents a handler of arbitrary messages.
    /// </summary>    
    public abstract class MessageProcessor : IMessageProcessor
    {
        private readonly IMessageProcessorBus _domainEventBus;                
        private readonly ThreadLocal<MessageStack> _currentMessage;                    

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageProcessor" /> class.
        /// </summary>                      
        protected MessageProcessor()
            : this(null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageProcessor" /> class.
        /// </summary>
        /// <param name="domainEventBus">
        /// A custom <see cref="IDomainEventBus" /> that will received all messages published by this processor's handlers.
        /// </param>
        protected MessageProcessor(IDomainEventBus domainEventBus)
        {
            _domainEventBus = InitializeMessageProcessorBus(domainEventBus);
            _currentMessage = new ThreadLocal<MessageStack>();
        }

        private IMessageProcessorBus InitializeMessageProcessorBus(IDomainEventBus domainEventBus)
        {
            var messageProcessorBus = new MessageProcessorBus(this);

            if (domainEventBus == null)
            {
                return messageProcessorBus;
            }
            return new MessageProcessorBusRelay(messageProcessorBus, domainEventBus);
        }

        /// <inheritdoc />
        public IMessageProcessorBus DomainEventBus
        {
            get { return _domainEventBus; }
        }

        /// <inheritdoc />
        public MessageStack CurrentMessage
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

        /// <summary>
        /// Returns the <see cref="IMessageHandlerPipelineFactory" /> of this processor. Default returns <c>null</c>.
        /// </summary>
        protected virtual IMessageHandlerPipelineFactory PipelineFactory
        {
            get { return null; }
        }

        /// <inheritdoc />
        public void Process<TMessage>(TMessage message) where TMessage : class
        {
            Process(message, (CancellationToken?) null);
        }

        /// <inheritdoc />
        public virtual void Process<TMessage>(TMessage message, CancellationToken? token) where TMessage : class
        {            
            BeginMessage(message, token);

            try
            {
                if (MessageHandlerFactory == null)
                {
                    throw NewMessageHandlerFactoryNotSetException();
                }
                CurrentMessage.ThrowIfCancellationRequested();

                using (var scope = new UnitOfWorkScope(DomainEventBus))
                {
                    foreach (var handler in MessageHandlerFactory.CreateMessageHandlersFor(message))
                    {
                        CreatePipelineFor(handler).Handle(message);

                        CurrentMessage.ThrowIfCancellationRequested();
                    }
                    scope.Complete();
                }
            }
            finally
            {
                EndMessage();
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
            BeginMessage(message, token);

            try
            {
                CurrentMessage.ThrowIfCancellationRequested();

                using (var scope = new UnitOfWorkScope(DomainEventBus))
                {
                    CreatePipelineFor(MessageHandlerFactory.CreateMessageHandler(handler)).Handle(message);

                    CurrentMessage.ThrowIfCancellationRequested();
                    scope.Complete();
                }
            }
            finally
            {
                EndMessage();
            }            
        }

        /// <inheritdoc />
        public void Process<TMessage>(TMessage message, Action<TMessage> action) where TMessage : class
        {
            Process(message, action, null);
        }

        /// <inheritdoc />
        public virtual void Process<TMessage>(TMessage message, Action<TMessage> action, CancellationToken? token) where TMessage : class
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }
            BeginMessage(message, token);

            try
            {
                CurrentMessage.ThrowIfCancellationRequested();

                using (var scope = new UnitOfWorkScope(DomainEventBus))
                {
                    CreatePipelineFor(MessageHandlerFactory.CreateMessageHandler(action)).Handle(message);

                    CurrentMessage.ThrowIfCancellationRequested();
                    scope.Complete();
                }
            }
            finally
            {
                EndMessage();
            }            
        }
        
        private void BeginMessage(object message, CancellationToken? token)
        {
            CurrentMessage = CurrentMessage == null ? new MessageStack(message, token) : CurrentMessage.NextMessage(message, token);            
        }

        private void EndMessage()
        {
            CurrentMessage = CurrentMessage.PreviousMessage;
        }

        private IMessageHandler<TMessage> CreatePipelineFor<TMessage>(IMessageHandlerPipeline<TMessage> handler) where TMessage : class
        {
            return PipelineFactory == null ? handler : PipelineFactory.CreatePipeline(handler, UnitOfWorkContext.Current);
        }

        private static Exception NewMessageHandlerFactoryNotSetException()
        {
            return new NotSupportedException(ExceptionMessages.MessageProcessor_FactoryNotSet);
        }
    }
}
