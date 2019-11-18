using System;
using Kingo.Reflection;

namespace Kingo.MicroServices.Controllers
{
    internal sealed class HandleMessageResult<TOutputStream> : MicroProcessorOperationTestResult, IHandleMessageResult<TOutputStream>
        where TOutputStream : MessageStream
    {
        #region [====== ExceptionResult ======]

        private sealed class ExceptionResult : MicroProcessorOperationTestResultBase, IHandleMessageResult<TOutputStream>
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

            public void IsMessageStream(Func<MessageStream, TOutputStream> assertion) =>
                throw NewExceptionThrownException(_exception);
        }

        #endregion

        #region [====== OutputStreamResult ======]

        private sealed class OutputStreamResult : MicroProcessorOperationTestResultBase, IHandleMessageResult<TOutputStream>
        {                        
            private readonly MessageStream _stream;
            private readonly Action<TOutputStream> _streamConsumer;

            public OutputStreamResult(MessageStream stream, Action<TOutputStream> streamConsumer)
            {
                _stream = stream;
                _streamConsumer = streamConsumer;
            }

            public override string ToString() =>
                $"{nameof(MessageStream)} ({_stream})";

            public override IInnerExceptionResult IsExceptionOfType<TException>(Action<TException> assertion = null) =>
                throw NewExceptionNotThrownException(typeof(TException));

            public void IsMessageStream(Func<MessageStream, TOutputStream> assertion)
            {
                if (assertion == null)
                {
                    throw new ArgumentNullException(nameof(assertion));
                }
                _streamConsumer.Invoke(AssertAndConvert(_stream, assertion));
            }                                    
            
            private static TOutputStream AssertAndConvert(MessageStream stream, Func<MessageStream, TOutputStream> assertion)
            {
                try
                {
                    return assertion.Invoke(stream);
                }
                catch (Exception innerException)
                {
                    throw NewAssertionOfEventStreamFailedException(innerException);
                }
            }

            private static Exception NewAssertionOfEventStreamFailedException(Exception innerException)
            {
                var messageFormat = ExceptionMessages.MessageHandlerResult_AssertionOfMessageStreamFailed;
                var message = string.Format(messageFormat, typeof(TOutputStream).FriendlyName());
                throw new TestFailedException(message, innerException);
            }
        }

        #endregion

        private readonly IHandleMessageResult<TOutputStream> _result;

        public HandleMessageResult(Exception exception)
        {
            _result = new ExceptionResult(exception);
        }

        public HandleMessageResult(MessageStream stream, Action<TOutputStream> streamConsumer)
        {
            _result = new OutputStreamResult(stream, streamConsumer);
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

        public void IsMessageStream(Func<MessageStream, TOutputStream> assertion)
        {
            try
            {
                _result.IsMessageStream(assertion);
            }
            finally
            {
                OnVerified();
            }
        }                    
    }
}
