using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kingo.MicroServices.TestEngine
{
    internal sealed class QueryTestOperation1<TResponse> : QueryTestOperation<TResponse>
    {
        private readonly IQuery<TResponse> _query;

        public QueryTestOperation1(QueryTestOperation<TResponse> operation, IQuery<TResponse> query) :
            base(operation)
        {
            _query = query ?? throw new ArgumentNullException(nameof(query));
        }

        public QueryTestOperation1(Action<QueryTestOperationInfo, MicroProcessorTestContext> configurator, IQuery<TResponse> query) :
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
