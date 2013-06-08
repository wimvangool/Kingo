
namespace YellowFlare.MessageProcessing
{
    internal sealed class MessageSourceFilter<TMessage> : IMessageHandler<TMessage> where TMessage : class
    {
        private readonly IMessageHandlerWithAttributes<TMessage> _handler;
        private readonly MessageProcessorContext _context;
        private readonly MessageHandlerPipelineFactory _pipelineFactory;

        public MessageSourceFilter(IMessageHandlerWithAttributes<TMessage> handler, MessageProcessorContext context, MessageHandlerPipelineFactory pipelineFactory)
        {
            _handler = handler;          
            _context = context;
            _pipelineFactory = pipelineFactory;
        }

        public void Handle(TMessage message)
        {
            if (HandlesMessagesFromSource(_handler, _context.PeekMessage().Source))
            {
                _pipelineFactory.CreatePipelineFor(_handler, _context).Handle(message);
            }
        }

        private static bool HandlesMessagesFromSource(IMessageHandlerWithAttributes<TMessage> handler, MessageSources source)
        {
            HandlesMessagesFromAttribute attribute;
            const bool includeInherited = true;            

            if (handler.TryGetMethodAttributeOfType(includeInherited, out attribute))
            {
                return HandlesMessagesFromSource(attribute.Sources, source);
            }
            if (handler.TryGetClassAttributeOfType(includeInherited, out attribute))
            {
                return HandlesMessagesFromSource(attribute.Sources, source);
            }
            return HandlesMessagesFromSource(MessageSources.All, source);
        }

        private static bool HandlesMessagesFromSource(MessageSources supportedSources, MessageSources source)
        {
            return (supportedSources & source) == source;
        }
    }
}
