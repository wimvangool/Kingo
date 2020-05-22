using System;
using Kingo.Reflection;

namespace Kingo.MicroServices
{
    internal abstract class QueryTestOperation<TRequest, TResponse> : QueryTestOperation
    {
        private readonly Action<QueryTestOperationInfo<TRequest>, MicroProcessorTestContext> _configurator;

        protected QueryTestOperation(QueryTestOperation<TRequest, TResponse> operation)
        {
            _configurator = operation._configurator;
        }

        protected QueryTestOperation(Action<QueryTestOperationInfo<TRequest>, MicroProcessorTestContext> configurator)
        {
            _configurator = configurator ?? throw new ArgumentNullException(nameof(configurator));
        }

        public override string ToString() =>
            $"{QueryType.FriendlyName()}.{nameof(IQuery<TRequest, TResponse>.ExecuteAsync)}({typeof(TRequest).FriendlyName()}, {nameof(QueryOperationContext)})";

        protected QueryTestOperationInfo<TRequest> CreateOperationInfo(MicroProcessorTestContext context) =>
            context.CreateOperationInfo(_configurator);
    }
}
