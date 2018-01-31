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
        internal MicroProcessorFilterAttribute()
        {
            Sources = MessageSources.All;
        }

        /// <summary>
        /// 
        /// </summary>
        public MessageSources Sources
        {
            get;
            set;
        }

        internal abstract void Accept(IMicroProcessorFilterAttributeVisitor visitor);

        /// <inheritdoc />
        public override string ToString() =>
            MicroProcessorPipeline.ToString(this);

        #region [====== IMicroProcessorFilter ======]                                

        /// <inheritdoc /> 
        public virtual Task<HandleAsyncResult> InvokeMessageHandlerAsync<TMessage>(MessageHandler<TMessage> handler, TMessage message, IMicroProcessorContext context) =>
            InvokeMessageHandlerOrQueryAsync(new MessageHandlerWrapper<TMessage>(handler, message), context);

        /// <inheritdoc />
        public virtual Task<ExecuteAsyncResult<TMessageOut>> InvokeQueryAsync<TMessageOut>(Query<TMessageOut> query, IMicroProcessorContext context) =>
            InvokeMessageHandlerOrQueryAsync(new QueryWrapper<TMessageOut>(query), context);

        /// <inheritdoc />
        public virtual Task<ExecuteAsyncResult<TMessageOut>> InvokeQueryAsync<TMessageIn, TMessageOut>(Query<TMessageIn, TMessageOut> query, TMessageIn message, IMicroProcessorContext context) =>
            InvokeMessageHandlerOrQueryAsync(new QueryWrapper<TMessageIn, TMessageOut>(query, message), context);

        /// <summary>
        /// Processes the current command, event or query asynchronously and returns the result.
        /// </summary>
        /// <typeparam name="TResult">Type of the result of the operation.</typeparam>
        /// <param name="handlerOrQuery">A message handler or query that will perform the operation.</param>
        /// <param name="context">Context of the <see cref="IMicroProcessor" /> that is currently handling the message.</param>
        /// <returns>The result of the operation.</returns>
        protected virtual Task<TResult> InvokeMessageHandlerOrQueryAsync<TResult>(MessageHandlerOrQuery<TResult> handlerOrQuery, IMicroProcessorContext context) =>
            handlerOrQuery.InvokeAsync(context);

        #endregion

        #region [====== FilterPipeline ======]

        /// <summary>
        /// Represents a pipeline of filters.
        /// </summary>
        protected sealed class FilterPipeline
        {            
            private readonly Stack<Func<IMicroProcessorFilter, IMicroProcessorFilter>> _filterFactories;
            
            internal FilterPipeline(MessageSources sources)
            {                
                _filterFactories = new Stack<Func<IMicroProcessorFilter, IMicroProcessorFilter>>();
                _filterFactories.Push(filter => new RequiresMessageSourceFilter(filter, sources));
            }

            /// <summary>
            /// Adds the specified <paramref name="filterFactory" /> to this pipeline, representing one filter.
            /// </summary>
            /// <param name="filterFactory">
            /// A delegate that can be used to create one filter decorating another filter.
            /// </param>
            /// <returns>A pipeline that contains the added <paramref name="filterFactory"/>.</returns>
            /// <exception cref="ArgumentNullException">
            /// <paramref name="filterFactory" /> is <c>null</c>.
            /// </exception>
            public FilterPipeline Add(Func<IMicroProcessorFilter, IMicroProcessorFilter> filterFactory)
            {
                if (filterFactory == null)
                {
                    throw new ArgumentNullException(nameof(filterFactory));
                }
                _filterFactories.Push(filterFactory);
                return this;
            }

            internal IMicroProcessorFilter Build(IMicroProcessorFilter filter)
            {
                var pipeline = filter;

                while (_filterFactories.Count > 0)
                {                   
                    pipeline = _filterFactories.Pop().Invoke(pipeline);
                }
                return pipeline;
            }
        }

        internal IMicroProcessorFilter BuildFilterPipeline() =>
            CreateFilterPipeline().Build(this);

        /// <summary>
        /// Creates and returns a pipeline of filter that is placed on top of this filter.
        /// </summary>                
        /// <returns>A pipeline to which a collection of other filters are added.</returns>
        protected virtual FilterPipeline CreateFilterPipeline() =>
            new FilterPipeline(Sources);

        #endregion
    }
}
