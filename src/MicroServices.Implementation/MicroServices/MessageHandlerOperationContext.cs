namespace Kingo.MicroServices
{
    /// <summary>
    /// Represents the context in which a <see cref="MicroProcessor"/> invokes a <see cref="IMessageHandler{TMessage}"/>.
    /// </summary>
    public sealed class MessageHandlerOperationContext : MicroProcessorOperationContext
    {                
        private readonly IUnitOfWork _unitOfWork;
        private readonly MessageBus _messageBus;

        internal MessageHandlerOperationContext(MicroProcessor processor, IUnitOfWork unitOfWork) :
            base(processor)
        {                        
            _unitOfWork = unitOfWork;
            _messageBus = new MessageBus();
        }

        private MessageHandlerOperationContext(MessageHandlerOperationContext context, IAsyncMethodOperation operation) :
            base(context, operation)
        {                        
            _unitOfWork = context._unitOfWork;
            _messageBus = new MessageBus();
        }       

        /// <summary>
        /// Represents the unit of work that is associated to the current operation.
        /// </summary>
        public IUnitOfWork UnitOfWork =>
            _unitOfWork;

        /// <summary>
        /// Represents the message bus that can be used to schedule commands to be sent or events to be published
        /// after the operation has completed.
        /// </summary>
        public IMessageBus MessageBus =>
            _messageBus;  
        
        internal MessageHandlerOperationContext PushOperation(HandleAsyncMethodOperation operation) =>
            new MessageHandlerOperationContext(this, operation);        
    }
}
