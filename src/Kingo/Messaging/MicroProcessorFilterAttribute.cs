using System;
using System.Threading.Tasks;

namespace Kingo.Messaging
{
    /// <summary>
    /// Serves as a base class for all filters that can also be declared as attributes.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public abstract class MicroProcessorFilterAttribute : Attribute, IMicroProcessorFilter
    {
        #region [====== Relay ======]

        private sealed class Relay : MicroProcessorFilter
        {
            private readonly MicroProcessorFilterAttribute _attribute;

            public Relay(MicroProcessorPipelineStage stage, byte stagePosition, MicroProcessorFilterAttribute attribute) :
                base(stage, stagePosition)
            {
                _attribute = attribute;
            }

            protected override Task<TResult> HandleOrExecuteAsync<TResult>(MessageHandlerOrQuery<TResult> handlerOrQuery, IMicroProcessorContext context) =>
                _attribute.HandleOrExecuteAsync(handlerOrQuery, context);
        }

        #endregion

        private readonly Relay _implementation;

        /// <summary>
        /// Initializes a new instance of the <see cref="MicroProcessorFilterAttribute" /> class.
        /// </summary>
        /// <param name="stage">Indicates which stage of the pipeline this filter is part of.</param>
        /// <param name="stagePosition">
        /// Indicates which position this filter should have in its stage, relative to all other filters in the specified <paramref name="stage"/>.
        /// </param>
        protected MicroProcessorFilterAttribute(MicroProcessorPipelineStage stage = MicroProcessorPipelineStage.ProcessingStage, byte stagePosition = 0)
        {
            _implementation = new Relay(stage, stagePosition, this);
        }

        /// <inheritdoc />
        public MicroProcessorPipelineStage Stage =>
            _implementation.Stage;

        /// <inheritdoc />
        public byte StagePosition =>
            _implementation.StagePosition;

        #region [====== IMicroProcessorFilter ======]

        /// <inheritdoc />
        public virtual Task<HandleAsyncResult> HandleAsync<TMessage>(MessageHandler<TMessage> handler, TMessage message, IMicroProcessorContext context) =>
            _implementation.HandleAsync(handler, message, context);

        /// <inheritdoc />
        public virtual Task<ExecuteAsyncResult<TMessageOut>> ExecuteAsync<TMessageOut>(Query<TMessageOut> query, IMicroProcessorContext context) =>
            _implementation.ExecuteAsync(query, context);

        /// <inheritdoc />
        public virtual Task<ExecuteAsyncResult<TMessageOut>> ExecuteAsync<TMessageIn, TMessageOut>(Query<TMessageIn, TMessageOut> query, TMessageIn message, IMicroProcessorContext context) =>
            _implementation.ExecuteAsync(query, message, context);

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
