using System;

namespace Kingo.MicroServices
{
    internal abstract class MicroProcessorTestResult : IMicroProcessorTestResult
    {
        public abstract void IsExpectedException<TException>(Action<TException> assertion = null) where TException : Exception;

        #region [====== ExceptionResult ======]

        protected static void IsExpectedException<TException>(Exception exception, Action<TException> assertion = null) where TException : Exception
        {
            if (exception is TException expectedException)
            {
                assertion?.Invoke(expectedException);
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
            return new AssertFailedException(message);
        }

        protected static Exception NewExceptionThrownException(Exception exception)
        {
            var messageFormat = ExceptionMessages.MicroProcessorTestResult_ExceptionThrown;
            var message = string.Format(messageFormat, exception.GetType().FriendlyName());
            return new AssertFailedException(message, exception);
        }

        #endregion

        #region [====== NoExceptionResult ======]

        protected static Exception NewExceptionNotThrownException(Type expectedType)
        {
            var messageFormat = ExceptionMessages.MicroProcessorTestResult_ExceptionNotThrown;
            var message = string.Format(messageFormat, expectedType.FriendlyName());
            return new AssertFailedException(message);
        }

        #endregion
    }
}
