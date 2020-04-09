using System;
using System.Collections.Generic;
using System.Text;
using Kingo.Reflection;

namespace Kingo.MicroServices.TestEngine
{
    internal sealed class ExceptionOutput
    {
        #region [====== IsNoExceptionMethod ======]

        private sealed class IsNoExceptionOutput : ITestOutputAssertMethod
        {
            private readonly ExceptionOutput _output;

            public IsNoExceptionOutput(ExceptionOutput output)
            {
                _output = output;
            }

            public void Execute() =>
                throw NewUnexpectedExceptionThrownException(_output._exception.GetType());

            private static Exception NewUnexpectedExceptionThrownException(Type exceptionType)
            {
                var messageFormat = ExceptionMessages.MicroProcessorTest_ExceptionThrown;
                var message = string.Format(messageFormat, exceptionType.FriendlyName());
                return new TestFailedException(message);
            }
        }

        #endregion

        private readonly MicroProcessorOperationException _exception;

        public ExceptionOutput(MicroProcessorOperationException exception)
        {
            _exception = exception;
        }

        public MessageEnvelope<TMessage> GetInputMessage<TMessage>() =>
            _exception.OperationStackTrace.RootOperation.Message.CastTo<TMessage>();

        #region [====== IsNoException ======]

        public ITestOutputAssertMethod IsNoException() =>
            new IsNoExceptionOutput(this);

        #endregion

        #region [====== IsExceptionOfType<TException> ======]

        public TException IsExceptionOfType<TException>() where TException : MicroProcessorOperationException
        {
            if (_exception is TException exception)
            {
                return exception;
            }
            throw NewExceptionNotOfExpectedTypeException(typeof(TException), _exception.GetType());
        }

        private static Exception NewExceptionNotOfExpectedTypeException(Type expectedType, Type actualType)
        {
            var messageFormat = ExceptionMessages.MicroProcessorTest_ExceptionNotOfExpectedType;
            var message = string.Format(messageFormat, expectedType.FriendlyName(), actualType.FriendlyName());
            return new TestFailedException(message);
        }

        #endregion
    }
}
