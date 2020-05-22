using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kingo.Reflection;

namespace Kingo.MicroServices
{
    internal sealed class ReadyToRunQueryTestState<TResponse> : ReadyToRunTestState<QueryTestOperation<TResponse>, VerifyingQueryTestOutputState<TResponse>>,
                                                                IReadyToRunQueryTestState<TResponse>
    {
        private readonly MicroProcessorTest _test;
        private readonly IEnumerable<MicroProcessorTestOperation> _givenOperations;
        private readonly QueryTestOperation<TResponse> _whenOperation;

        public ReadyToRunQueryTestState(MicroProcessorTest test, IEnumerable<MicroProcessorTestOperation> givenOperations, QueryTestOperation<TResponse> whenOperation)
        {
            _test = test;
            _givenOperations = givenOperations;
            _whenOperation = whenOperation;
        }

        protected override MicroProcessorTest Test =>
            _test;

        protected override QueryTestOperation<TResponse> WhenOperation =>
            _whenOperation;

        public override string ToString() =>
            $"Ready to process request with query of type '{_whenOperation.QueryType.FriendlyName()}'...";

        public async Task ThenOutputIs<TException>(Action<TException, MicroProcessorTestContext> assertMethod = null) where TException : MicroProcessorOperationException =>
            (await RunTestAsync()).AssertOutputIsException(assertMethod);

        public async Task ThenOutputIsResponse(Action<TResponse, MicroProcessorTestContext> assertMethod = null) =>
            (await RunTestAsync()).AssertOutputIsResponse(assertMethod);

        protected override RunningTestState<QueryTestOperation<TResponse>, VerifyingQueryTestOutputState<TResponse>> CreateRunningTestState() =>
            new RunningQueryTestState<TResponse>(_test, _givenOperations);
    }
}
