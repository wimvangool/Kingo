
namespace YellowFlare.MessageProcessing
{
    /// <summary>
    /// Represents a factory class that can be used to dynamically create a pipeline on top of a handler.
    /// </summary>        
    public class MessageHandlerPipelineFactory
    {
        internal IMessageHandler<TMessage> CreatePipelineFor<TMessage>(IMessageHandlerWithAttributes<TMessage> handler, MessageProcessorContext context, MessageSources source)
            where TMessage : class
        {
            // On top of the custom pipeline, two infrastructural behaviors are defined:
            // - The stack-manager pushes the current message on the stack, along with it's source.
            // - The source-filter then checks if the specified handler accepts messages from this source,
            //   and if so, uses this pipeline-factory to build the custom pipeline and then handles the message.            
            IMessageHandler<TMessage> pipeline = new MessageSourceFilter<TMessage>(handler, context, this);
            pipeline = new MessageStackManager<TMessage>(pipeline, context, source);
            return pipeline;
        }

        /// <summary>
        /// Creates and returns a message-handler pipeline on top of the specified handler.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message handled by the handler/pipeline.</typeparam>
        /// <param name="handler">The handler containing the actual business logic.</param>
        /// <param name="context">The context in which the handler handles the message.</param>
        /// <returns>A message-handler pipeline.</returns>
        protected internal virtual IMessageHandler<TMessage> CreatePipelineFor<TMessage>(IMessageHandlerWithAttributes<TMessage> handler, MessageProcessorContext context)
            where TMessage : class
        {
            return handler;
        }           
    }
}
