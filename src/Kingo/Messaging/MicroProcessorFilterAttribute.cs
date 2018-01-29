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

        #region [====== Pipeline ======]

        /// <summary>
        /// Represents a pipeline of filters.
        /// </summary>
        protected sealed class Pipeline
        {            
            private readonly Stack<Func<IMicroProcessorFilter, IMicroProcessorFilter>> _filterFactories;

            internal Pipeline()
            {                
                _filterFactories = new Stack<Func<IMicroProcessorFilter, IMicroProcessorFilter>>();
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
            public Pipeline Add(Func<IMicroProcessorFilter, IMicroProcessorFilter> filterFactory)
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

        internal IMicroProcessorFilter CreateFilterPipeline() =>
            CreateFilterPipeline(new Pipeline()).Build(this);

        /// <summary>
        /// Creates and returns a pipeline of filter that is placed on top of this filter.
        /// </summary>
        /// <param name="pipeline">A pipeline of filters.</param>
        /// <returns>A pipeline to which a collection of other filters are added.</returns>
        protected virtual Pipeline CreateFilterPipeline(Pipeline pipeline) =>
            pipeline;

        #endregion
    }
}
