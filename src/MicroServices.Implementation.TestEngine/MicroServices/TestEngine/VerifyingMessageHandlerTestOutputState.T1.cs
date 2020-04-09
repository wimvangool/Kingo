using System;
using System.Collections.Generic;
using System.Text;

namespace Kingo.MicroServices.TestEngine
{
    internal sealed class VerifyingMessageHandlerTestOutputState<TMessage> : VerifyingOutputState
    {
        private readonly MicroProcessorTest _test;
        private readonly IMessageHandlerTestOutput<TMessage> _output;

        public VerifyingMessageHandlerTestOutputState(MicroProcessorTest test, MicroProcessorTestContext context, MicroProcessorOperationException exception)
        {
            _test = test;
            _output = new MessageHandlerExceptionOutput<TMessage>(context, exception);
        }

        public VerifyingMessageHandlerTestOutputState(MicroProcessorTest test, MicroProcessorTestContext context, MicroProcessorTestOperationId operationId)
        {
            _test = test;
            _output = new MessageStreamOutput<TMessage>(context, operationId);
        }

        protected override MicroProcessorTest Test =>
            _test;

        public void AssertOutputIsException<TException>(Action<TMessage, TException, MicroProcessorTestContext> assertMethod) where TException : MicroProcessorOperationException =>
            VerifyThat(_output.IsException(assertMethod));

        public void AssertOutputIsMessageStream(Action<TMessage, MessageStream, MicroProcessorTestContext> assertMethod) =>
            VerifyThat(_output.IsMessageStream(assertMethod));
    }
}
