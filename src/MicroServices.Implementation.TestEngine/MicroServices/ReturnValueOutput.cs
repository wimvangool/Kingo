using System;
using Kingo.MicroServices.DataContracts;
using Kingo.Reflection;

namespace Kingo.MicroServices
{
    internal abstract class ReturnValueOutput
    {
        #region [====== IsExceptionMethod<TException> ======]

        private sealed class IsExceptionMethod : ITestOutputAssertMethod
        {
            private readonly Type _expectedExceptionType;

            public IsExceptionMethod(Type expectedExceptionType)
            {
                _expectedExceptionType = expectedExceptionType;
            }

            public void Execute() =>
                throw NewExceptionNotThrownException(_expectedExceptionType);

            private static Exception NewExceptionNotThrownException(Type exceptionType)
            {
                var messageFormat = ExceptionMessages.MicroProcessorTest_ExceptionNotThrown;
                var message = string.Format(messageFormat, exceptionType.FriendlyName());
                return new TestFailedException(message);
            }
        }

        #endregion

        protected ITestOutputAssertMethod IsException<TException>() =>
            new IsExceptionMethod(typeof(TException));
    }
}
