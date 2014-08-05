using System.ComponentModel.Messaging.Resources;
using System.Threading;

namespace System.ComponentModel.Messaging.Server
{
    /// <summary>
    /// Represents a handler of arbitrary messages.
    /// </summary>    
    public abstract class MessageProcessor : IMessageProcessor
    {
        private readonly IMessageProcessorBus _domainEventBus;                
        private readonly ThreadLocal<UseCase> _currentUseCase;                    

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
            _currentUseCase = new ThreadLocal<UseCase>();
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
        public UseCase CurrentUseCase
        {
            get { return _currentUseCase.Value; }
            private set { _currentUseCase.Value = value; }
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
            Process(message, null, null);
        }

        /// <inheritdoc />
        public virtual void Process<TMessage>(TMessage message, CancellationToken? token, IProgressReporter reporter) where TMessage : class
        {            
            BeginUseCase(message, token, reporter);

            try
            {
                if (MessageHandlerFactory == null)
                {
                    throw NewMessageHandlerFactoryNotSetException();
                }
                CurrentUseCase.ThrowIfCancellationRequested();

                using (var scope = new UnitOfWorkScope(DomainEventBus))
                {
                    foreach (var handler in MessageHandlerFactory.CreateMessageHandlersFor(message))
                    {
                        CreatePipelineFor(handler).Handle(message);

                        CurrentUseCase.ThrowIfCancellationRequested();
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
            Process(message, handler, null, null);
        }

        /// <inheritdoc />
        public virtual void Process<TMessage>(TMessage message, IMessageHandler<TMessage> handler, CancellationToken? token, IProgressReporter reporter) where TMessage : class
        {
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }
            BeginUseCase(message, token, reporter);

            try
            {
                CurrentUseCase.ThrowIfCancellationRequested();

                using (var scope = new UnitOfWorkScope(DomainEventBus))
                {
                    CreatePipelineFor(MessageHandlerFactory.CreateMessageHandler(handler)).Handle(message);

                    CurrentUseCase.ThrowIfCancellationRequested();
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
            Process(message, action, null, null);
        }

        /// <inheritdoc />
        public virtual void Process<TMessage>(TMessage message, Action<TMessage> action, CancellationToken? token, IProgressReporter reporter) where TMessage : class
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }
            BeginUseCase(message, token, reporter);

            try
            {
                CurrentUseCase.ThrowIfCancellationRequested();

                using (var scope = new UnitOfWorkScope(DomainEventBus))
                {
                    CreatePipelineFor(MessageHandlerFactory.CreateMessageHandler(action)).Handle(message);

                    CurrentUseCase.ThrowIfCancellationRequested();
                    scope.Complete();
                }
            }
            finally
            {
                EndMessage();
            }            
        }

        private void BeginUseCase(object message, CancellationToken? token, IProgressReporter reporter)
        {
            CurrentUseCase = CurrentUseCase == null ? new UseCase(message, token, reporter) : CurrentUseCase.CreateChildUseCase(message, token, reporter);            
        }

        private void EndMessage()
        {
            CurrentUseCase = CurrentUseCase.ParentUseCase;
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
