using System;

namespace YellowFlare.MessageProcessing
{
    /// <summary>
    /// Represents a handler of arbitrary messages.
    /// </summary>    
    public sealed class MessageProcessor : IMessageProcessor
    {
        private readonly MessageProcessorBus _bus;  
        private readonly MessageHandlerFactory _handlerFactory;
        private readonly IMessageHandlerPipelineFactory _pipelineFactory;
             
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageProcessor" /> class.
        /// </summary>
        /// <param name="processor">Processor that encapsulates this processor.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="processor" /> is <c>null</c>.
        /// </exception>
        public MessageProcessor(IMessageProcessor processor)
            : this(processor, null, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageProcessor" /> class.
        /// </summary>
        /// <param name="processor">Processor that encapsulates this processor.</param>
        /// <param name="handlerFactory">Factory used to instantiate the message-handlers (optional).</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="processor"/> is <c>null</c>.
        /// </exception>
        public MessageProcessor(IMessageProcessor processor, MessageHandlerFactory handlerFactory)
            : this(processor, handlerFactory, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageProcessor" /> class.
        /// </summary>
        /// <param name="processor">Processor that encapsulates this processor.</param>
        /// <param name="pipelineFactory">Factory used to construct a pipeline on top of every message-handler (optional).</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="processor"/> is <c>null</c>.        
        /// </exception>
        public MessageProcessor(IMessageProcessor processor, IMessageHandlerPipelineFactory pipelineFactory)
            : this(processor, null, pipelineFactory) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageProcessor" /> class.
        /// </summary>
        /// <param name="processor">Processor that encapsulates this processor.</param>
        /// <param name="handlerFactory">Factory used to instantiate the message-handlers (optional).</param>
        /// <param name="pipelineFactory">Factory used to construct a pipeline on top of every message-handler (optional).</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="processor"/> is <c>null</c>.        
        /// </exception>
        public MessageProcessor(IMessageProcessor processor, MessageHandlerFactory handlerFactory, IMessageHandlerPipelineFactory pipelineFactory)
        {            
            _bus = new MessageProcessorBus(processor);          
            _handlerFactory = handlerFactory;
            _pipelineFactory = pipelineFactory;        
        }

        /// <inheritdoc />
        public IMessageProcessorBus Bus
        {
            get { return _bus; }
        }

        /// <inheritdoc />
        public void Handle<TMessage>(TMessage message) where TMessage : class
        {
            if (_handlerFactory == null)
            {
                return;
            }
            using (var scope = new UnitOfWorkScope())
            {
                foreach (var handler in _handlerFactory.CreateMessageHandlersFor(message))
                {
                    CreatePipelineFor(handler).Handle(message);
                }
                scope.Complete();
            }
        }               

        /// <inheritdoc />
        public void Handle<TMessage>(TMessage message, IMessageHandler<TMessage> handler) where TMessage : class
        {
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }
            using (var scope = new UnitOfWorkScope())
            {
                CreatePipelineFor(MessageHandlerFactory.CreateMessageHandler(handler)).Handle(message);
                scope.Complete();
            }
        }        

        /// <inheritdoc />
        public void Handle<TMessage>(TMessage message, Action<TMessage> action) where TMessage : class
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }
            using (var scope = new UnitOfWorkScope())
            {
                CreatePipelineFor(MessageHandlerFactory.CreateMessageHandler(action)).Handle(message);
                scope.Complete();
            }
        }               

        private IMessageHandler<TMessage> CreatePipelineFor<TMessage>(IMessageHandlerPipeline<TMessage> handler) where TMessage : class
        {
            return _pipelineFactory == null ? handler : _pipelineFactory.CreatePipeline(handler, UnitOfWorkContext.Current);
        }                                               
    }
}
