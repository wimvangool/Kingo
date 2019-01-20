using System;
using System.Threading;
using System.Threading.Tasks;

namespace Kingo.MicroServices
{
    internal sealed class ExecuteQueryMethod<TResponse> : ExecuteQueryMethodBase<TResponse>
    {
        private readonly MicroProcessor _processor;
        private readonly CancellationToken? _token;
        private readonly IQuery<TResponse> _query;

        public ExecuteQueryMethod(MicroProcessor processor, CancellationToken? token, IQuery<TResponse> query)            
        {
            _processor = processor;
            _token = token;
            _query = query ?? throw new ArgumentNullException(nameof(query));
        }       

        protected override Task<InvokeAsyncResult<TResponse>> InvokeAsyncCore(QueryContext context) =>
            _processor.Pipeline.CreatePipeline(new QueryDecorator<TResponse>(_query, context)).Method.InvokeAsync();

        protected override QueryContext CreateQueryContext() =>
            new QueryContext(_processor.ServiceProvider, _processor.Principal, _token);
    }
}
