
namespace YellowFlare.MessageProcessing
{
    /// <summary>
    /// Represents a factory class that can be used to dynamically create a pipeline on top of a handler.
    /// </summary>        
    public class MessageHandlerPipelineFactory
    {
        internal IMessageHandler<TMessage> CreatePipeline<TMessage>(IMessageHandlerPipeline<TMessage> handler, MessageProcessorContext context, MessageSources source)
            where TMessage : class
        {
            // On top of the optional custom pipeline, two infrastructural behaviors are defined that prevent
            // unnecessary calls to handlers that do not support the current source (MessageSourceFilter) and
            // manage the message-stack (MessageStackManager).
            IMessageHandlerPipeline<TMessage> pipeline = new LazyMessageHandlerPipeline<TMessage>(handler, this);
            pipeline = new MessageStackManager<TMessage>(pipeline, source, context);
            pipeline = new MessageSourceFilter<TMessage>(pipeline, source);
            return pipeline;
        }

        /// <summary>
        /// Creates and returns a message-handler pipeline on top of the specified handler.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message handled by the handler/pipeline.</typeparam>
        /// <param name="pipeline">The handler containing the actual business logic.</param>        
        /// <returns>A message-handler pipeline.</returns>
        protected internal virtual IMessageHandler<TMessage> CreatePipeline<TMessage>(IMessageHandlerPipeline<TMessage> pipeline)
            where TMessage : class
        {
            return pipeline;
        }           
    }
}
