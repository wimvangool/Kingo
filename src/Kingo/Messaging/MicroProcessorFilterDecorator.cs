using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kingo.Messaging
{
    /// <summary>
    /// When implemented, represents a filter that decorates, and therefore extends the functionality of, another filter in the pipeline.
    /// </summary>
    public abstract class MicroProcessorFilterDecorator : IMicroProcessorFilter
    {
        #region [====== MessagePipeline ======]

        /// <summary>
        /// Represents the (remaining) message pipeline that was constructed for a specific message.
        /// </summary>        
        protected abstract class MessagePipeline<TResult>
        {            
            /// <summary>
            /// Invokes the appropriate method on the associated <see cref="NextFilter"/> and returns its result.
            /// </summary>
            /// <param name="context">Context of the <see cref="IMicroProcessor" /> that is currently handling the message.</param>
            /// <returns>The result of the operation.</returns>
            public abstract Task<TResult> InvokeNextFilterAsync(IMicroProcessorContext context);

            /// <summary>
            /// Skips the next filter by invoking the message handler or query directly and returns its result.
            /// </summary>
            /// <param name="context">Context of the <see cref="IMicroProcessor" /> that is currently handling the message.</param>
            /// <returns>The result of the operation.</returns>
            public abstract Task<TResult> SkipNextFilterAsync(IMicroProcessorContext context);
        }

        #endregion

        #region [====== MessageHandlerPipeline<TMessage> ======]

        /// <summary>
        /// Represents the (remaining) message pipeline that was constructed for a specific message.
        /// </summary>        
        protected sealed class MessageHandlerPipeline<TMessage> : MessagePipeline<HandleAsyncResult>
        {
            private readonly IMicroProcessorFilter _nextFilter;
            private readonly MessageHandler<TMessage> _handler;
            private readonly TMessage _message;

            internal MessageHandlerPipeline(IMicroProcessorFilter nextFilter, MessageHandler<TMessage> handler, TMessage message)                
            {
                _nextFilter = nextFilter;
                _handler = handler;
                _message = message;
            }

            /// <inheritdoc />
            public override Task<HandleAsyncResult> InvokeNextFilterAsync(IMicroProcessorContext context) =>
                _nextFilter.HandleAsync(_handler, _message, context);

            public override Task<HandleAsyncResult> SkipNextFilterAsync(IMicroProcessorContext context) =>
                _handler.HandleAsync(_message, context);
        }

        #endregion

        #region [====== QueryPipeline<TMessageOut> ======]

        /// <summary>
        /// Represents the (remaining) message pipeline that was constructed for a specific message.
        /// </summary>        
        protected sealed class QueryPipeline<TMessageOut> : MessagePipeline<ExecuteAsyncResult<TMessageOut>>
        {
            private readonly IMicroProcessorFilter _nextFilter;
            private readonly Query<TMessageOut> _query;

            internal QueryPipeline(IMicroProcessorFilter nextFilter, Query<TMessageOut> query)                
            {
                _nextFilter = nextFilter;
                _query = query;
            }

            /// <inheritdoc />
            public override Task<ExecuteAsyncResult<TMessageOut>> InvokeNextFilterAsync(IMicroProcessorContext context) =>
                _nextFilter.ExecuteAsync(_query, context);

            /// <inheritdoc />
            public override Task<ExecuteAsyncResult<TMessageOut>> SkipNextFilterAsync(IMicroProcessorContext context) =>
                _query.ExecuteAsync(context);
        }

        #endregion

        #region [====== QueryPipeline<TMessageIn, TMessageOut> ======]

        /// <summary>
        /// Represents the (remaining) message pipeline that was constructed for a specific message.
        /// </summary>        
        protected sealed class QueryPipeline<TMessageIn, TMessageOut> : MessagePipeline<ExecuteAsyncResult<TMessageOut>>
        {
            private readonly IMicroProcessorFilter _nextFilter;
            private readonly Query<TMessageIn, TMessageOut> _query;
            private readonly TMessageIn _message;

            internal QueryPipeline(IMicroProcessorFilter nextFilter, Query<TMessageIn, TMessageOut> query, TMessageIn message)               
            {
                _nextFilter = nextFilter;
                _query = query;
                _message = message;
            }

            /// <inheritdoc />
            public override Task<ExecuteAsyncResult<TMessageOut>> InvokeNextFilterAsync(IMicroProcessorContext context) =>
                _nextFilter.ExecuteAsync(_query, _message, context);

            /// <inheritdoc />
            public override Task<ExecuteAsyncResult<TMessageOut>> SkipNextFilterAsync(IMicroProcessorContext context) =>
                _query.ExecuteAsync(_message, context);
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="MicroProcessorFilterDecorator" /> class.
        /// </summary>
        /// <param name="nextFilter">The filter to decorate.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="nextFilter"/> is <c>null</c>.
        /// </exception>
        protected MicroProcessorFilterDecorator(IMicroProcessorFilter nextFilter)
        {
            NextFilter = nextFilter ?? throw new ArgumentNullException(nameof(nextFilter));
        }

        /// <summary>
        /// The decorated filter, which is next in line to invoke, if required.
        /// </summary>
        protected IMicroProcessorFilter NextFilter
        {
            get;
        }

        /// <inheritdoc />
        public override string ToString() =>
            MicroProcessorPipeline.ToString(this, NextFilter);

        /// <inheritdoc />
        Task<HandleAsyncResult> IMicroProcessorFilter.HandleAsync<TMessage>(MessageHandler<TMessage> handler, TMessage message, IMicroProcessorContext context) =>
            HandleAsync(new MessageHandlerPipeline<TMessage>(NextFilter, handler, message), context);

        /// <summary>
        /// Invokes the next filter or skips it by invoking the appropriate method on the specified <paramref name="pipeline"/>.
        /// </summary>        
        /// <param name="pipeline">The remaining piece of the pipeline.</param>
        /// <param name="context">Context of the <see cref="IMicroProcessor" /> that is currently handling the message.</param>
        /// <returns>The result of the operation.</returns>
        protected virtual Task<HandleAsyncResult> HandleAsync<TMessage>(MessageHandlerPipeline<TMessage> pipeline, IMicroProcessorContext context) =>
            HandleOrExecuteAsync(pipeline, context);

        /// <inheritdoc />
        Task<ExecuteAsyncResult<TMessageOut>> IMicroProcessorFilter.ExecuteAsync<TMessageOut>(Query<TMessageOut> query, IMicroProcessorContext context) =>
            ExecuteAsync(new QueryPipeline<TMessageOut>(NextFilter, query), context);

        /// <summary>
        /// Invokes the next filter or skips it by invoking the appropriate method on the specified <paramref name="pipeline"/>.
        /// </summary>        
        /// <param name="pipeline">The remaining piece of the pipeline.</param>
        /// <param name="context">Context of the <see cref="IMicroProcessor" /> that is currently handling the message.</param>
        /// <returns>The result of the operation.</returns>
        protected virtual Task<ExecuteAsyncResult<TMessageOut>> ExecuteAsync<TMessageOut>(QueryPipeline<TMessageOut> pipeline, IMicroProcessorContext context) =>
            HandleOrExecuteAsync(pipeline, context);

        /// <inheritdoc />
        Task<ExecuteAsyncResult<TMessageOut>> IMicroProcessorFilter.ExecuteAsync<TMessageIn, TMessageOut>(Query<TMessageIn, TMessageOut> query, TMessageIn message, IMicroProcessorContext context) =>
            ExecuteAsync(new QueryPipeline<TMessageIn, TMessageOut>(NextFilter, query, message), context);

        /// <summary>
        /// Invokes the next filter or skips it by invoking the appropriate method on the specified <paramref name="pipeline"/>.
        /// </summary>        
        /// <param name="pipeline">The remaining piece of the pipeline.</param>
        /// <param name="context">Context of the <see cref="IMicroProcessor" /> that is currently handling the message.</param>
        /// <returns>The result of the operation.</returns>
        protected virtual Task<ExecuteAsyncResult<TMessageOut>> ExecuteAsync<TMessageIn, TMessageOut>(QueryPipeline<TMessageIn, TMessageOut> pipeline, IMicroProcessorContext context) =>
            HandleOrExecuteAsync(pipeline, context);

        /// <summary>
        /// Invokes the next filter or skips it by invoking the appropriate method on the specified <paramref name="pipeline"/>.
        /// </summary>        
        /// <param name="pipeline">The remaining piece of the pipeline.</param>
        /// <param name="context">Context of the <see cref="IMicroProcessor" /> that is currently handling the message.</param>
        /// <returns>The result of the operation.</returns>
        protected virtual Task<TResult> HandleOrExecuteAsync<TResult>(MessagePipeline<TResult> pipeline, IMicroProcessorContext context) =>
            pipeline.InvokeNextFilterAsync(context);
    }
}
