using System;
using System.Threading.Tasks;

namespace Kingo.Messaging
{
    /// <summary>
    /// Represents a filter-decorator that will invoke the next filter of the source of message matches
    /// with one of the supported sources; otherwise it will skip it.
    /// </summary>
    public sealed class RequiresMessageSourceFilter : MicroProcessorFilterDecorator
    {
        private readonly MessageSources _sources;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequiresMessageSourceFilter" /> class.
        /// </summary>
        /// <param name="nextFilter">The filter to decorate.</param>
        /// <param name="sources">The sources that are supported by the next filter.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="nextFilter"/> is <c>null</c>.
        /// </exception>
        public RequiresMessageSourceFilter(IMicroProcessorFilter nextFilter, MessageSources sources) :
            base(nextFilter)
        {
            _sources = sources;    
        }

        /// <inheritdoc />
        protected override Task<TResult> HandleOrExecuteAsync<TResult>(MessagePipeline<TResult> pipeline, IMicroProcessorContext context)
        {
            var messageSource = context.Messages.Current?.Source;
            if (messageSource == null || _sources.HasFlag(messageSource.Value))
            {
                return pipeline.InvokeNextFilterAsync(context);
            }
            return pipeline.SkipNextFilterAsync(context);
        }
    }
}
