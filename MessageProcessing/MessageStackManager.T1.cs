
namespace YellowFlare.MessageProcessing
{
    internal sealed class MessageStackManager<TMessage> : MessageHandlerPipelineDecorator<TMessage> where TMessage : class
    {        
        private readonly MessageProcessorContext _context;
        private readonly MessageSources _source;

        public MessageStackManager(IMessageHandlerPipeline<TMessage> handler, MessageSources source, MessageProcessorContext context)
            : base(handler)
        {                        
            _context = context;
            _source = source;
        }

        public override void Handle(TMessage message)
        {
            _context.PushMessage(new Message(message, _source));

            try
            {
                Handler.Handle(message);
            }
            finally
            {
                _context.PopMessage();
            }
        }
    }
}
