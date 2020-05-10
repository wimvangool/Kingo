using System.Threading;

namespace Kingo.MicroServices
{
    internal sealed class CommandHandlerOperation<TCommand> : MessageHandlerRootOperation<TCommand>
    {
        public CommandHandlerOperation(MicroProcessor processor, IMessageHandler<TCommand> messageHandler, Message<TCommand> message, CancellationToken? token) :
            this(processor, new HandleAsyncMethod<TCommand>(messageHandler), message, token) { }

        public CommandHandlerOperation(MicroProcessor processor, HandleAsyncMethod<TCommand> method, Message<TCommand> message, CancellationToken? token) :
            base(processor, method, message, token) { }
    }
}
