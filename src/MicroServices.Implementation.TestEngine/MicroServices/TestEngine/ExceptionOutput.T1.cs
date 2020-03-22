using System;
using System.Collections.Generic;
using System.Text;

namespace Kingo.MicroServices.TestEngine
{
    internal sealed class ExceptionOutput<TMessage> : IMessageHandlerTestOutput<TMessage>
    {
        private readonly MicroProcessorTestContext _context;
        private readonly ExceptionOutput _exception;

        public ExceptionOutput(MicroProcessorTestContext context, MicroProcessorOperationException exception)
        {
            _context = context;
            _exception = new ExceptionOutput(exception);
        }

        public void AssertOutputIsException<TException>(Action<TMessage, TException, MicroProcessorTestContext> assertMethod) where TException : MicroProcessorOperationException =>
            AssertOutputIsException(assertMethod, _exception.GetInputMessage<TMessage>(), _exception.IsExceptionOfType<TException>());

        private void AssertOutputIsException<TException>(Action<TMessage, TException, MicroProcessorTestContext> assertMethod, MessageEnvelope<TMessage> message, TException exception) where TException : Exception =>
            assertMethod?.Invoke(message.Content, exception, _context);

        public void AssertOutputIsMessageStream(Action<TMessage, MessageStream, MicroProcessorTestContext> assertMethod) =>
            _exception.IsUnexpectedException();
    }
}
