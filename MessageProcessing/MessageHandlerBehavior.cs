
namespace YellowFlare.MessageProcessing
{    
    internal sealed class MessageHandlerBehavior : MessageCommandDecorator
    {
        private readonly IMessageHandlerBehavior _behavior;

        public MessageHandlerBehavior(IMessageCommand command, IMessageHandlerBehavior behavior) : base(command)
        {
            _behavior = behavior;
        }

        public override void Execute()
        {
            _behavior.Execute(Command);
        }
    }
}
