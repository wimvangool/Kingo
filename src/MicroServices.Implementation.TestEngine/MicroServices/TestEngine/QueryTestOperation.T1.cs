using System;
using Kingo.Reflection;

namespace Kingo.MicroServices.TestEngine
{
    internal abstract class QueryTestOperation<TResponse> : QueryTestOperation
    {
        private readonly Action<QueryTestOperationInfo, MicroProcessorTestContext> _configurator;

        protected QueryTestOperation(QueryTestOperation<TResponse> operation)
        {
            _configurator = operation._configurator;
        }

        protected QueryTestOperation(Action<QueryTestOperationInfo, MicroProcessorTestContext> configurator)
        {
            _configurator = configurator;
        }

        public override string ToString() =>
            $"{QueryType.FriendlyName()}.{nameof(IQuery<TResponse>.ExecuteAsync)}({nameof(QueryOperationContext)})";

        protected QueryTestOperationInfo CreateOperationInfo(MicroProcessorTestContext context) =>
            context.CreateOperationInfo(_configurator);
    }
}
