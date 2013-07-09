using System;
using System.Threading;

namespace YellowFlare.MessageProcessing
{
    /// <summary>
    /// Represents a handler of arbitrary messages.
    /// </summary>    
    public abstract class MessageProcessor : IMessageProcessor
    {
        private readonly MessageProcessorBus _domainEventBus;        
        private readonly MessageHandlerFactory _handlerFactory;
        private readonly IMessageHandlerPipelineFactory _pipelineFactory;
        private readonly ThreadLocal<Message> _currentMessage;
             
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageProcessor" /> class.
        /// </summary>        
        protected MessageProcessor()
            : this(null, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageProcessor" /> class.
        /// </summary>        
        /// <param name="handlerFactory">Factory used to instantiate the message-handlers (optional).</param>        
        protected MessageProcessor(MessageHandlerFactory handlerFactory)
            : this(handlerFactory, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageProcessor" /> class.
        /// </summary>        
        /// <param name="pipelineFactory">Factory used to construct a pipeline on top of every message-handler (optional).</param>        
        protected MessageProcessor(IMessageHandlerPipelineFactory pipelineFactory)
            : this(null, pipelineFactory) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageProcessor" /> class.
        /// </summary>        
        /// <param name="handlerFactory">Factory used to instantiate the message-handlers (optional).</param>
        /// <param name="pipelineFactory">Factory used to construct a pipeline on top of every message-handler (optional).</param>        
        protected MessageProcessor(MessageHandlerFactory handlerFactory, IMessageHandlerPipelineFactory pipelineFactory)
        {            
            _domainEventBus = new MessageProcessorBus(this);              
            _handlerFactory = handlerFactory;
            _pipelineFactory = pipelineFactory;  
            _currentMessage = new ThreadLocal<Message>();
        }

        /// <inheritdoc />
        public virtual IMessageProcessorBus DomainEventBus
        {
            get { return _domainEventBus; }
        }

        /// <inheritdoc />
        public Message CurrentMessage
        {
            get { return _currentMessage.Value; }
            private set { _currentMessage.Value = value; }
        }

        /// <inheritdoc />
        public virtual void Handle<TMessage>(TMessage message) where TMessage : class
        {
            if (_handlerFactory == null)
            {
                return;
            }
            BeginMessage(message);

            try
            {
                using (var scope = new UnitOfWorkScope(DomainEventBus))
                {
                    foreach (var handler in _handlerFactory.CreateMessageHandlersFor(message))
                    {
                        CreatePipelineFor(handler).Handle(message);
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
        public virtual void Handle<TMessage>(TMessage message, IMessageHandler<TMessage> handler) where TMessage : class
        {
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }
            BeginMessage(message);

            try
            {
                using (var scope = new UnitOfWorkScope(DomainEventBus))
                {
                    CreatePipelineFor(MessageHandlerFactory.CreateMessageHandler(handler)).Handle(message);
                    scope.Complete();
                }
            }
            finally
            {
                EndMessage();
            }            
        }        

        /// <inheritdoc />
        public virtual void Handle<TMessage>(TMessage message, Action<TMessage> action) where TMessage : class
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }
            BeginMessage(message);

            try
            {
                using (var scope = new UnitOfWorkScope(DomainEventBus))
                {
                    CreatePipelineFor(MessageHandlerFactory.CreateMessageHandler(action)).Handle(message);
                    scope.Complete();
                }
            }
            finally
            {
                EndMessage();
            }            
        }
        
        private void BeginMessage(object message)
        {
            CurrentMessage = CurrentMessage == null ? new Message(message) : CurrentMessage.NextMessage(message);            
        }

        private void EndMessage()
        {
            CurrentMessage = CurrentMessage.PreviousMessage;
        }

        private IMessageHandler<TMessage> CreatePipelineFor<TMessage>(IMessageHandlerPipeline<TMessage> handler) where TMessage : class
        {
            return _pipelineFactory == null ? handler : _pipelineFactory.CreatePipeline(handler, UnitOfWorkContext.Current);
        }                                               
    }
}
