using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kingo.Messaging
{
    /// <summary>
    /// Serves as a base class for all filters that can be part of the <see cref="IMicroProcessor" /> pipeline.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public abstract class MicroProcessorFilterAttribute : Attribute, IMicroProcessorFilter
    {                     
        /// <summary>
        /// Initializes a new instance of the <see cref="MicroProcessorFilterAttribute" /> class.
        /// </summary>
        protected MicroProcessorFilterAttribute()
        {
            Sources = MessageSources.All;
        }

        /// <summary>                 
        /// Indicates for which message sources this filter will be used.
        /// </summary>
        public MessageSources Sources
        {
            get;
            set;
        }

        /// <inheritdoc />
        public virtual bool IsEnabled(IMicroProcessorContext context) =>
            Sources.HasFlag(context.StackTrace.CurrentSource);

        internal abstract void Accept(IMicroProcessorFilterAttributeVisitor visitor);

        /// <inheritdoc />
        public override string ToString() =>
            MicroProcessorPipeline.ToString(this);

        #region [====== IMicroProcessorFilter ======]                                

        /// <inheritdoc /> 
        public virtual Task<InvokeAsyncResult<IMessageStream>> InvokeMessageHandlerAsync(MessageHandler handler, MicroProcessorContext context) =>
            InvokeMessageHandlerOrQueryAsync(handler, context);

        /// <inheritdoc />
        public virtual Task<InvokeAsyncResult<TMessageOut>> InvokeQueryAsync<TMessageOut>(Query<TMessageOut> query, MicroProcessorContext context) =>
            InvokeMessageHandlerOrQueryAsync(query, context);        

        /// <summary>
        /// Invokes the specified <paramref name="handlerOrQuery"/> and returns it result, unless it's <see cref="MessageHandlerOrQuery{TResult}.Yield(TResult)"/> method is used.
        /// </summary>
        /// <typeparam name="TResult">Type of the result of the operation.</typeparam>
        /// <param name="handlerOrQuery">A message handler or query that will perform the operation.</param>
        /// <param name="context">Context of the <see cref="IMicroProcessor" /> that is currently handling the message.</param>
        /// <returns>The result of the operation.</returns>
        protected virtual Task<InvokeAsyncResult<TResult>> InvokeMessageHandlerOrQueryAsync<TResult>(MessageHandlerOrQuery<TResult> handlerOrQuery, MicroProcessorContext context) =>
            handlerOrQuery.InvokeAsync(context);

        #endregion        
    }
}
