using System.Threading;

namespace Kingo.MicroServices
{
    internal sealed class CommandHandlerOperation<TCommand> : MessageHandlerRootOperation<TCommand>
    {
        public CommandHandlerOperation(MicroProcessor processor, IMessageHandler<TCommand> messageHandler, MessageEnvelope<TCommand> message, CancellationToken? token) :
            this(processor, new HandleAsyncMethod<TCommand>(messageHandler), message, token) { }

        public CommandHandlerOperation(MicroProcessor processor, HandleAsyncMethod<TCommand> method, MessageEnvelope<TCommand> message, CancellationToken? token) :
            base(processor, method, message.ToProcess(MessageKind.Command), token) { }        

        protected override MicroProcessorOperationException NewMicroProcessorOperationException(MessageHandlerOperationException exception) =>
            exception.AsBadRequestException(exception.Message);        
    }
}
