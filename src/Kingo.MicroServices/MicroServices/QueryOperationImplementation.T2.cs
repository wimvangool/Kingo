using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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

            public MethodOperation(QueryOperationImplementation<TRequest, TResponse> operation, QueryOperationContext context)
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

            public override MicroProcessorOperationKinds Kind =>
                _operation.Kind;

            public override ExecuteAsyncMethod Method =>
                _method;

            public override QueryOperationContext Context =>
                _context;

            public override async Task<QueryOperationResult<TResponse>> ExecuteAsync() =>
                new QueryOperationResult<TResponse>(await _method.ExecuteAsync(_operation._message.Instance, _context));
        }

        #endregion
        
        private readonly IQuery<TRequest, TResponse> _query;
        private readonly Message<TRequest> _message;

        public QueryOperationImplementation(MicroProcessor processor, IQuery<TRequest, TResponse> query, TRequest message, CancellationToken? token) :
            base(processor, token)
        {                                  
            _query = query ?? throw new ArgumentNullException(nameof(query));
            _message = new Message<TRequest>(message, MessageKind.Request);
        }        

        public override IMessage Message =>
            _message;

        protected override ExecuteAsyncMethodOperation<TResponse> CreateMethodOperation(QueryOperationContext context) => 
            new MethodOperation(this, context);        
    }
}
