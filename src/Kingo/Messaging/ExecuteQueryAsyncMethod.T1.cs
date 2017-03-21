using System.Threading;
using System.Threading.Tasks;

namespace Kingo.Messaging
{
    internal sealed class ExecuteQueryAsyncMethod<TMessageOut> : ExecuteAsyncMethod<TMessageOut>
    {
        public static Task<TMessageOut> Invoke(MicroProcessor processor, IQuery<TMessageOut> query, CancellationToken? token) =>
            Invoke(new ExecuteQueryAsyncMethod<TMessageOut>(processor, new QueryContext(token), query));

        private readonly IQuery<TMessageOut> _query;

        private ExecuteQueryAsyncMethod(MicroProcessor processor, QueryContext context, IQuery<TMessageOut> query)
        {
            Processor = processor;
            Context = context;

            _query = query;
        }

        protected override MicroProcessor Processor
        {
            get;
        }

        protected override QueryContext Context
        {
            get;
        }

        protected override Task<ExecuteAsyncResult<TMessageOut>> InvokeCore() =>
            MicroProcessorPipeline.BuildPipeline(Processor.ProcessorPipeline, Context, _query).ExecuteAsync(Context);
    }
}
