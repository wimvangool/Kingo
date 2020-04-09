using System;
using System.Collections.Generic;
using System.Text;

namespace Kingo.MicroServices.TestEngine
{
    internal sealed class MessageHandlerExceptionOutput<TMessage> : IMessageHandlerTestOutput<TMessage>
    {
        #region [====== IsExceptionMethod<TException> ======]

        private sealed class IsExceptionMethod<TException> : ITestOutputAssertMethod where TException : MicroProcessorOperationException
        {
            private readonly MessageHandlerExceptionOutput<TMessage> _output;
            private readonly Action<TMessage, TException, MicroProcessorTestContext> _assertMethod;

            public IsExceptionMethod(MessageHandlerExceptionOutput<TMessage> output, Action<TMessage, TException, MicroProcessorTestContext> assertMethod)
            {
                _output = output;
                _assertMethod = assertMethod;
            }

            public void Execute() =>
                Execute(_output._output.GetInputMessage<TMessage>(), _output._output.IsExceptionOfType<TException>());

            private void Execute(MessageEnvelope<TMessage> message, TException exception) =>
                _assertMethod?.Invoke(message.Content, exception, _output._context);
        }

        #endregion

        private readonly MicroProcessorTestContext _context;
        private readonly ExceptionOutput _output;

        public MessageHandlerExceptionOutput(MicroProcessorTestContext context, MicroProcessorOperationException exception)
        {
            _context = context;
            _output = new ExceptionOutput(exception);
        }

        public ITestOutputAssertMethod IsException<TException>(Action<TMessage, TException, MicroProcessorTestContext> assertMethod) where TException : MicroProcessorOperationException =>
            new IsExceptionMethod<TException>(this, assertMethod);

        public ITestOutputAssertMethod IsMessageStream(Action<TMessage, MessageStream, MicroProcessorTestContext> assertMethod) =>
            _output.IsNoException();
    }
}
