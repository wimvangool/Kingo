using Kingo.Clocks;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Represents the context in which a <see cref="MicroProcessor"/> invokes a <see cref="IMessageHandler{TMessage}"/>.
    /// </summary>
    public sealed class MessageHandlerOperationContext : MicroProcessorOperationContext
    {
        private readonly CommandProcessor _commandProcessor;
        private readonly IUnitOfWork _unitOfWork;
        private readonly MessageBus _messageBus;

        internal MessageHandlerOperationContext(MicroProcessor processor, IUnitOfWork unitOfWork) :
            base(processor)
        {                        
            _commandProcessor = new CommandProcessor(this);
            _unitOfWork = unitOfWork;
            _messageBus = new MessageBus(processor.MessageFactory, Clock);
        }

        private MessageHandlerOperationContext(MessageHandlerOperationContext context, IAsyncMethodOperation operation) :
            base(context, operation)
        {
            _commandProcessor = new CommandProcessor(this);
            _unitOfWork = context._unitOfWork;
            _messageBus = context.CreateMessageBus(operation);
        }

        /// <summary>
        /// Gets the processor that can be used to execute (internal) commands as part of another operation.
        /// </summary>
        public ICommandProcessor CommandProcessor =>
            _commandProcessor;

        /// <summary>
        /// Represents the unit of work that is associated to the current operation.
        /// </summary>
        public IUnitOfWork UnitOfWork =>
            _unitOfWork;

        /// <summary>
        /// Represents the message bus that can be used to send commands and publish events
        /// as result of the current operation.
        /// </summary>
        public IMessageBus MessageBus =>
            _messageBus;

        private MessageBus CreateMessageBus(IAsyncMethodOperation nextOperation) =>
            CreateMessageBus(nextOperation, Processor.MessageFactory, Clock);

        private MessageBus CreateMessageBus(IAsyncMethodOperation nextOperation, MessageFactory messageFactory, IClock clock)
        {
            if (nextOperation.Message.Direction == MessageDirection.Internal)
            {
                return new MessageBus(messageFactory, clock, _messageBus);
            }
            return new MessageBus(messageFactory, clock);
        }

        internal MessageHandlerOperationResult<TMessage> CommitResult<TMessage>(Message<TMessage> input) =>
            _messageBus.CommitResult(input);

        internal MessageHandlerOperationContext PushOperation<TMessage>(HandleAsyncMethodOperation<TMessage> operation) =>
            new MessageHandlerOperationContext(this, operation);
    }
}
