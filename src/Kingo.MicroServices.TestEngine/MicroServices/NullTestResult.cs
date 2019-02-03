using System;
using Kingo.Reflection;

namespace Kingo.MicroServices
{
    internal sealed class NullTestResult : IRunTestResult
    {
        private readonly IMicroProcessorTest _test;

        public NullTestResult(IMicroProcessorTest test)
        {
            _test = test;
        }

        public void Complete() =>
            throw NewMissingResultException();

        private Exception NewMissingResultException()
        {
            var messageFormat = ExceptionMessages.NullTestResult_MissingResult;
            var message = string.Format(messageFormat, _test.GetType().FriendlyName());
            return new TestFailedException(message);
        }
    }
}
