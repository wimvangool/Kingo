using System.Threading.Tasks;

namespace Kingo.Messaging
{
    /// <summary>
    /// When implemented by a class, represents a filter that a <see cref="IMicroProcessor" /> uses to process each message.
    /// </summary>
    public interface IMicroProcessorFilter
    {
        /// <summary>
        /// Determines whether or not this filter should be used/invoked in the pipeline based on the specified <paramref name="context"/>.
        /// </summary>
        /// <param name="context">The context in which the processor is currently executing the pipeline.</param>
        /// <returns><c>true</c> if this filter should be used; otherwise <c>false</c>.</returns>
        bool IsEnabled(IMicroProcessorContext context);

        /// <summary>
        /// Invokes the specified <paramref name="handler" /> using the specified <paramref name="context" />.
        /// </summary>
        /// <param name="handler">The handler that will be invoked by this filter.</param>        
        /// <param name="context">Context of the <see cref="IMicroProcessor" /> that is currently processing the message.</param>        
        /// <returns>A stream of events that represent the changes that were made by this handler.</returns> 
        Task<InvokeAsyncResult<IMessageStream>> InvokeMessageHandlerAsync(MessageHandler handler, MicroProcessorContext context);

        /// <summary>
        /// Invokes the specified <paramref name="query"/> with the specified <paramref name="context" />.
        /// </summary>        
        /// <param name="query">The query that will be invoked by this filter.</param>
        /// <param name="context">Context of the <see cref="IMicroProcessor" /> that is currently processing the message.</param>        
        /// <returns>The result of the specified <paramref name="query"/>.</returns>
        Task<InvokeAsyncResult<TMessageOut>> InvokeQueryAsync<TMessageOut>(Query<TMessageOut> query, MicroProcessorContext context);              
    }
}
