using System;
using System.Threading;
using System.Threading.Tasks;
using static Kingo.Ensure;

namespace Kingo.MicroServices
{
    internal sealed class QueryOperationImplementation<TRequest, TResponse> : QueryOperation<TResponse>
    {
        #region [====== MethodOperation ======]

        private sealed class MethodOperation : ExecuteAsyncMethodOperation<TResponse>
        {
            private readonly QueryOperationImplementation<TRequest, TResponse> _operation;
            private readonly ExecuteAsyncMethod<TRequest, TResponse> _method;
            private readonly QueryOperationContext _context;

            public MethodOperation(QueryOperationImplementation<TRequest, TResponse> operation, MicroProcessorOperationContext context)
            {
                _operation = operation;
                _method = new ExecuteAsyncMethod<TRequest, TResponse>(operation._query);
                _context = context.PushOperation(this);
            }

            public override IMessage Message =>
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

            private async Task<Message<TResponse>> ExecuteMethodAsync() =>
                Context.Processor.MessageFactory
                    .CreateMessage(await _method.ExecuteAsync(_operation._message.Content, _context).ConfigureAwait(false))
                    .CorrelateWith(Message);
        }

        #endregion
        
        private readonly IQuery<TRequest, TResponse> _query;
        private readonly Message<TRequest> _message;

        public QueryOperationImplementation(MicroProcessor processor, IQuery<TRequest, TResponse> query, Message<TRequest> message, CancellationToken? token) :
            base(processor, token)
        {
            _query = IsNotNull(query, nameof(query));
            _message = IsNotNull(message, nameof(message));
        }

        public QueryOperationImplementation(MicroProcessorOperationContext context, IQuery<TRequest, TResponse> query, Message<TRequest> message, CancellationToken? token) :
            base(context, token)
        {
            _query = IsNotNull(query, nameof(query));
            _message = IsNotNull(message, nameof(message));
        }

        public override IMessage Message =>
            _message;

        protected override ExecuteAsyncMethodOperation<TResponse> CreateMethodOperation(MicroProcessorOperationContext context) => 
            new MethodOperation(this, context);        
    }
}
