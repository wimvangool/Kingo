using System;

namespace YellowFlare.MessageProcessing
{
    /// <summary>
    /// Represents a handler of arbitrary commands.
    /// </summary>    
    public sealed class MessageProcessor : IMessageProcessor
    {        
        private readonly MessageHandlerFactory _handlerFactory;
        private readonly MessageHandlerPipelineFactory _pipelineFactory;
        private readonly DomainEventBus _domainEventBus;       

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageProcessor" /> class.
        /// </summary>
        /// <param name="handlerFactory">The factory used to instantiate the message-handlers.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="handlerFactory"/> is <c>null</c>.
        /// </exception>
        public MessageProcessor(MessageHandlerFactory handlerFactory)
        {
            if (handlerFactory == null)
            {
                throw new ArgumentNullException("handlerFactory");
            }
            _handlerFactory = handlerFactory;            
            _pipelineFactory = new MessageHandlerPipelineFactory();
            _domainEventBus = new DomainEventBus(this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageProcessor" /> class.
        /// </summary>
        /// <param name="handlerFactory">The factory used to instantiate the message-handlers.</param>
        /// <param name="pipelineFactory">The factory used to build a (custom) pipeline.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="handlerFactory"/> is <c>null</c>.        
        /// </exception>
        public MessageProcessor(MessageHandlerFactory handlerFactory, MessageHandlerPipelineFactory pipelineFactory)
        {
            if (handlerFactory == null)
            {
                throw new ArgumentNullException("handlerFactory");
            }
            _handlerFactory = handlerFactory;           
            _pipelineFactory = pipelineFactory ?? new MessageHandlerPipelineFactory();
            _domainEventBus = new DomainEventBus(this);
        }

        /// <inheritdoc />
        public IDomainEventBus DomainEventBus
        {
            get { return _domainEventBus; }
        }

        /// <inheritdoc />
        public void Handle<TMessage>(TMessage message) where TMessage : class
        {
            Handle(message, MessageSources.EnterpriseServiceBus);
        }

        internal void Handle<TMessage>(TMessage message, MessageSources source) where TMessage : class
        {
            using (var scope = new UnitOfWorkScope())
            {
                foreach (var handler in _handlerFactory.CreateMessageHandlersFor(message))
                {                    
                    CreatePipelineFor(handler, source).Handle(message);
                }
                scope.Complete();
            }
        }        

        /// <inheritdoc />
        public void Handle<TMessage>(TMessage message, IMessageHandler<TMessage> handler) where TMessage : class
        {            
            Handle(message, handler, MessageSources.EnterpriseServiceBus);
        }

        internal void Handle<TMessage>(TMessage message, IMessageHandler<TMessage> handler, MessageSources source) where TMessage : class
        {
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }
            using (var scope = new UnitOfWorkScope())
            {
                CreatePipelineFor(MessageHandlerFactory.CreateMessageHandler(handler), source).Handle(message);
                scope.Complete();
            }
        }

        /// <inheritdoc />
        public void Handle<TMessage>(TMessage message, Action<TMessage> action) where TMessage : class
        {
            Handle(message, action, MessageSources.EnterpriseServiceBus);
        }

        internal void Handle<TMessage>(TMessage message, Action<TMessage> action, MessageSources source) where TMessage : class
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }
            using (var scope = new UnitOfWorkScope())
            {
                CreatePipelineFor(MessageHandlerFactory.CreateMessageHandler(action), source).Handle(message);
                scope.Complete();
            }
        }        

        private IMessageHandler<TMessage> CreatePipelineFor<TMessage>(IMessageHandlerPipeline<TMessage> handler, MessageSources source) where TMessage : class
        {
            return _pipelineFactory.CreatePipeline(handler, UnitOfWorkContext.Current, source);
        }                                               
    }
}
