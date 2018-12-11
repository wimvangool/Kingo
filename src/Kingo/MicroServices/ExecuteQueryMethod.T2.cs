using System;
using System.Threading;
using System.Threading.Tasks;

namespace Kingo.MicroServices
{
    internal sealed class ExecuteQueryMethod<TMessageIn, TMessageOut> : ExecuteQueryMethodBase<TMessageOut>
    {
        private readonly MicroProcessor _processor;
        private readonly CancellationToken? _token;
        private readonly IQuery<TMessageIn, TMessageOut> _query;
        private readonly TMessageIn _message;

        public ExecuteQueryMethod(MicroProcessor processor, CancellationToken? token, IQuery<TMessageIn, TMessageOut> query, TMessageIn message)            
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

        protected override Task<InvokeAsyncResult<TMessageOut>> InvokeAsyncCore(QueryContext context) =>
            _processor.PipelineFactory.CreatePipeline(new QueryDecorator<TMessageIn, TMessageOut>(_query, _message, context)).Method.InvokeAsync();

        protected override QueryContext CreateQueryContext() =>
            new QueryContext(_processor.MessageHandlerFactory, _processor.Principal, _token, _message);
    }
}
