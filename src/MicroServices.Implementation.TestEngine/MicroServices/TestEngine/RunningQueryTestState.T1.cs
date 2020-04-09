using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kingo.MicroServices.TestEngine
{
    internal sealed class RunningQueryTestState<TResponse> : RunningTestState<QueryTestOperation<TResponse>, VerifyingQueryTestOutputState<TResponse>>
    {
        public RunningQueryTestState(MicroProcessorTest test, IEnumerable<MicroProcessorTestOperation> operations) :
            base(test, operations) { }

        protected override VerifyingQueryTestOutputState<TResponse> CreateVerifyingOutputState(MicroProcessorTestContext context, MicroProcessorTestOperationId operationId) =>
            new VerifyingQueryTestOutputState<TResponse>(Test, context, operationId);

        protected override VerifyingQueryTestOutputState<TResponse> CreateVerifyingOutputState(MicroProcessorTestContext context, MicroProcessorOperationException exception) =>
            new VerifyingQueryTestOutputState<TResponse>(Test, context, exception);
    }
}
