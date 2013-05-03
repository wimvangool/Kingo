
namespace YellowFlare.MessageProcessing
{
    internal abstract class MessageCommandDecorator : IMessageCommand
    {
        private readonly IMessageCommand _command;

        protected MessageCommandDecorator(IMessageCommand command)
        {
            _command = command;
        }

        public object Handler
        {
            get { return _command.Handler; }
        }

        public object Message
        {
            get { return _command.Message; }
        }

        protected IMessageCommand Command
        {
            get { return _command; }
        }

        public abstract void Execute();
    }
}
