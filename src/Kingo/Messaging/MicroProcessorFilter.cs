using System.Threading.Tasks;

namespace Kingo.Messaging
{
    /// <summary>
    /// Serves as a base class for all filters.
    /// </summary>
    public abstract class MicroProcessorFilter : IMicroProcessorFilter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MicroProcessorFilter" /> class.
        /// </summary>
        /// <param name="stage">Indicates which stage of the pipeline this filter is part of.</param>
        /// <param name="stagePosition">
        /// Indicates which position this filter should have in its stage, relative to all other filters in the specified <paramref name="stage"/>.
        /// </param>
        protected MicroProcessorFilter(MicroProcessorPipelineStage stage = MicroProcessorPipelineStage.ProcessingStage, byte stagePosition = 0)
        {
            Stage = stage;
            StagePosition = stagePosition;
        }

        /// <inheritdoc />
        public MicroProcessorPipelineStage Stage
        {
            get;
        }

        /// <inheritdoc />
        public byte StagePosition
        {
            get;
        }

        #region [====== IMicroProcessorFilter ======]

        /// <inheritdoc />
        public virtual Task<HandleAsyncResult> HandleAsync<TMessage>(MessageHandler<TMessage> handler, TMessage message, IMicroProcessorContext context) =>
            HandleOrExecuteAsync(new MessageHandlerWrapper<TMessage>(handler, message), context);

        /// <inheritdoc />
        public virtual Task<ExecuteAsyncResult<TMessageOut>> ExecuteAsync<TMessageOut>(Query<TMessageOut> query, IMicroProcessorContext context) =>
            HandleOrExecuteAsync(new QueryWrapper<TMessageOut>(query), context);

        /// <inheritdoc />
        public virtual Task<ExecuteAsyncResult<TMessageOut>> ExecuteAsync<TMessageIn, TMessageOut>(Query<TMessageIn, TMessageOut> query, TMessageIn message, IMicroProcessorContext context) =>
            HandleOrExecuteAsync(new QueryWrapper<TMessageIn, TMessageOut>(query, message), context);

        /// <summary>
        /// Processes the current command, event or query asynchronously and returns the result.
        /// </summary>
        /// <typeparam name="TResult">Type of the result of the operation.</typeparam>
        /// <param name="handlerOrQuery">A message handler or query that will perform the operation.</param>
        /// <param name="context">Context of the <see cref="IMicroProcessor" /> that is currently handling the message.</param>
        /// <returns>The result of the operation.</returns>
        protected abstract Task<TResult> HandleOrExecuteAsync<TResult>(MessageHandlerOrQuery<TResult> handlerOrQuery, IMicroProcessorContext context);

        #endregion        
    }
}
