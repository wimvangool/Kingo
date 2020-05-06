using System.Threading;

namespace Kingo.MicroServices
{
    internal sealed class CommandHandlerOperation<TCommand> : MessageHandlerRootOperation<TCommand>
    {
        public CommandHandlerOperation(MicroProcessor processor, IMessageHandler<TCommand> messageHandler, Message<TCommand> message, CancellationToken? token) :
            this(processor, new HandleAsyncMethod<TCommand>(messageHandler), message, token) { }

        public CommandHandlerOperation(MicroProcessor processor, HandleAsyncMethod<TCommand> method, Message<TCommand> message, CancellationToken? token) :
            base(processor, method, message, token) { }        

        // When a MessageHandlerOperationException was thrown when processing a Command, we regard it as a BadRequest, assuming the
        // exception was caused because some business rule was violated or a conflict occurred.
        protected override MicroProcessorOperationException NewMicroProcessorOperationException(MessageHandlerOperationException.WithStackTrace exception) =>
            exception.ToBadRequestException(exception.Message);        
    }
}
