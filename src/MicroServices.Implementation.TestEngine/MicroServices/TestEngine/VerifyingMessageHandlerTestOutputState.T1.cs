using System;
using System.Collections.Generic;
using System.Text;

namespace Kingo.MicroServices.TestEngine
{
    internal sealed class VerifyingMessageHandlerTestOutputState<TMessage> : MicroProcessorTestState
    {
        private readonly MicroProcessorTest _test;

        public VerifyingMessageHandlerTestOutputState(MicroProcessorTest test, MessageEnvelope<TMessage> message, Exception exception)
        {
            _test = test;
        }

        public VerifyingMessageHandlerTestOutputState(MicroProcessorTest test, MessageEnvelope<TMessage> message, MessageStream stream)
        {
            _test = test;
        }

        protected override MicroProcessorTest Test =>
            throw new NotImplementedException();

        public void AssertOutputIsException<TException>(Action<TMessage, TException, MicroProcessorTestContext> assertMethod) where TException : Exception =>
            throw new NotImplementedException();

        public void AssertOutputIsMessageStream(Action<TMessage, MessageStream, MicroProcessorTestContext> assertMethod) =>
            throw new NotImplementedException();
    }
}
