using System.Threading.Tasks;

namespace Kingo.MicroServices
{
    internal sealed class CommandProcessor : InternalMessageProcessor<MessageHandlerOperationContext>, ICommandProcessor
    {
        private readonly MessageHandlerOperationContext _context;

        public CommandProcessor(MessageHandlerOperationContext context)
        {
            _context = context;
        }

        protected override MessageHandlerOperationContext Context =>
            _context;

        public Task ExecuteCommandAsync<TCommand>(IMessageHandler<TCommand> messageHandler, TCommand message) =>
            ExecuteCommandAsync(messageHandler, CreateCommand(message));

        private Task ExecuteCommandAsync<TCommand>(IMessageHandler<TCommand> messageHandler, Message<TCommand> message) =>
            new CommandHandlerBranchOperation<TCommand>(_context, messageHandler, message, _context.Token).ExecuteAsync();

        private Message<TCommand> CreateCommand<TCommand>(TCommand message) =>
            CreateMessage(MessageKind.Command, message);
    }
}
