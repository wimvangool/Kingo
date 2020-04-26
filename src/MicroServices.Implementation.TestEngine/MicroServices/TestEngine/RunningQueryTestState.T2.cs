using System.Collections.Generic;

namespace Kingo.MicroServices.TestEngine
{
    internal sealed class RunningQueryTestState<TRequest, TResponse> : RunningTestState<QueryTestOperation<TRequest, TResponse>, VerifyingQueryTestOutputState<TRequest, TResponse>>
    {
        public RunningQueryTestState(MicroProcessorTest test, IEnumerable<MicroProcessorTestOperation> operations) :
            base(test, operations) { }

        protected override VerifyingQueryTestOutputState<TRequest, TResponse> CreateVerifyingOutputState(MicroProcessorTestContext context, MicroProcessorTestOperationId operationId) =>
            new VerifyingQueryTestOutputState<TRequest, TResponse>(Test, context, operationId);

        protected override VerifyingQueryTestOutputState<TRequest, TResponse> CreateVerifyingOutputState(MicroProcessorTestContext context, MicroProcessorOperationException exception) =>
            new VerifyingQueryTestOutputState<TRequest, TResponse>(Test, context, exception);
    }
}
