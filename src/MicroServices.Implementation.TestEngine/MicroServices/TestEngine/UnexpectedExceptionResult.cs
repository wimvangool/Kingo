using System;
using Kingo.Reflection;

namespace Kingo.MicroServices.TestEngine
{
    internal sealed class UnexpectedExceptionResult : IRunTestResult
    {
        private readonly IMicroProcessorOperationTest _test;
        private readonly Exception _exception;

        public UnexpectedExceptionResult(IMicroProcessorOperationTest test, Exception exception)
        {
            _test = test;
            _exception = exception;
        }

        public void Complete() =>
            throw NewUnexpectedExceptionWasThrownException();

        private Exception NewUnexpectedExceptionWasThrownException()
        {
            var messageFormat = ExceptionMessages.UnexpectedExceptionResult_UnexpectedException;
            var message = string.Format(messageFormat, _test.GetType().FriendlyName(), _exception.GetType().FriendlyName());
            return new TestFailedException(message, _exception);
        }
    }
}
