using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kingo.MicroServices.TestEngine
{
    internal sealed class RunningMessageHandlerTestState<TMessage> : RunningTestState
    {
        public RunningMessageHandlerTestState(MicroProcessorTest test, IEnumerable<MicroProcessorTestOperation> operations) :
            base(test, operations) { }

        public Task<VerifyingMessageHandlerTestOutputState<TMessage>> RunAsync(MessageHandlerTestOperation<TMessage> operation) =>
            RunAsync(operation, Test.CreateTestContext());

        private async Task<VerifyingMessageHandlerTestOutputState<TMessage>> RunAsync(MessageHandlerTestOperation<TMessage> operation, MicroProcessorTestContext context)
        {
            using (context.Processor.ServiceProvider.CreateScope())
            {
                await RunGivenOperationsAsync(context);

                try
                {
                    return MoveToOutputState(context, await operation.RunAsync(this, context));
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

        private VerifyingMessageHandlerTestOutputState<TMessage> MoveToOutputState(MicroProcessorTestContext context, MicroProcessorTestOperationId operationId) =>
            MoveTo(new VerifyingMessageHandlerTestOutputState<TMessage>(Test, context, operationId));

        private VerifyingMessageHandlerTestOutputState<TMessage> MoveToOutputState(MicroProcessorTestContext context, MicroProcessorOperationException exception) =>
            MoveTo(new VerifyingMessageHandlerTestOutputState<TMessage>(Test, context, exception));

        private VerifyingMessageHandlerTestOutputState<TMessage> MoveTo(VerifyingMessageHandlerTestOutputState<TMessage> newState) =>
            Test.MoveToState(this, newState);
    }
}
