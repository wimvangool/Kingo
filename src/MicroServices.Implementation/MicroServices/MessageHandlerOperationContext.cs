using Kingo.Clocks;

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
            _messageBus = new MessageBus(processor.MessageFactory, Clock);
        }

        private MessageHandlerOperationContext(MessageHandlerOperationContext context, IAsyncMethodOperation operation) :
            base(context, operation)
        {                        
            _unitOfWork = context._unitOfWork;
            _messageBus = new MessageBus(context.Processor.MessageFactory, Clock);
        }

        /// <summary>
        /// Represents the unit of work that is associated to the current operation.
        /// </summary>
        public IUnitOfWork UnitOfWork =>
            _unitOfWork;

        /// <summary>
        /// Represents the message bus that can be used to send commands and publish events
        /// after the operation has completed.
        /// </summary>
        public IMessageBus MessageBus =>
            _messageBus;

        internal MessageBusResult MessageBusResult() =>
            _messageBus.ToResult();

        internal MessageHandlerOperationContext PushOperation(HandleAsyncMethodOperation operation) =>
            new MessageHandlerOperationContext(this, operation);
    }
}
