using System;
using System.Collections.Generic;
using System.Text;

namespace Kingo.MicroServices.TestEngine
{
    internal sealed class MessageStreamOutput<TMessage> : NoExceptionOutput, IMessageHandlerTestOutput<TMessage>
    {
        private readonly MicroProcessorTestOperationId _operationId;

        public MessageStreamOutput(MicroProcessorTestContext context, MicroProcessorTestOperationId operationId) : base(context)
        {
            _operationId = operationId;
        }

        public void AssertOutputIsException<TException>(Action<TMessage, TException, MicroProcessorTestContext> assertMethod) where TException : MicroProcessorOperationException =>
            throw NewExceptionNotThrownException(typeof(TException));

        public void AssertOutputIsMessageStream(Action<TMessage, MessageStream, MicroProcessorTestContext> assertMethod) =>
            AssertOutputIsMessageStream(assertMethod, Context.GetInputMessage<TMessage>(_operationId), Context.GetOutputStream(_operationId));

        private void AssertOutputIsMessageStream(Action<TMessage, MessageStream, MicroProcessorTestContext> assertMethod, MessageEnvelope<TMessage> message, MessageStream stream) =>
            assertMethod?.Invoke(message.Content, stream, Context);
    }
}
