using System.Threading.Tasks;

namespace Kingo.Messaging
{
    /// <summary>
    /// When implemented by a class, represents a filter that a <see cref="IMicroProcessor" /> uses to process each message.
    /// </summary>
    public interface IMicroProcessorFilter
    {        
        /// <summary>
        /// Invokes the specified <paramref name="handler" /> with the specified <paramref name="message"/> and <paramref name="context" />.
        /// </summary>
        /// <param name="handler">The handler that will be invoked by this filter.</param>
        /// <param name="message">A message.</param>
        /// <param name="context">Context of the <see cref="IMicroProcessor" /> that is currently processing the message.</param>        
        /// <returns>A stream of events that represent the changes that were made by this handler.</returns> 
        Task<HandleAsyncResult> InvokeMessageHandlerAsync<TMessage>(MessageHandler<TMessage> handler, TMessage message, IMicroProcessorContext context);

        /// <summary>
        /// Invokes the specified <paramref name="query"/> with the specified <paramref name="context" />.
        /// </summary>        
        /// <param name="query">The query that will be invoked by this filter.</param>
        /// <param name="context">Context of the <see cref="IMicroProcessor" /> that is currently processing the message.</param>        
        /// <returns>The result of the specified <paramref name="query"/>.</returns>
        Task<ExecuteAsyncResult<TMessageOut>> InvokeQueryAsync<TMessageOut>(Query<TMessageOut> query, IMicroProcessorContext context);

        /// <summary>
        /// Invokes the specified <paramref name="query"/> with the specified <paramref name="message"/> and <paramref name="context" />.
        /// </summary>  
        /// <param name="query">The query that will be invoked by this filter.</param>      
        /// <param name="message">A message.</param>
        /// <param name="context">Context of the <see cref="IMicroProcessor" /> that is currently processing the message.</param>        
        /// <returns>The result of the specified <paramref name="query"/>.</returns>
        Task<ExecuteAsyncResult<TMessageOut>> InvokeQueryAsync<TMessageIn, TMessageOut>(Query<TMessageIn, TMessageOut> query, TMessageIn message, IMicroProcessorContext context);        
    }
}
