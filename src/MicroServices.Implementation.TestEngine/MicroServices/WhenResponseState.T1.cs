﻿using System;
using Kingo.Reflection;

namespace Kingo.MicroServices
{
    internal sealed class WhenResponseState<TResponse> : MicroProcessorTestState, IWhenResponseState<TResponse>
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
            $"Configuring a query of type '{typeof(IQuery<TResponse>).FriendlyName()}'...";

        public IReadyToRunQueryTestState<TResponse> IsExecutedBy<TQuery>(Action<QueryTestOperationInfo, MicroProcessorTestContext> configurator) where TQuery : class, IQuery<TResponse> =>
            MoveToReadyToRunQueryState(new QueryTestOperation1<TResponse, TQuery>(configurator));

        public IReadyToRunQueryTestState<TResponse> IsExecutedBy(IQuery<TResponse> query, Action<QueryTestOperationInfo, MicroProcessorTestContext> configurator) =>
            MoveToReadyToRunQueryState(new QueryTestOperation1<TResponse>(configurator, query));

        private IReadyToRunQueryTestState<TResponse> MoveToReadyToRunQueryState(QueryTestOperation<TResponse> whenOperation) =>
            Test.MoveToState(this, new ReadyToRunQueryTestState<TResponse>(_test, _givenOperations, whenOperation));
    }
}