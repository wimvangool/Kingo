using System;
using System.Threading;
using System.Threading.Tasks;

namespace Kingo.MicroServices
{
    internal sealed class QueryOperationImplementation<TResponse> : QueryOperation<TResponse>
    {
        #region [====== MethodOperation ======]

        private sealed class MethodOperation : ExecuteAsyncMethodOperation<TResponse>
        {
            private readonly QueryOperationImplementation<TResponse> _operation;
            private readonly ExecuteAsyncMethod<TResponse> _method;
            private readonly QueryOperationContext _context;

            public MethodOperation(QueryOperationImplementation<TResponse> operation, MicroProcessorOperationContext context)
            {
                _operation = operation;
                _method = new ExecuteAsyncMethod<TResponse>(operation._query);
                _context = context.PushOperation(this);
            }

            public override IMessageToProcess Message =>
                _operation.Message;

            public override CancellationToken Token =>
                _operation.Token;

            public override MicroProcessorOperationType Type =>
                _operation.Type;

            public override MicroProcessorOperationKind Kind =>
                _operation.Kind;

            public override ExecuteAsyncMethod Method =>
                _method;

            public override QueryOperationContext Context =>
                _context;

            public override async Task<QueryOperationResult<TResponse>> ExecuteAsync() =>
                new QueryOperationResult<TResponse>(await ExecuteMethodAsync().ConfigureAwait(false));

            private async Task<MessageEnvelope<TResponse>> ExecuteMethodAsync() =>
                Context.Processor.CreateMessageFactory().Wrap(await _method.ExecuteAsync(_context).ConfigureAwait(false));
        }

        #endregion

        private readonly IQuery<TResponse> _query;

        public QueryOperationImplementation(MicroProcessor processor, IQuery<TResponse> query, CancellationToken? token) :
            base(processor, token)
        {
            _query = query ?? throw new ArgumentNullException(nameof(query));
        }

        public QueryOperationImplementation(MicroProcessorOperationContext context, IQuery<TResponse> query, CancellationToken? token) :
            base(context, token)
        {
            _query = query ?? throw new ArgumentNullException(nameof(query));
        }

        public override IMessageToProcess Message =>
            null;

        protected override ExecuteAsyncMethodOperation<TResponse> CreateMethodOperation(MicroProcessorOperationContext context) =>
            new MethodOperation(this, context);
    }
}
