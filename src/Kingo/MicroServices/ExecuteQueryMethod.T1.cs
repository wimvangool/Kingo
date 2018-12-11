using System;
using System.Threading;
using System.Threading.Tasks;

namespace Kingo.MicroServices
{
    internal sealed class ExecuteQueryMethod<TMessageOut> : ExecuteQueryMethodBase<TMessageOut>
    {
        private readonly MicroProcessor _processor;
        private readonly CancellationToken? _token;
        private readonly IQuery<TMessageOut> _query;

        public ExecuteQueryMethod(MicroProcessor processor, CancellationToken? token, IQuery<TMessageOut> query)            
        {
            _processor = processor;
            _token = token;
            _query = query ?? throw new ArgumentNullException(nameof(query));
        }       

        protected override Task<InvokeAsyncResult<TMessageOut>> InvokeAsyncCore(QueryContext context) =>
            _processor.PipelineFactory.CreatePipeline(new QueryDecorator<TMessageOut>(_query, context)).Method.InvokeAsync();

        protected override QueryContext CreateQueryContext() =>
            new QueryContext(_processor.MessageHandlerFactory, _processor.Principal, _token);
    }
}
