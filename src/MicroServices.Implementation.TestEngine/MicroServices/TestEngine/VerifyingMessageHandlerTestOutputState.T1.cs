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
            _output = new ExceptionOutput<TMessage>(context, exception);
        }

        public VerifyingMessageHandlerTestOutputState(MicroProcessorTest test, MicroProcessorTestContext context, MicroProcessorTestOperationId operationId)
        {
            _test = test;
            _output = new MessageStreamOutput<TMessage>(context, operationId);
        }

        protected override MicroProcessorTest Test =>
            _test;

        public void AssertOutputIsException<TException>(Action<TMessage, TException, MicroProcessorTestContext> assertMethod) where TException : MicroProcessorOperationException
        {
            try
            {
                _output.AssertOutputIsException(assertMethod);
            }
            catch (TestFailedException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw NewOutputAssertionFailedException(exception);
            }
            finally
            {
                MoveToEndState();
            }
        }

        public void AssertOutputIsMessageStream(Action<TMessage, MessageStream, MicroProcessorTestContext> assertMethod)
        {
            try
            {
                _output.AssertOutputIsMessageStream(assertMethod);
            }
            catch (TestFailedException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw NewOutputAssertionFailedException(exception);
            }
            finally
            {
                MoveToEndState();
            }
        }
    }
}
