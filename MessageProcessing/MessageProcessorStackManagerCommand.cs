
namespace YellowFlare.MessageProcessing
{
    internal sealed class MessageProcessorStackManagerCommand : MessageCommandDecorator
    {        
        private readonly MessageProcessorContext _context;

        public MessageProcessorStackManagerCommand(IMessageCommand command, MessageProcessorContext context) : base(command)
        {            
            _context = context;
        }       

        public override void Execute()
        {
            _context.PushCommand(Command);

            try
            {
                Command.Execute();
            }
            finally
            {
                _context.PopCommand();
            }
        }
    }
}
