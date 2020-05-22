using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kingo.MicroServices.DataContracts;

namespace Kingo.MicroServices
{
    internal abstract class RunningTestState<TOperation, TOutputState> : RunningTestState
        where TOperation : MicroProcessorTestOperation
        where TOutputState : MicroProcessorTestState
    {
        protected RunningTestState(MicroProcessorTest test, IEnumerable<MicroProcessorTestOperation> givenOperations) :
            base(test, givenOperations) { }

        public Task<TOutputState> RunAsync(TOperation operation) =>
            RunAsync(operation, Test.CreateTestContext());

        private async Task<TOutputState> RunAsync(TOperation operation, MicroProcessorTestContext context)
        {
            using (context.Processor.ServiceProvider.CreateScope())
            {
                try
                {
                    return MoveToOutputState(context, await RunOperationsAsync(operation, context));
                }
                catch (MicroProcessorOperationException exception)
                {
                    return MoveToOutputState(context, exception);
                }
                catch (TestFailedException)
                {
                    throw;
                }
                catch (Exception exception)
                {
                    throw NewOperationFailedException(operation, exception);
                }
            }
        }

        private TOutputState MoveToOutputState(MicroProcessorTestContext context, MicroProcessorTestOperationId operationId) =>
            MoveTo(CreateVerifyingOutputState(context, operationId));

        private TOutputState MoveToOutputState(MicroProcessorTestContext context, MicroProcessorOperationException exception) =>
            MoveTo(CreateVerifyingOutputState(context, exception));

        private TOutputState MoveTo(TOutputState newState) =>
            Test.MoveToState(this, newState);

        protected abstract TOutputState CreateVerifyingOutputState(MicroProcessorTestContext context, MicroProcessorTestOperationId operationId);

        protected abstract TOutputState CreateVerifyingOutputState(MicroProcessorTestContext context, MicroProcessorOperationException exception);
    }
}
