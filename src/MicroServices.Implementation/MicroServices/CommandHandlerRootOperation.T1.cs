using System.Threading;

namespace Kingo.MicroServices
{
    internal sealed class CommandHandlerRootOperation<TCommand> : MessageHandlerRootOperation<TCommand>
    {
        public CommandHandlerRootOperation(MicroProcessor processor, IMessageHandler<TCommand> messageHandler, Message<TCommand> message, CancellationToken? token) :
            this(processor, new HandleAsyncMethod<TCommand>(messageHandler), message, token) { }

        public CommandHandlerRootOperation(MicroProcessor processor, HandleAsyncMethod<TCommand> method, Message<TCommand> message, CancellationToken? token) :
            base(processor, method, message, token) { }
    }
}
