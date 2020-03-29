using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kingo.MicroServices.TestEngine
{
    internal sealed class QueryTestOperation1<TResponse, TQuery> : QueryTestOperation<TResponse>
        where TQuery : class, IQuery<TResponse>
    {
        public QueryTestOperation1(Action<QueryTestOperationInfo, MicroProcessorTestContext> configurator) :
            base(configurator) { }

        public override Type QueryType =>
            typeof(TQuery);

        public override Task<MicroProcessorTestOperationId> RunAsync(RunningTestState state, MicroProcessorTestContext context) =>
            CreateOperation(context.Resolve<TQuery>()).RunAsync(state, context);

        private QueryTestOperation1<TResponse> CreateOperation(IQuery<TResponse> query) =>
            new QueryTestOperation1<TResponse>(this, query);
    }
}
