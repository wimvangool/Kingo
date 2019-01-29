using System;

namespace Kingo.MicroServices
{
    internal sealed class UnexpectedExceptionResult : IRunTestResult
    {
        private readonly IMicroProcessorTest _test;
        private readonly Exception _exception;

        public UnexpectedExceptionResult(IMicroProcessorTest test, Exception exception)
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
            return new MicroProcessorTestFailedException(message, _exception);
        }
    }
}
