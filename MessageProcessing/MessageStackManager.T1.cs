
namespace YellowFlare.MessageProcessing
{
    internal sealed class MessageStackManager<TMessage> : IMessageHandler<TMessage> where TMessage : class
    {
        private readonly IMessageHandler<TMessage> _handler;
        private readonly MessageProcessorContext _context;
        private readonly MessageSources _source;

        public MessageStackManager(IMessageHandler<TMessage> handler, MessageProcessorContext context, MessageSources source)
        {            
            _handler = handler;
            _context = context;
            _source = source;
        }

        public void Handle(TMessage message)
        {
            _context.PushMessage(new Message(message, _source));

            try
            {
                _handler.Handle(message);
            }
            finally
            {
                _context.PopMessage();
            }
        }
    }
}
