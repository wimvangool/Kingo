using System.Threading.Tasks;

namespace Kingo.Messaging
{
    /// <summary>
    /// When implemented by a class, represents a pipeline that a <see cref="IMicroProcessor" /> uses to process each message.
    /// </summary>
    public interface IMicroProcessorPipeline
    {
        /// <summary>
        /// Handles the specified <paramref name="message"/> asynchronously.
        /// </summary>
        /// <param name="handler">The handler that will be invoked by this pipeline.</param>
        /// <param name="message">A message.</param>
        /// <param name="context">Context of the <see cref="IMicroProcessor" /> that is currently processing the message.</param>        
        /// <returns>A stream of events that represent the changes that were made by this handler.</returns> 
        Task<HandleAsyncResult> HandleAsync<TMessage>(MessageHandler<TMessage> handler, TMessage message, IMicroProcessorContext context);

        /// <summary>
        /// Executes the specified <paramref name="query"/> asynchronously and returns its result.
        /// </summary>        
        /// <param name="query">The query that will be executed in this pipeline.</param>
        /// <param name="context">Context of the <see cref="IMicroProcessor" /> that is currently processing the message.</param>        
        /// <returns>The result of the specified <paramref name="query"/>.</returns>
        Task<ExecuteAsyncResult<TMessageOut>> ExecuteAsync<TMessageOut>(Query<TMessageOut> query, IMicroProcessorContext context);

        /// <summary>
        /// Executes the specified <paramref name="query"/> asynchronously and returns its result.
        /// </summary>  
        /// <param name="query">The query that will be executed in this pipeline.</param>      
        /// <param name="message">A message.</param>
        /// <param name="context">Context of the <see cref="IMicroProcessor" /> that is currently processing the message.</param>        
        /// <returns>The result of the specified <paramref name="query"/>.</returns>
        Task<ExecuteAsyncResult<TMessageOut>> ExecuteAsync<TMessageIn, TMessageOut>(Query<TMessageIn, TMessageOut> query, TMessageIn message, IMicroProcessorContext context);        
    }
}
