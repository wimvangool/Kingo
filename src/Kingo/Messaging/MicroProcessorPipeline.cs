using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kingo.Messaging
{
    /// <summary>
    /// Serves as a base class for a <see cref="IMicroProcessorPipeline" /> implementation that
    /// does not discriminate between handling a message or executing a query.
    /// </summary>
    public abstract class MicroProcessorPipeline : IMicroProcessorPipeline
    {
        #region [====== IMicroProcessorPipeline ======]

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

        #region [====== Build Pipeline ======]        

        internal static MessageHandler<TMessage> BuildPipeline<TMessage>(IReadOnlyList<IMicroProcessorPipeline> pipelines, MessageHandler<TMessage> handler)
        {
            var pipeline = handler;

            for (int index = pipelines.Count - 1; index >= 0; index--)
            {
                pipeline = new MessageHandlerConnector<TMessage>(pipeline, pipelines[index]);
            }
            return pipeline;
        }      

        internal static Query<TMessageOut> BuildPipeline<TMessageOut>(IReadOnlyList<IMicroProcessorPipeline> pipelines, QueryContext context, IQuery<TMessageOut> query)
        {
            Query<TMessageOut> pipeline = new QueryDecorator<TMessageOut>(context, query);

            for (int index = pipelines.Count - 1; index >= 0; index--)
            {
                pipeline = new QueryConnector<TMessageOut>(pipeline, pipelines[index]);
            }
            return pipeline;
        }    

        internal static Query<TMessageIn, TMessageOut> BuildPipeline<TMessageIn, TMessageOut>(IReadOnlyList<IMicroProcessorPipeline> pipelines, QueryContext context, IQuery<TMessageIn, TMessageOut> query)
        {
            Query<TMessageIn, TMessageOut> pipeline = new QueryDecorator<TMessageIn, TMessageOut>(context, query);

            for (int index = pipelines.Count - 1; index >= 0; index--)
            {
                pipeline = new QueryConnector<TMessageIn, TMessageOut>(pipeline, pipelines[index]);
            }
            return pipeline;
        }

        #endregion
    }
}
