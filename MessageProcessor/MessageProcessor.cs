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
        private readonly ThreadLocal<Message> _currentMessage;                    

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageProcessor" /> class.
        /// </summary>                      
        protected MessageProcessor()
        {            
            _domainEventBus = new MessageProcessorBus(this);                          
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

        /// <summary>
        /// Returns the <see cref="MessageHandlerFactory" /> of this processor. Default returns <c>null</c>.
        /// </summary>
        protected virtual MessageHandlerFactory MessageHandlerFactory
        {
            get { return null; }
        }

        /// <summary>
        /// Returns the <see cref="IMessageHandlerPipelineFactory" /> of this processor. Default returns <c>null</c>.
        /// </summary>
        protected virtual IMessageHandlerPipelineFactory PipelineFactory
        {
            get { return null; }
        }

        /// <inheritdoc />
        public virtual void Handle<TMessage>(TMessage message) where TMessage : class
        {            
            BeginMessage(message);

            try
            {
                if (MessageHandlerFactory == null)
                {
                    return;
                }
                using (var scope = new UnitOfWorkScope(DomainEventBus))
                {
                    foreach (var handler in MessageHandlerFactory.CreateMessageHandlersFor(message))
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
            return PipelineFactory == null ? handler : PipelineFactory.CreatePipeline(handler, UnitOfWorkContext.Current);
        }                                               
    }
}
