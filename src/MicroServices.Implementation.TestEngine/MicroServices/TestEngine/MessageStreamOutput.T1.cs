using System;
using System.Collections.Generic;
using System.Text;

namespace Kingo.MicroServices.TestEngine
{
    internal sealed class MessageStreamOutput<TMessage> : ReturnValueOutput, IMessageHandlerTestOutput<TMessage>
    {
        #region [====== IsMessageStreamMethod ======]

        private sealed class IsMessageStreamMethod : ITestOutputAssertMethod
        {
            private readonly MessageStreamOutput<TMessage> _output;
            private readonly Action<TMessage, MessageStream, MicroProcessorTestContext> _assertMethod;

            public IsMessageStreamMethod(MessageStreamOutput<TMessage> output, Action<TMessage, MessageStream, MicroProcessorTestContext> assertMethod)
            {
                _output = output;
                _assertMethod = assertMethod;
            }

            public void Execute() =>
                Execute(_output._context.GetInputMessage<TMessage>(_output._operationId), _output._context.GetOutputStream(_output._operationId));

            private void Execute(MessageEnvelope<TMessage> message, MessageStream stream) =>
                _assertMethod?.Invoke(message.Content, stream, _output._context);
        }

        #endregion

        private readonly MicroProcessorTestContext _context;
        private readonly MicroProcessorTestOperationId _operationId;

        public MessageStreamOutput(MicroProcessorTestContext context, MicroProcessorTestOperationId operationId)
        {
            _context = context;
            _operationId = operationId;
        }

        public ITestOutputAssertMethod IsException<TException>(Action<TMessage, TException, MicroProcessorTestContext> assertMethod) where TException : MicroProcessorOperationException =>
            IsException<TException>();

        public ITestOutputAssertMethod IsMessageStream(Action<TMessage, MessageStream, MicroProcessorTestContext> assertMethod) =>
            new IsMessageStreamMethod(this, assertMethod);
    }
}
