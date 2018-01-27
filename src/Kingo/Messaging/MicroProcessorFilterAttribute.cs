using System;
using System.Threading.Tasks;

namespace Kingo.Messaging
{
    /// <summary>
    /// Serves as a base class for all filters that can be part of the <see cref="IMicroProcessor" /> pipeline.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public abstract class MicroProcessorFilterAttribute : Attribute, IMicroProcessorFilter
    {                       
        internal MicroProcessorFilterAttribute() { }

        internal abstract void Accept(IMicroProcessorFilterAttributeVisitor visitor);

        /// <inheritdoc />
        public override string ToString() =>
            MicroProcessorPipeline.ToString(this);

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
        protected virtual Task<TResult> HandleOrExecuteAsync<TResult>(MessageHandlerOrQuery<TResult> handlerOrQuery, IMicroProcessorContext context) =>
            handlerOrQuery.HandleMessageOrExecuteQueryAsync(context);

        #endregion

        /// <summary>
        /// Creates a mini-pipeline based on this filter that will be integrated into the <see cref="IMicroProcessor" />'s pipeline to process each message.
        /// Returns this filter by default.
        /// </summary>
        /// <returns>A filter or filter pipeline that will be integrated into the <see cref="IMicroProcessor"/>'s pipeline.</returns>
        protected internal virtual IMicroProcessorFilter CreateFilterPipeline() =>
            this;
    }
}
