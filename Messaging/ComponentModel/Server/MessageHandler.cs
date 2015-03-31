namespace System.ComponentModel.Server
{
    internal sealed class MessageHandler : IMessageHandler
    {
        private readonly IMessageHandler _nextHandler;
        private readonly MessageHandlerModule _nextModule;

        internal MessageHandler(IMessageHandler nextHandler, MessageHandlerModule nextModule)
        {
            _nextHandler = nextHandler;
            _nextModule = nextModule;
        }

        public IMessage Message
        {
            get { return _nextHandler.Message; }
        }

        public void Invoke()
        {
            _nextModule.Invoke(_nextHandler);
        }
    }
}
