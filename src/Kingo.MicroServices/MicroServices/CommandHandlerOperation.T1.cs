using System.Threading;

namespace Kingo.MicroServices
{
    internal sealed class CommandHandlerOperation<TCommand> : MessageHandlerRootOperation<TCommand>
    {
        public CommandHandlerOperation(MicroProcessor processor, IMessageHandler<TCommand> messageHandler, TCommand message, CancellationToken? token) :
            this(processor, new HandleAsyncMethod<TCommand>(messageHandler), message, token) { }

        internal CommandHandlerOperation(MicroProcessor processor, HandleAsyncMethod<TCommand> method, TCommand message, CancellationToken? token) :
            base(processor, method, new Message<TCommand>(message, MessageKind.Command), token) { }        

        protected override MicroProcessorOperationException NewMicroProcessorOperationException(MessageHandlerOperationException exception) =>
            exception.AsBadRequestException(exception.Message);        
    }
}
