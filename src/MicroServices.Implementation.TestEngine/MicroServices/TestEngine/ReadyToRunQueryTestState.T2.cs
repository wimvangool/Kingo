using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kingo.Reflection;

namespace Kingo.MicroServices.TestEngine
{
    internal sealed class ReadyToRunQueryTestState<TRequest, TResponse> : ReadyToRunTestState<QueryTestOperation<TRequest, TResponse>, VerifyingQueryTestOutputState<TRequest, TResponse>>,
                                                                          IReadyToRunQueryTestState<TRequest, TResponse>
    {
        private readonly MicroProcessorTest _test;
        private readonly IEnumerable<MicroProcessorTestOperation> _givenOperations;
        private readonly QueryTestOperation<TRequest, TResponse> _whenOperation;

        public ReadyToRunQueryTestState(MicroProcessorTest test, IEnumerable<MicroProcessorTestOperation> givenOperations, QueryTestOperation<TRequest, TResponse> whenOperation)
        {
            _test = test;
            _givenOperations = givenOperations;
            _whenOperation = whenOperation;
        }

        protected override MicroProcessorTest Test =>
            _test;

        protected override QueryTestOperation<TRequest, TResponse> WhenOperation =>
            _whenOperation;

        public override string ToString() =>
            $"Ready to process request of type '{typeof(TRequest).FriendlyName()}' with query of type '{_whenOperation.QueryType.FriendlyName()}'...";

        public async Task ThenOutputIs<TException>(Action<TRequest, TException, MicroProcessorTestContext> assertMethod = null) where TException : MicroProcessorOperationException =>
            (await RunTestAsync()).AssertOutputIsException(assertMethod);

        public async Task ThenOutputIsResponse(Action<TRequest, TResponse, MicroProcessorTestContext> assertMethod = null) =>
            (await RunTestAsync()).AssertOutputIsResponse(assertMethod);

        protected override RunningTestState<QueryTestOperation<TRequest, TResponse>, VerifyingQueryTestOutputState<TRequest, TResponse>> CreateRunningTestState() =>
            new RunningQueryTestState<TRequest, TResponse>(_test, _givenOperations);
    }
}
