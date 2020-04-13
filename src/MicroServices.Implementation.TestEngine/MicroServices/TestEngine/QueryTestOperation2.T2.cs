using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kingo.MicroServices.TestEngine
{
    internal sealed class QueryTestOperation2<TRequest, TResponse> : QueryTestOperation<TRequest, TResponse>
    {
        private readonly IQuery<TRequest, TResponse> _query;

        public QueryTestOperation2(QueryTestOperation<TRequest, TResponse> operation, IQuery<TRequest, TResponse> query) :
            base(operation)
        {
            _query = query ?? throw new ArgumentNullException(nameof(query));
        }

        public QueryTestOperation2(Action<QueryTestOperationInfo<TRequest>, MicroProcessorTestContext> configurator, IQuery<TRequest, TResponse> query) :
            base(configurator)
        {
            _query = query ?? throw new ArgumentNullException(nameof(query));
        }

        public override Type QueryType =>
            _query.GetType();

        public override Task<MicroProcessorTestOperationId> RunAsync(RunningTestState state, Queue<MicroProcessorTestOperation> nextOperations, MicroProcessorTestContext context) =>
            state.ExecuteQuery(context, _query, CreateOperationInfo(context));
    }
}
