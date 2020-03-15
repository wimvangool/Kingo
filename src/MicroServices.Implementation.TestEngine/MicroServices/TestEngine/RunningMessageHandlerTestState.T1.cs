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
                    await operation.RunAsync(this, context);
                }
                catch (Exception exception)
                {
                    throw new NotImplementedException();
                }
                throw new NotImplementedException();
            }
        }
    }
}
