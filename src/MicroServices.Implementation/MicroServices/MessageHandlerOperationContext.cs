using Kingo.Clocks;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Represents the context in which a <see cref="MicroProcessor"/> invokes a <see cref="IMessageHandler{TMessage}"/>.
    /// </summary>
    public sealed class MessageHandlerOperationContext : MicroProcessorOperationContext, IMessageHandlerOperationContext
    {                
        private readonly IUnitOfWork _unitOfWork;
        private readonly MessageBus _messageBus;

        internal MessageHandlerOperationContext(MicroProcessor processor, IUnitOfWork unitOfWork) :
            base(processor)
        {                        
            _unitOfWork = unitOfWork;
            _messageBus = CreateMessageBus(processor, Clock);
        }

        private MessageHandlerOperationContext(MessageHandlerOperationContext context, IAsyncMethodOperation operation) :
            base(context, operation)
        {                        
            _unitOfWork = context._unitOfWork;
            _messageBus = CreateMessageBus(context.Processor, Clock);
        }

        /// <inheritdoc />
        public IUnitOfWork UnitOfWork =>
            _unitOfWork;

        /// <inheritdoc />
        public IMessageBus MessageBus =>
            _messageBus;

        internal MessageBusResult MessageBusResult() =>
            _messageBus.ToResult();

        internal MessageHandlerOperationContext PushOperation(HandleAsyncMethodOperation operation) =>
            new MessageHandlerOperationContext(this, operation);
        
        private static MessageBus CreateMessageBus(MicroProcessor processor, IClock clock) =>
            new MessageBus(processor.MessageFactory, processor.ServiceProvider, clock);
    }
}
