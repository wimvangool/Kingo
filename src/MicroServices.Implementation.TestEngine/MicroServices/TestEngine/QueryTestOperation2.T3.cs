using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kingo.MicroServices.TestEngine
{
    internal sealed class QueryTestOperation2<TRequest, TResponse, TQuery> : QueryTestOperation<TRequest, TResponse>
        where TQuery : class, IQuery<TRequest, TResponse>
    {
        public QueryTestOperation2(Action<QueryTestOperationInfo<TRequest>, MicroProcessorTestContext> configurator) :
            base(configurator) { }

        public override Type QueryType =>
            typeof(TQuery);

        public override Task<MicroProcessorTestOperationId> RunAsync(RunningTestState state, Queue<MicroProcessorTestOperation> nextOperations, MicroProcessorTestContext context) =>
            CreateOperation(context.Resolve<TQuery>()).RunAsync(state, nextOperations, context);

        private QueryTestOperation2<TRequest, TResponse> CreateOperation(IQuery<TRequest, TResponse> query) =>
            new QueryTestOperation2<TRequest, TResponse>(this, query);
    }
}
