
namespace YellowFlare.MessageProcessing
{
    internal sealed class ExternalMessageHandler<TMessage> : IMessageHandler<TMessage>, IExternalMessageHandler<TMessage>
        where TMessage : class
    {
        private readonly IExternalMessageHandler<TMessage> _handler;

        public ExternalMessageHandler(IExternalMessageHandler<TMessage> handler)
        {
            _handler = handler;
        }

        object IMessageHandler<TMessage>.Handler
        {
            get { return _handler; }
        }

        void IMessageHandler<TMessage>.Handle(TMessage message)
        {
            _handler.Handle(message);
        }

        public void Handle(TMessage message)
        {
            _handler.Handle(message);
        }
    }
}
