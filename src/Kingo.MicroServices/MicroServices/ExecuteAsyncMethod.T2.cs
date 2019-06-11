using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kingo.MicroServices
{
    internal sealed class ExecuteAsyncMethod<TRequest, TResponse> : ExecuteAsyncMethod, IQuery<TRequest, TResponse>
    {
        private readonly IQuery<TRequest, TResponse> _query;

        public ExecuteAsyncMethod(IQuery<TRequest, TResponse> query) :
            base(QueryType.FromInstance(query), QueryInterface.FromType<TRequest, TResponse>())
        {
            _query = query;
        }

        public Task<TResponse> ExecuteAsync(TRequest message, QueryOperationContext context) =>
            _query.ExecuteAsync(message, context);
    }
}
