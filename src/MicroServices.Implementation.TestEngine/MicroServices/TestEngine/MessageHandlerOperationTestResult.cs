using System;
using Kingo.Reflection;

namespace Kingo.MicroServices.TestEngine
{
    internal sealed class MessageHandlerOperationTestResult : MicroProcessorOperationTestResult, IMessageHandlerOperationTestResult
    {
        #region [====== ExceptionResult ======]

        private sealed class ExceptionResult : MicroProcessorOperationTestResultBase, IMessageHandlerOperationTestResult
        {
            private readonly Exception _exception;            

            public ExceptionResult(Exception exception)
            {                
                _exception = exception;
            }

            public override string ToString() =>
                _exception.GetType().FriendlyName();

            public override IInnerExceptionResult IsExceptionOfType<TException>(Action<TException> assertion = null) =>
                IsExpectedException(_exception, assertion);

            public void IsMessageStream(int minLength, int maxLength, Action <MessageStream> assertion) =>
                throw NewExceptionThrownException(_exception);
        }

        #endregion

        #region [====== OutputStreamResult ======]

        private sealed class OutputStreamResult : MicroProcessorOperationTestResultBase, IMessageHandlerOperationTestResult
        {                        
            private readonly MessageStream _stream;

            public OutputStreamResult(MessageStream stream)
            {
                _stream = stream;
            }

            public override string ToString() =>
                $"{nameof(MessageStream)} ({_stream})";

            public override IInnerExceptionResult IsExceptionOfType<TException>(Action<TException> assertion = null) =>
                throw NewExceptionNotThrownException(typeof(TException));

            public void IsMessageStream(int minLength, int maxLength, Action<MessageStream> assertion = null)
            {
                if (HasExpectedLength(minLength, maxLength, _stream.Count))
                {
                    if (assertion == null)
                    {
                        return;
                    }
                    try
                    {
                        assertion.Invoke(_stream);
                    }
                    catch (Exception innerException)
                    {
                        throw NewAssertionOfEventStreamFailedException(innerException);
                    }
                    return;
                }
                throw NewUnexpectedStreamLengthException(minLength, maxLength, _stream.Count);
            }

            private static bool HasExpectedLength(int minLength, int maxLength, int actualLength)
            {
                if (0 <= minLength && minLength <= maxLength)
                {
                    return minLength <= actualLength && actualLength <= maxLength;
                }
                throw NewInvalidStreamLengthSpecifiedException(minLength, maxLength);
            }

            private static Exception NewInvalidStreamLengthSpecifiedException(int minLength, int maxLength)
            {
                var messageFormat = ExceptionMessages.MessageHandlerOperationResult_InvalidStreamLengthSpecified;
                var message = string.Format(messageFormat, FormatExpectedLength(minLength, maxLength));
                return new ArgumentOutOfRangeException(message);
            }

            private static Exception NewUnexpectedStreamLengthException(int minLength, int maxLength, int actualLength)
            {
                var messageFormat = ExceptionMessages.MessageHandlerOperationResult_UnexpectedStreamLength;
                var message = string.Format(messageFormat, actualLength, FormatExpectedLength(minLength, maxLength));
                return new TestFailedException(message);
            }

            private static string FormatExpectedLength(int minLength, int maxLength) =>
                minLength == maxLength ? minLength.ToString() : $"[{minLength}, {maxLength}]";

            private static Exception NewAssertionOfEventStreamFailedException(Exception innerException)
            {
                var messageFormat = ExceptionMessages.MessageHandlerOperationTestResult_AssertionOfMessageStreamFailed;
                var message = string.Format(messageFormat, nameof(MessageStream));
                throw new TestFailedException(message, innerException);
            }
        }

        #endregion

        private readonly IMessageHandlerOperationTestResult _result;

        public MessageHandlerOperationTestResult(Exception exception)
        {
            _result = new ExceptionResult(exception);
        }

        public MessageHandlerOperationTestResult(MessageStream stream)
        {
            _result = new OutputStreamResult(stream);
        }

        public override string ToString() =>
            _result.ToString();

        public override IInnerExceptionResult IsExceptionOfType<TException>(Action<TException> assertion = null)
        {
            try
            {
                return _result.IsExceptionOfType(assertion);
            }
            finally
            {
                OnVerified();
            }
        }            

        public void IsMessageStream(int minLength, int maxLength, Action <MessageStream> assertion = null)
        {
            try
            {
                _result.IsMessageStream(minLength, maxLength, assertion);
            }
            finally
            {
                OnVerified();
            }
        }                    
    }
}
