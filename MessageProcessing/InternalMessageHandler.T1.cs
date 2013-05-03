
namespace YellowFlare.MessageProcessing
{
    internal sealed class InternalMessageHandler<TMessage> : IMessageHandler<TMessage>, IInternalMessageHandler<TMessage>
        where TMessage : class
    {
        private readonly IInternalMessageHandler<TMessage> _handler;

        public InternalMessageHandler(IInternalMessageHandler<TMessage> handler)
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
