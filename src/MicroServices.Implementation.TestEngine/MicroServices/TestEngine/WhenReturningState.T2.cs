using System;
using System.Collections.Generic;
using System.Text;
using Kingo.Reflection;

namespace Kingo.MicroServices.TestEngine
{
    internal sealed class WhenReturningState<TRequest, TResponse> : MicroProcessorTestState, IWhenReturningState<TRequest, TResponse>
    {
        private readonly MicroProcessorTest _test;
        private readonly MicroProcessorTestOperationQueue _givenOperations;

        public WhenReturningState(MicroProcessorTest test, MicroProcessorTestOperationQueue givenOperations)
        {
            _test = test;
            _givenOperations = givenOperations;
        }

        protected override MicroProcessorTest Test =>
            _test;

        public override string ToString() =>
            $"Configuring a query of type '{typeof(IQuery<TRequest, TResponse>).FriendlyName()}'...";

        public IReadyToRunQueryTestState<TRequest, TResponse> IsExecutedBy<TQuery>(Action<QueryTestOperationInfo<TRequest>, MicroProcessorTestContext> configurator) where TQuery : class, IQuery<TRequest, TResponse> =>
            MoveToReadyToRunQueryState(new QueryTestOperation2<TRequest, TResponse, TQuery>(configurator));

        public IReadyToRunQueryTestState<TRequest, TResponse> IsExecutedByQuery(Action<QueryTestOperationInfo<TRequest>, MicroProcessorTestContext> configurator, IQuery<TRequest, TResponse> query) =>
            MoveToReadyToRunQueryState(new QueryTestOperation2<TRequest, TResponse>(configurator, query));

        private IReadyToRunQueryTestState<TRequest, TResponse> MoveToReadyToRunQueryState(QueryTestOperation<TRequest, TResponse> whenOperation) =>
            Test.MoveToState(this, new ReadyToRunQueryTestState<TRequest, TResponse>(_test, _givenOperations, whenOperation));
    }
}
