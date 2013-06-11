
namespace YellowFlare.MessageProcessing
{
    internal sealed class MessageSourceFilter<TMessage> : MessageHandlerPipelineDecorator<TMessage> where TMessage : class
    {
        private readonly MessageSources _source;      

        public MessageSourceFilter(IMessageHandlerPipeline<TMessage> handler, MessageSources source) : base(handler)
        {
            _source = source;           
        }

        public override void Handle(TMessage message)
        {
            if (HandlerHandlesMessagesFromSource())
            {
                Handler.Handle(message);
            }
        }

        private bool HandlerHandlesMessagesFromSource()
        {
            HandlesMessagesFromAttribute attribute;
            const bool includeInherited = true;            

            if (TryGetMethodAttributeOfType(includeInherited, out attribute))
            {
                return HandlesMessagesFromSource(attribute.Sources, _source);
            }
            if (TryGetClassAttributeOfType(includeInherited, out attribute))
            {
                return HandlesMessagesFromSource(attribute.Sources, _source);
            }
            return HandlesMessagesFromSource(MessageSources.All, _source);
        }

        private static bool HandlesMessagesFromSource(MessageSources acceptedSources, MessageSources currentSource)
        {
            return (acceptedSources & currentSource) == currentSource;
        }
    }
}
