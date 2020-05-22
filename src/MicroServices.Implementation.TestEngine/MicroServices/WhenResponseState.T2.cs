using System;
using Kingo.Reflection;
using static Kingo.MicroServices.MicroProcessorTestContext;

namespace Kingo.MicroServices
{
    internal sealed class WhenResponseState<TRequest, TResponse> : MicroProcessorTestState, IWhenResponseState<TRequest, TResponse>
    {
        private readonly MicroProcessorTest _test;
        private readonly MicroProcessorTestOperationQueue _givenOperations;

        public WhenResponseState(MicroProcessorTest test, MicroProcessorTestOperationQueue givenOperations)
        {
            _test = test;
            _givenOperations = givenOperations;
        }

        protected override MicroProcessorTest Test =>
            _test;

        public override string ToString() =>
            $"Configuring a query of type '{typeof(IQuery<TRequest, TResponse>).FriendlyName()}'...";

        public IReadyToRunQueryTestState<TRequest, TResponse> IsExecutedBy<TQuery>(TRequest request) where TQuery : class, IQuery<TRequest, TResponse> =>
            IsExecutedBy<TQuery>(ConfigureRequest(request));

        public IReadyToRunQueryTestState<TRequest, TResponse> IsExecutedBy<TQuery>(Action<QueryTestOperationInfo<TRequest>, MicroProcessorTestContext> configurator) where TQuery : class, IQuery<TRequest, TResponse> =>
            MoveToReadyToRunQueryState(new QueryTestOperation2<TRequest, TResponse, TQuery>(configurator));

        public IReadyToRunQueryTestState<TRequest, TResponse> IsExecutedBy(IQuery<TRequest, TResponse> query, Action<QueryTestOperationInfo<TRequest>, MicroProcessorTestContext> configurator) =>
            MoveToReadyToRunQueryState(new QueryTestOperation2<TRequest, TResponse>(configurator, query));

        private IReadyToRunQueryTestState<TRequest, TResponse> MoveToReadyToRunQueryState(QueryTestOperation<TRequest, TResponse> whenOperation) =>
            Test.MoveToState(this, new ReadyToRunQueryTestState<TRequest, TResponse>(_test, _givenOperations, whenOperation));
    }
}
