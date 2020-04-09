using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kingo.MicroServices.TestEngine
{
    internal sealed class RunningMessageHandlerTestState<TMessage> : RunningTestState<MessageHandlerTestOperation<TMessage>, VerifyingMessageHandlerTestOutputState<TMessage>>
    {
        public RunningMessageHandlerTestState(MicroProcessorTest test, IEnumerable<MicroProcessorTestOperation> operations) :
            base(test, operations) { }

        protected override VerifyingMessageHandlerTestOutputState<TMessage> CreateVerifyingOutputState(MicroProcessorTestContext context, MicroProcessorTestOperationId operationId) =>
            new VerifyingMessageHandlerTestOutputState<TMessage>(Test, context, operationId);

        protected override VerifyingMessageHandlerTestOutputState<TMessage> CreateVerifyingOutputState(MicroProcessorTestContext context, MicroProcessorOperationException exception) =>
            new VerifyingMessageHandlerTestOutputState<TMessage>(Test, context, exception);
    }
}
