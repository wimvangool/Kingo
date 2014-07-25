
namespace System.ComponentModel.Messaging.Server
{
    /// <summary>
    /// When implemented by a class, constructs a pipeline on top of every message-handler.
    /// </summary>
    public interface IMessageHandlerPipelineFactory
    {
        /// <summary>
        /// Creates and returns a message-handler pipeline on top of the specified handler.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message handled by the handler/pipeline.</typeparam>
        /// <param name="pipeline">The handler containing the actual business logic.</param>   
        /// <param name="context">Context in which the handler will be executed.</param>     
        /// <returns>A message-handler pipeline.</returns>
        IMessageHandler<TMessage> CreatePipeline<TMessage>(IMessageHandlerPipeline<TMessage> pipeline, UnitOfWorkContext context) where TMessage : class;        
    }
}
