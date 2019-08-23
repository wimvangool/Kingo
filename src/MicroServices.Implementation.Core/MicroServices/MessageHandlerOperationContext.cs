namespace Kingo.MicroServices
{
    /// <summary>
    /// Represents the context in which a <see cref="MicroProcessor"/> invokes a <see cref="IMessageHandler{TMessage}"/>.
    /// </summary>
    public sealed class MessageHandlerOperationContext : MicroProcessorOperationContext
    {                
        private readonly IUnitOfWork _unitOfWork;
        private readonly EventBus _eventBus;

        internal MessageHandlerOperationContext(MicroProcessor processor, IUnitOfWork unitOfWork) :
            base(processor)
        {                        
            _unitOfWork = unitOfWork;
            _eventBus = new EventBus();
        }

        private MessageHandlerOperationContext(MessageHandlerOperationContext context, IAsyncMethodOperation operation) :
            base(context, operation)
        {                        
            _unitOfWork = context._unitOfWork;
            _eventBus = new EventBus();
        }       

        /// <summary>
        /// Represents the unit of work that is associated to the current operation.
        /// </summary>
        public IUnitOfWork UnitOfWork =>
            _unitOfWork;

        /// <summary>
        /// Represents the event bus to which all events resulting from the current operation can be published.
        /// </summary>
        public IEventBus EventBus =>
            _eventBus;  
        
        internal MessageHandlerOperationContext PushOperation(HandleAsyncMethodOperation operation) =>
            new MessageHandlerOperationContext(this, operation);        
    }
}
