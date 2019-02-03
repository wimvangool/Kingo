using System.Threading.Tasks;

namespace Kingo.MicroServices
{
    /// <summary>
    /// When implemented by a class, represents a filter that a <see cref="IMicroProcessor" /> uses to process each message.
    /// </summary>
    public interface IMicroProcessorFilter
    {
        /// <summary>
        /// Returns the stage of the pipeline into which this filter is placed and used.
        /// </summary>
        MicroProcessorFilterStage Stage
        {
            get;
        }

        /// <summary>
        /// Determines whether or not this filter should be used/invoked in the pipeline based on the specified <paramref name="context"/>.
        /// </summary>
        /// <param name="context">The context in which the processor is currently executing the pipeline.</param>
        /// <returns><c>true</c> if this filter should be used; otherwise <c>false</c>.</returns>
        bool IsEnabled(MicroProcessorContext context);

        /// <summary>
        /// Invokes the specified <paramref name="handler" />.
        /// </summary>
        /// <param name="handler">The handler that will be invoked by this filter.</param>
        /// <returns>A stream of events that represent the changes that were made by this handler.</returns> 
        Task<InvokeAsyncResult<MessageStream>> InvokeMessageHandlerAsync(MessageHandler handler);

        /// <summary>
        /// Invokes the specified <paramref name="query"/> or returns a cached result.
        /// </summary>
        /// <param name="query">The query that will be invoked by this filter (if no cached result is returned).</param>
        /// <returns>The result of the specified <paramref name="query"/> or a cached result.</returns>
        Task<InvokeAsyncResult<TResponse>> InvokeQueryAsync<TResponse>(Query<TResponse> query);              
    }
}
