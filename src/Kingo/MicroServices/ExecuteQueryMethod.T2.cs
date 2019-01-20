using System;
using System.Threading;
using System.Threading.Tasks;

namespace Kingo.MicroServices
{
    internal sealed class ExecuteQueryMethod<TRequest, TResponse> : ExecuteQueryMethodBase<TResponse>
    {
        private readonly MicroProcessor _processor;
        private readonly CancellationToken? _token;
        private readonly IQuery<TRequest, TResponse> _query;
        private readonly TRequest _message;

        public ExecuteQueryMethod(MicroProcessor processor, CancellationToken? token, IQuery<TRequest, TResponse> query, TRequest message)            
        {
            if (ReferenceEquals(message, null))
            {
                throw new ArgumentNullException(nameof(message));
            }
            _processor = processor;
            _token = token;
            _query = query ?? throw new ArgumentNullException(nameof(query));
            _message = message;
        }        

        protected override Task<InvokeAsyncResult<TResponse>> InvokeAsyncCore(QueryContext context) =>
            _processor.Pipeline.CreatePipeline(new QueryDecorator<TRequest, TResponse>(_query, _message, context)).Method.InvokeAsync();

        protected override QueryContext CreateQueryContext() =>
            new QueryContext(_processor.ServiceProvider, _processor.Principal, _token, _message);
    }
}
