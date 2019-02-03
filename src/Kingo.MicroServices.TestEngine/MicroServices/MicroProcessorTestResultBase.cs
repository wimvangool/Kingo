using System;
using Kingo.Reflection;

namespace Kingo.MicroServices
{
    internal abstract class MicroProcessorTestResultBase : IMicroProcessorTestResult
    {
        public abstract IInnerExceptionResult IsExceptionOfType<TException>(Action<TException> assertion = null) where TException : Exception;        

        #region [====== ExceptionResult ======]

        private sealed class InnerExceptionResult : IInnerExceptionResult
        {
            private readonly Exception _exception;

            public InnerExceptionResult(Exception exception)
            {
                _exception = exception;
            }

            public IInnerExceptionResult WithInnerExceptionOfType<TException>(Action<TException> assertion = null)
                where TException : Exception
            {
                if (_exception.InnerException == null)
                {
                    throw NewInnerExceptionNullException(_exception, typeof(TException));
                }
                return IsExpectedException(_exception.InnerException, assertion);
            }

            private static Exception NewInnerExceptionNullException(Exception exception, Type expectedExceptionType)
            {
                var messageFormat = ExceptionMessages.MicroProcessorTestResult_InnerExceptionNull;
                var message = string.Format(messageFormat, exception.GetType(), expectedExceptionType.FriendlyName());
                return new TestFailedException(message);
            }
        }

        protected static IInnerExceptionResult IsExpectedException<TException>(Exception exception, Action<TException> assertion = null) where TException : Exception
        {
            if (exception is TException expectedException)
            {
                try
                {
                    assertion?.Invoke(expectedException);
                }
                catch (Exception innerException)
                {
                    throw NewAssertionOfExceptionFailedException(exception.GetType(), innerException);
                }
                return new InnerExceptionResult(exception);
            }
            else
            {
                throw NewExceptionNotOfExpectedTypeException(typeof(TException), exception.GetType());
            }            
        }

        private static Exception NewExceptionNotOfExpectedTypeException(Type expectedType, Type actualType)
        {
            var messageFormat = ExceptionMessages.MicroProcessorTestResult_ExceptionNotOfExpectedType;
            var message = string.Format(messageFormat, expectedType.FriendlyName(), actualType.FriendlyName());
            return new TestFailedException(message);
        }

        protected static Exception NewExceptionThrownException(Exception exception)
        {
            var messageFormat = ExceptionMessages.MicroProcessorTestResult_ExceptionThrown;
            var message = string.Format(messageFormat, exception.GetType().FriendlyName());
            return new TestFailedException(message, exception);
        }

        private static Exception NewAssertionOfExceptionFailedException(Type exceptionType, Exception innerException)
        {
            var messageFormat = ExceptionMessages.MicroProcessorTestResult_AssertionOfExceptionFailed;
            var message = string.Format(messageFormat, exceptionType.FriendlyName());
            return new TestFailedException(message, innerException);
        }

        #endregion

        #region [====== NoExceptionResult ======]

        protected static Exception NewExceptionNotThrownException(Type expectedType)
        {
            var messageFormat = ExceptionMessages.MicroProcessorTestResult_ExceptionNotThrown;
            var message = string.Format(messageFormat, expectedType.FriendlyName());
            return new TestFailedException(message);
        }

        #endregion
    }
}
